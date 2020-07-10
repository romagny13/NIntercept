using NIntercept.Builder;
using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public sealed class ProxyScope
    {
        public const string IntercetorsFieldName = "__interceptors";
        public const string InterceptorSelectorFieldName = "__interceptorSelector";
        private const FieldAttributes StaticReadOnlyFieldAttributes = FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly;
        private ModuleScope moduleScope;
        private TypeBuilder typeBuilder;
        private ProxyTypeDefinition typeDefinition;
        private List<FieldBuilder> fields;
        private FieldBuilder[] constructorFields;
        private List<PropertyBuilder> properties;
        private List<MethodBuilder> methods;
        private List<EventBuilder> events;
        private InterceptableMethodBuilder interceptableMethodBuilder;
        private List<PropertyMapping> propertyMappings;
        private List<MethodMapping> methodMappings;
        private List<EventMapping> eventMappings;

        public ProxyScope(ModuleScope moduleScope, TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {
            if (moduleScope is null)
                throw new ArgumentNullException(nameof(moduleScope));
            if (typeBuilder is null)
                throw new ArgumentNullException(nameof(typeBuilder));
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));

            this.fields = new List<FieldBuilder>();
            this.properties = new List<PropertyBuilder>();
            this.methods = new List<MethodBuilder>();
            this.events = new List<EventBuilder>();
            this.propertyMappings = new List<PropertyMapping>();
            this.methodMappings = new List<MethodMapping>();
            this.eventMappings = new List<EventMapping>();

            this.moduleScope = moduleScope;
            this.typeBuilder = typeBuilder;
            this.typeDefinition = typeDefinition;

            interceptableMethodBuilder = typeDefinition.Options?.InterceptableMethodBuilder;
            if (interceptableMethodBuilder == null)
                interceptableMethodBuilder = new InterceptableMethodBuilder();
        }

        internal ModuleScope ModuleScope
        {
            get { return moduleScope; }
        }

        internal FieldBuilder[] ConstructorFields
        {
            get { return constructorFields; }
        }

        public ProxyTypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public IReadOnlyList<FieldBuilder> Fields
        {
            get { return fields; }
        }

        public IReadOnlyList<PropertyBuilder> Properties
        {
            get { return properties; }
        }

        public IReadOnlyList<MethodBuilder> Methods
        {
            get { return methods; }
        }

        public IReadOnlyList<EventBuilder> Events
        {
            get { return events; }
        }

        internal void DefineTypeAndMembers()
        {
            if (typeDefinition.TypeDefinitionType == TypeDefinitionType.ClassProxy)
                typeBuilder.SetParent(typeDefinition.Type);

            AddCustomAttributes();
            AddInterfaces();

            CreateFields();

            var additionalCode = typeDefinition.Options?.AdditionalCode;
            if (additionalCode != null)
                additionalCode.BeforeDefine(this);

            CreateProperties();
            CreateMethods();
            CreateEvents();

            CreateStaticConstructor();
            CreateConstructor();

            if (additionalCode != null)
                additionalCode.AfterDefine(this);
        }

        private void AddCustomAttributes()
        {
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(Constructors.SerializableAttributeConstructor, new object[0]));
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(Constructors.XmlIncludeAttributeConstructor, new object[] { typeDefinition.Type }));

            var options = typeDefinition.Options;
            if (options != null)
            {
                foreach (var additionalTypeAttribute in options.AdditionalTypeAttributes)
                    typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(additionalTypeAttribute.Constructor, additionalTypeAttribute.Args));
            }

            if (typeDefinition.TypeDefinitionType == TypeDefinitionType.InterfaceProxy)
                AttributeHelper.AddInterceptorAttributes(typeBuilder, typeDefinition.InterceptorAttributes);
        }

        private void AddInterfaces()
        {
            if (typeDefinition.TypeDefinitionType == TypeDefinitionType.InterfaceProxy)
            {
                InterfaceProxyDefinition interfaceDefinition = typeDefinition as InterfaceProxyDefinition;
                typeBuilder.AddInterfaceImplementation(typeDefinition.Type);
                typeBuilder.AddInterfacesToImplement(interfaceDefinition.Interfaces);
            }

            foreach (var mixinDefinition in typeDefinition.MixinDefinitions)
                typeBuilder.AddInterfacesToImplement(mixinDefinition.Interfaces);
        }

        private void CreateFields()
        {
            // fields injected in ctor
            var fieldList = new List<FieldBuilder>();
            fieldList.Add(DefineField(IntercetorsFieldName, typeof(IInterceptor[]), FieldAttributes.Private));

            if (typeDefinition.TargetType != null)
                fieldList.Add(DefineField(typeDefinition.TargetFieldName, typeDefinition.TargetType, FieldAttributes.Private));

            if (typeDefinition.Options?.InterceptorSelector != null)
                fieldList.Add(DefineField(InterceptorSelectorFieldName, typeof(IInterceptorSelector), FieldAttributes.Private));

            foreach (var mixinDefinition in typeDefinition.MixinDefinitions)
                fieldList.Add(DefineField(mixinDefinition.TargetFieldName, mixinDefinition.Type, FieldAttributes.Private));

            this.constructorFields = fieldList.ToArray();
        }

        private void CreateProperties()
        {
            foreach (var propertyDefinition in typeDefinition.PropertyDefinitions)
                CreateInterceptableProperty(propertyDefinition);
        }

        private void CreateMethods()
        {
            foreach (var methodDefinition in typeDefinition.MethodDefinitions)
                CreateInterceptableMethod(methodDefinition, methodDefinition.Method);
        }

        private void CreateEvents()
        {
            foreach (var eventDefinition in typeDefinition.EventDefinitions)
                CreateInterceptableEvent(eventDefinition);
        }

        private void CreateStaticConstructor()
        {
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig, CallingConventions.Standard, new Type[0]);

            var il = constructorBuilder.GetILGenerator();

            foreach (var mapping in propertyMappings)
            {
                EmitHelper.StorePropertyToField(il, mapping.Property, mapping.MemberField);
                if (mapping.GetMethodBuilder != null)
                    EmitHelper.StoreMethodToField(il, mapping.GetMethodBuilder, mapping.GetMethodField);
                if (mapping.SetMethodBuilder != null)
                    EmitHelper.StoreMethodToField(il, mapping.SetMethodBuilder, mapping.SetMethodField);
            }

            foreach (var mapping in methodMappings)
            {
                EmitHelper.StoreMethodToField(il, mapping.Method, mapping.MemberField);
                EmitHelper.StoreMethodToField(il, mapping.MethodBuilder, mapping.MethodField);
            }

            foreach (var mapping in eventMappings)
            {
                EmitHelper.StoreEventToField(il, mapping.Event, mapping.MemberField);
                EmitHelper.StoreMethodToField(il, mapping.AddMethodBuilder, mapping.AddMethodField);
                EmitHelper.StoreMethodToField(il, mapping.RemoveMethodBuilder, mapping.RemoveMethodField);
            }

            il.Emit(OpCodes.Ret);
        }

        private void CreateConstructor()
        {
            if (typeDefinition.TypeDefinitionType == TypeDefinitionType.InterfaceProxy)
                typeBuilder.AddConstructor(constructorFields);
            else
                typeBuilder.AddConstructor(constructorFields, (typeDefinition as ClassProxyDefinition).Constructor);
        }

        public bool HasImplementedInterface(Type interfaceType)
        {
            return typeBuilder.HasImplementedInterface(interfaceType);
        }

        public void AddInterfaceImplementation(Type interfaceType)
        {
            typeBuilder.AddInterfaceImplementation(interfaceType);
        }

        public FieldBuilder DefineField(string fieldName, Type fieldType, FieldAttributes attributes)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, fieldType, attributes);
            if (!attributes.HasFlag(FieldAttributes.Static))
                fieldBuilder.SetCustomAttribute(new CustomAttributeBuilder(Constructors.XmlIgnoreAttributeConstructor, new object[0]));
            fields.Add(fieldBuilder);
            return fieldBuilder;
        }

        public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, attributes, returnType, parameterTypes);
            properties.Add(propertyBuilder);
            return propertyBuilder;
        }

        public PropertyBuilder DefineFullProperty(string propertyName, PropertyAttributes propertyAttributes, Type returnType, Type[] parameterTypes, MethodAttributes methodAttributes, FieldBuilder field)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            PropertyBuilder propertyBuilder = DefineProperty(propertyName, propertyAttributes, returnType, parameterTypes);

            MethodBuilder getMethodBuilder = typeBuilder.DefineGetMethod( propertyName, methodAttributes, returnType, field);
            propertyBuilder.SetGetMethod(getMethodBuilder);
            methods.Add(getMethodBuilder);

            MethodBuilder setMethodBuilder = typeBuilder.DefineSetMethod(propertyName, methodAttributes, parameterTypes, field);
            propertyBuilder.SetSetMethod(setMethodBuilder);
            methods.Add(setMethodBuilder);

            return propertyBuilder;
        }

        public PropertyBuilder DefineReadOnlyProperty(string propertyName, PropertyAttributes propertyAttributes, Type returnType, Type[] parameterTypes, MethodAttributes methodAttributes, FieldBuilder field)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            PropertyBuilder propertyBuilder = DefineProperty(propertyName, propertyAttributes, returnType, parameterTypes);

            MethodBuilder getMethodBuilder = typeBuilder.DefineGetMethod(propertyName, methodAttributes, returnType, field);
            propertyBuilder.SetGetMethod(getMethodBuilder);
            methods.Add(getMethodBuilder);

            return propertyBuilder;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attributes)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(name, attributes);
            methods.Add(methodBuilder);
            return methodBuilder;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(name, attributes, returnType, parameterTypes);
            methods.Add(methodBuilder);
            return methodBuilder;
        }

        public void DefineMethodOverride(MethodBuilder methodBuilder, MethodInfo method)
        {
            typeBuilder.DefineMethodOverride(methodBuilder, method);
        }

        public EventBuilder DefineEvent(string name, EventAttributes attributes, Type eventType)
        {
            EventBuilder eventBuilder = typeBuilder.DefineEvent(name, attributes, eventType);
            events.Add(eventBuilder);
            return eventBuilder;
        }

        public EventBuilder DefineFullEvent(string eventName, EventAttributes eventAttributes, Type eventType, FieldBuilder field)
        {
            if (eventName is null)
                throw new ArgumentNullException(nameof(eventName));
            if (eventType is null)
                throw new ArgumentNullException(nameof(eventType));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            EventBuilder eventBuilder = DefineEvent(eventName, eventAttributes, eventType);

            MethodBuilder addMethodBuilder = typeBuilder.DefineAddMethod(eventName, eventType, field);
            eventBuilder.SetAddOnMethod(addMethodBuilder);
            methods.Add(addMethodBuilder);

            MethodBuilder removeMethodBuilder = typeBuilder.DefineRemoveMethod(eventName, eventType, field);
            eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
            methods.Add(removeMethodBuilder);

            return eventBuilder;
        }

        public PropertyBuilder CreateInterceptableProperty(PropertyDefinition propertyDefinition)
        {
            if (propertyDefinition is null)
                throw new ArgumentNullException(nameof(propertyDefinition));

            var attributes = StaticReadOnlyFieldAttributes;
            FieldBuilder memberField = DefineField(propertyDefinition.MemberFieldName, typeof(PropertyInfo), attributes);
            FieldBuilder getMethodField = null;
            FieldBuilder setMethodField = null;

            if (propertyDefinition.GetMethodDefinition != null)
                getMethodField = DefineField(propertyDefinition.GetMethodDefinition.CallerMethodFieldName, typeof(MethodInfo), attributes);
            if (propertyDefinition.SetMethodDefinition != null)
                setMethodField = DefineField(propertyDefinition.SetMethodDefinition.CallerMethodFieldName, typeof(MethodInfo), attributes);

            Type returnType = propertyDefinition.PropertyType;
            PropertyBuilder propertyBuilder = DefineProperty(propertyDefinition.Name, propertyDefinition.Attributes, returnType, new Type[] { returnType });
            MethodBuilder getMethodBuilder = null;
            MethodBuilder setMethodBuilder = null;
            var getMethodDefinition = propertyDefinition.GetMethodDefinition;
            if (getMethodDefinition != null)
            {
                getMethodBuilder = interceptableMethodBuilder.CreateMethod(this, getMethodDefinition, propertyDefinition.Property, memberField, getMethodField);
                AttributeHelper.AddInterceptorAttributes(getMethodBuilder, propertyDefinition.GettterInterceptorAttributes);
                propertyBuilder.SetGetMethod(getMethodBuilder);
            }

            var setMethodDefinition = propertyDefinition.SetMethodDefinition;
            if (setMethodDefinition != null)
            {
                setMethodBuilder = interceptableMethodBuilder.CreateMethod(this, setMethodDefinition, propertyDefinition.Property, memberField, setMethodField);
                AttributeHelper.AddInterceptorAttributes(setMethodBuilder, propertyDefinition.SetterInterceptorAttributes);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            propertyMappings.Add(new PropertyMapping(propertyDefinition, memberField, getMethodField, setMethodField, propertyBuilder, getMethodBuilder, setMethodBuilder));

            return propertyBuilder;
        }

        public MethodBuilder CreateInterceptableMethod(MethodDefinition methodDefinition, MemberInfo member)
        {
            if (methodDefinition is null)
                throw new ArgumentNullException(nameof(methodDefinition));
            if (member is null)
                throw new ArgumentNullException(nameof(member));

            var attributes = StaticReadOnlyFieldAttributes;
            FieldBuilder memberField = DefineField(methodDefinition.MemberFieldName, typeof(MethodInfo), attributes);
            FieldBuilder methodField = DefineField(methodDefinition.CallerMethodFieldName, typeof(MethodInfo), attributes);

            MethodBuilder methodBuilder = interceptableMethodBuilder.CreateMethod(this, methodDefinition, member, memberField, methodField);
            methodMappings.Add(new MethodMapping(methodDefinition, memberField, methodField, methodBuilder));
            return methodBuilder;
        }

        public EventBuilder CreateInterceptableEvent(EventDefinition eventDefinition)
        {
            if (eventDefinition is null)
                throw new ArgumentNullException(nameof(eventDefinition));

            var attributes = StaticReadOnlyFieldAttributes;
            FieldBuilder memberField = DefineField(eventDefinition.MemberFieldName, typeof(EventInfo), attributes);
            FieldBuilder addMethodField = DefineField(eventDefinition.AddMethodDefinition.CallerMethodFieldName, typeof(MethodInfo), attributes);
            FieldBuilder removeMethodField = DefineField(eventDefinition.RemoveMethodDefinition.CallerMethodFieldName, typeof(MethodInfo), attributes);

            EventBuilder eventBuilder = DefineEvent(eventDefinition.Name, eventDefinition.Attributes, eventDefinition.EventHandlerType);

            var addMethodDefinition = eventDefinition.AddMethodDefinition;
            MethodBuilder addMethodBuilder = interceptableMethodBuilder.CreateMethod(this, addMethodDefinition, eventDefinition.Event, memberField, addMethodField);
            AttributeHelper.AddInterceptorAttributes(addMethodBuilder, eventDefinition.AddOnInterceptorAttributes);
            eventBuilder.SetAddOnMethod(addMethodBuilder);

            var removeMethodDefinition = eventDefinition.RemoveMethodDefinition;
            MethodBuilder removeMethodBuilder = interceptableMethodBuilder.CreateMethod(this, removeMethodDefinition, eventDefinition.Event, memberField, removeMethodField);
            AttributeHelper.AddInterceptorAttributes(removeMethodBuilder, eventDefinition.RemoveOnInterceptorAttributes);
            eventBuilder.SetRemoveOnMethod(removeMethodBuilder);

            eventMappings.Add(new EventMapping(eventDefinition, eventBuilder, memberField, addMethodField, removeMethodField, addMethodBuilder, removeMethodBuilder));

            return eventBuilder;
        }
    }
}
