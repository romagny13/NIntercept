using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyScope
    {
        private const string IntercetorsFieldName = "_interceptors";
        private ModuleScope moduleScope;
        private TypeBuilder typeBuilder;
        private ProxyTypeDefinition typeDefinition;
        private List<FieldBuilder> fieldBuilders;
        private List<PropertyBuilder> propertyBuilders;
        private List<MethodBuilder> methodBuilders;
        private List<EventBuilder> eventBuilders;
        private IProxyServiceProvider serviceProvider;

        public ProxyScope(ModuleScope moduleScope, TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {
            if (moduleScope is null)
                throw new ArgumentNullException(nameof(moduleScope));
            if (typeBuilder is null)
                throw new ArgumentNullException(nameof(typeBuilder));
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));

            this.fieldBuilders = new List<FieldBuilder>();
            this.propertyBuilders = new List<PropertyBuilder>();
            this.methodBuilders = new List<MethodBuilder>();
            this.eventBuilders = new List<EventBuilder>();

            this.moduleScope = moduleScope;
            this.typeBuilder = typeBuilder;
            this.typeDefinition = typeDefinition;

            serviceProvider = typeDefinition.Options?.ServiceProvider;
            if (serviceProvider == null)
                serviceProvider = new DefaultServiceProvider();
        }

        public ModuleScope ModuleScope
        {
            get { return moduleScope; }
        }

        public TypeBuilder TypeBuilder
        {
            get { return typeBuilder; }
        }

        public ProxyTypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public IReadOnlyList<FieldBuilder> FieldBuilders
        {
            get { return fieldBuilders; }
        }

        public IReadOnlyList<PropertyBuilder> PropertyBuilders
        {
            get { return propertyBuilders; }
        }

        public IReadOnlyList<MethodBuilder> MethodBuilders
        {
            get { return methodBuilders; }
        }

        public IReadOnlyList<EventBuilder> EventBuilders
        {
            get { return eventBuilders; }
        }

        internal void DefineTypeAndMembers()
        {
            if (typeDefinition.TypeDefinitionType == TypeDefinitionType.ClassProxy)
                typeBuilder.SetParent(typeDefinition.Type);

            AddCustomAttributes();
            AddInterfaces();

            CreateFields();

            var codeGenerator = typeDefinition.Options?.AdditionalCode;
            if (codeGenerator != null)
                codeGenerator.BeforeDefine(this);

            CreateConstructor();
            CreateProperties();
            CreateMethods();
            CreateEvents();
            DefineMixins();

            if (codeGenerator != null)
                codeGenerator.AfterDefine(this);
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
            CreateField(IntercetorsFieldName, typeof(IInterceptor[]));

            if (typeDefinition.TargetType != null)
                CreateField(typeDefinition.TargetFieldName, typeDefinition.TargetType);

            foreach (var mixinDefinition in typeDefinition.MixinDefinitions)
                CreateField(mixinDefinition.TargetFieldName, mixinDefinition.Type);
        }

        private void CreateConstructor()
        {
            if (typeDefinition.IsInterface)
                typeBuilder.AddConstructor(fieldBuilders.ToArray());
            else
                typeBuilder.AddConstructor(fieldBuilders.ToArray(), (typeDefinition as ClassProxyDefinition).Constructor);
        }

        private void CreateProperties()
        {
            foreach (var propertyDefinition in typeDefinition.PropertyDefinitions)
                CreateProperty(propertyDefinition);

            InterfaceProxyDefinition interfaceDefinition = typeDefinition as InterfaceProxyDefinition;
            if (interfaceDefinition != null)
            {
                foreach (var typeDefinitionToImplement in interfaceDefinition.InterfacesToImplement)
                {
                    foreach (var propertyDefinition in typeDefinitionToImplement.PropertyDefinitions)
                        CreateProperty(propertyDefinition);
                }
            }
        }

        private void CreateMethods()
        {
            foreach (var methodDefinition in typeDefinition.MethodDefinitions)
                CreateMethod(methodDefinition, methodDefinition.Method);

            InterfaceProxyDefinition interfaceDefinition = typeDefinition as InterfaceProxyDefinition;
            if (interfaceDefinition != null)
            {
                foreach (var typeDefinitionToImplement in interfaceDefinition.InterfacesToImplement)
                {
                    foreach (var methodDefinition in typeDefinitionToImplement.MethodDefinitions)
                        CreateMethod(methodDefinition, methodDefinition.Method);
                }
            }
        }

        private void CreateEvents()
        {
            foreach (var eventDefinition in typeDefinition.EventDefinitions)
                CreateEvent(eventDefinition);

            InterfaceProxyDefinition interfaceDefinition = typeDefinition as InterfaceProxyDefinition;
            if (interfaceDefinition != null)
            {
                foreach (var typeDefinitionToImplement in interfaceDefinition.InterfacesToImplement)
                {
                    foreach (var eventDefinition in typeDefinitionToImplement.EventDefinitions)
                        CreateEvent(eventDefinition);
                }
            }
        }

        private void DefineMixins()
        {
            foreach (var mixinDefinition in typeDefinition.MixinDefinitions)
            {
                foreach (var typeDefinitionToImplement in mixinDefinition.InterfacesToImplement)
                {
                    foreach (var propertyDefinition in typeDefinitionToImplement.PropertyDefinitions)
                        CreateProperty(propertyDefinition);

                    foreach (var methodDefinition in typeDefinitionToImplement.MethodDefinitions)
                        CreateMethod(methodDefinition, methodDefinition.Method);

                    foreach (var eventDefinition in typeDefinitionToImplement.EventDefinitions)
                        CreateEvent(eventDefinition);
                }
            }
        }

        public FieldBuilder CreateField(string fieldName, Type fieldType, FieldAttributes fieldAttributes)
        {
            if (fieldName is null)
                throw new ArgumentNullException(nameof(fieldName));
            if (fieldType is null)
                throw new ArgumentNullException(nameof(fieldType));

            FieldBuilder field = typeBuilder.DefineField(fieldName, fieldType, fieldAttributes);
            field.SetCustomAttribute(new CustomAttributeBuilder(Constructors.XmlIgnoreAttributeConstructor, new object[0]));
            fieldBuilders.Add(field);
            return field;
        }

        public FieldBuilder CreateField(string fieldName, Type fieldType)
        {
            return CreateField(fieldName, fieldType, FieldAttributes.Private);
        }

        public PropertyBuilder CreateProperty(PropertyDefinition propertyDefinition)
        {
            if (propertyDefinition is null)
                throw new ArgumentNullException(nameof(propertyDefinition));

            PropertyBuilder propertyBuilder = serviceProvider.ProxyPropertyBuilder.CreateProperty(this, propertyDefinition);
            propertyBuilders.Add(propertyBuilder);
            return propertyBuilder;
        }

        public MethodBuilder CreateMethod(MethodDefinition methodDefinition, MemberInfo member)
        {
            if (methodDefinition is null)
                throw new ArgumentNullException(nameof(methodDefinition));

            MethodBuilder methodBuilder = serviceProvider.ProxyMethodBuilder.CreateMethod(this, methodDefinition, member);
            methodBuilders.Add(methodBuilder);
            return methodBuilder;
        }

        public EventBuilder CreateEvent(EventDefinition eventDefinition)
        {
            if (eventDefinition is null)
                throw new ArgumentNullException(nameof(eventDefinition));

            EventBuilder eventBuilder = serviceProvider.ProxyEventBuilder.CreateEvent(this, eventDefinition);
            eventBuilders.Add(eventBuilder);
            return eventBuilder;
        }

        public MethodBuilder CreateCallbackMethod(CallbackMethodDefinition callbackMethodDefinition)
        {
            return serviceProvider.CallbackMethodBuilder.CreateMethod(this, callbackMethodDefinition);
        }
    }
}
