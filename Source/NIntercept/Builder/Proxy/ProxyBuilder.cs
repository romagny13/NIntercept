using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyBuilder : IProxyBuilder
    {
        private static readonly IProxyPropertyBuilder DefaultProxyPropertyBuilder;
        private static readonly IProxyMethodBuilder DefaultProxyMethodBuilder;
        private static readonly IProxyEventBuilder DefaultProxyEventBuilder;
        private ModuleScope moduleScope;
        private IProxyPropertyBuilder proxyPropertyBuilder;
        private IProxyMethodBuilder proxyMethodBuilder;
        private IProxyEventBuilder proxyEventBuilder;

        static ProxyBuilder()
        {
            DefaultProxyPropertyBuilder = new ProxyPropertyBuilder();
            DefaultProxyMethodBuilder = new ProxyMethodBuilder();
            DefaultProxyEventBuilder = new ProxyEventBuilder();
        }

        public ProxyBuilder(ModuleScope moduleScope)
        {
            if (moduleScope is null)
                throw new ArgumentNullException(nameof(moduleScope));

            this.moduleScope = moduleScope;
        }

        public ProxyBuilder()
            : this(new ModuleScope())
        { }

        public ModuleScope ModuleScope
        {
            get { return moduleScope; }
        }

        public virtual IProxyPropertyBuilder ProxyPropertyBuilder
        {
            get { return proxyPropertyBuilder ?? DefaultProxyPropertyBuilder; }
            set { proxyPropertyBuilder = value; }
        }

        public virtual IProxyMethodBuilder ProxyMethodBuilder
        {
            get { return proxyMethodBuilder ?? DefaultProxyMethodBuilder; }
            set { proxyMethodBuilder = value; }
        }

        public virtual IProxyEventBuilder ProxyEventBuilder
        {
            get { return proxyEventBuilder ?? DefaultProxyEventBuilder; }
            set { proxyEventBuilder = value; }
        }

        public TypeBuilder CreateType(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));

            TypeBuilder typeBuilder = DefineType(typeDefinition);

            FieldBuilder[] fields = DefineFields(typeBuilder, typeDefinition);
            FieldBuilder[] mixinFields = DefineMixinFields(typeBuilder, typeDefinition.MixinDefinitions);
            FieldBuilder[] allFields = mixinFields.Length > 0 ? fields.Concat(mixinFields).ToArray() : fields;

            DefineConstructors(typeBuilder, typeDefinition, allFields);
            DefineProperties(typeBuilder, typeDefinition, fields);
            DefineMethods(typeBuilder, typeDefinition, fields);
            DefineEvents(typeBuilder, typeDefinition, fields);
            DefineMixins(typeBuilder, typeDefinition.MixinDefinitions, mixinFields, fields[0]);

            return typeBuilder;
        }

        protected TypeBuilder DefineType(ProxyTypeDefinition typeDefinition)
        {
            TypeBuilder typeBuilder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);
            if (typeDefinition.TypeDefinitionType == TypeDefinitionType.ClassProxy)
                typeBuilder.SetParent(typeDefinition.Type); // inherit attributes from parent

            DefineTypeAttributes(typeBuilder, typeDefinition);
            DefineInterfaces(typeBuilder, typeDefinition);

            return typeBuilder;
        }

        protected void DefineTypeAttributes(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(Constructors.SerializableAttributeConstructor, new object[0]));
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(Constructors.XmlIncludeAttributeConstructor, new object[] { typeDefinition.Type }));

            var options = typeDefinition.Options;
            if(options != null)
            {
                foreach (var additionalTypeAttribute in options.AdditionalTypeAttributes)
                    typeBuilder.SetCustomAttribute(additionalTypeAttribute);
            }

            if (typeDefinition.TypeDefinitionType == TypeDefinitionType.InterfaceProxy)
            {
                AttributeHelper.AddInterceptorAttributes(typeBuilder, typeDefinition.InterceptorAttributes);
            }
        }

        protected void DefineInterfaces(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
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

        protected virtual FieldBuilder DefineField(TypeBuilder typeBuilder, string fieldName, Type fieldType)
        {
            FieldBuilder field = typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Private);
            field.SetCustomAttribute(new CustomAttributeBuilder(Constructors.XmlIgnoreAttributeConstructor, new object[0]));
            return field;
        }

        protected FieldBuilder[] DefineFields(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {
            FieldBuilder interceptorsField = DefineField(typeBuilder, "_interceptors", typeof(IInterceptor[]));
            if (typeDefinition.Target != null)
            {
                FieldBuilder targetField = DefineField(typeBuilder, "_target", typeDefinition.TargetType);
                return new FieldBuilder[] { interceptorsField, targetField };
            }
            return new FieldBuilder[] { interceptorsField };
        }

        protected FieldBuilder[] DefineMixinFields(TypeBuilder typeBuilder, MixinDefinition[] mixinDefinitions)
        {
            int length = mixinDefinitions.Length;
            var fields = new FieldBuilder[length];
            for (int i = 0; i < length; i++)
            {
                var mixinDefinition = mixinDefinitions[i];
                fields[i] = DefineField(typeBuilder, $"_{NamingHelper.ToCamelCase(mixinDefinition.Name)}", mixinDefinition.Type);
            }
            return fields;
        }

        protected void DefineConstructors(TypeBuilder typeBuilder, TypeDefinition typeDefinition, FieldBuilder[] fields)
        {
            if (typeDefinition.IsInterface)
                typeBuilder.AddConstructor(fields);
            else
            {
                ConstructorInfo baseCtor = typeDefinition.Type
                    .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (baseCtor == null || baseCtor.IsPrivate)
                    throw new NotSupportedException($"No parameterless constructor found for '{typeDefinition.Type.Name}'");

                typeBuilder.AddConstructor(fields, baseCtor);
            }
        }

        protected void DefineProperties(TypeBuilder typeBuilder, TypeDefinition typeDefinition, FieldBuilder[] fields)
        {
            foreach (var propertyDefinition in typeDefinition.PropertyDefinitions)
                DefineProperty(typeBuilder, propertyDefinition, fields);

            InterfaceProxyDefinition interfaceDefinition = typeDefinition as InterfaceProxyDefinition;
            if (interfaceDefinition != null)
            {
                foreach (var typeDefinitionToImplement in interfaceDefinition.InterfacesToImplement)
                {
                    foreach (var propertyDefinition in typeDefinitionToImplement.PropertyDefinitions)
                        DefineProperty(typeBuilder, propertyDefinition, fields);
                }
            }
        }

        protected void DefineMethods(TypeBuilder typeBuilder, TypeDefinition typeDefinition, FieldBuilder[] fields)
        {
            foreach (var methodDefinition in typeDefinition.MethodDefinitions)
                DefineMethod(typeBuilder, methodDefinition, fields);

            InterfaceProxyDefinition interfaceDefinition = typeDefinition as InterfaceProxyDefinition;
            if (interfaceDefinition != null)
            {
                foreach (var typeDefinitionToImplement in interfaceDefinition.InterfacesToImplement)
                {
                    foreach (var methodDefinition in typeDefinitionToImplement.MethodDefinitions)
                        DefineMethod(typeBuilder, methodDefinition, fields);
                }
            }
        }

        protected void DefineEvents(TypeBuilder typeBuilder, TypeDefinition typeDefinition, FieldBuilder[] fields)
        {
            foreach (var eventDefinition in typeDefinition.EventDefinitions)
                DefineEvent(typeBuilder, eventDefinition, fields);

            InterfaceProxyDefinition interfaceDefinition = typeDefinition as InterfaceProxyDefinition;
            if (interfaceDefinition != null)
            {
                foreach (var typeDefinitionToImplement in interfaceDefinition.InterfacesToImplement)
                {
                    foreach (var eventDefinition in typeDefinitionToImplement.EventDefinitions)
                        DefineEvent(typeBuilder, eventDefinition, fields);
                }
            }
        }

        protected void DefineMixins(TypeBuilder typeBuilder, MixinDefinition[] mixinDefinitions, FieldBuilder[] mixinFields, FieldBuilder interceptorsField)
        {
            for (int i = 0; i < mixinFields.Length; i++)
            {
                MixinDefinition mixinDefinition = mixinDefinitions[i];
                FieldBuilder mixinField = mixinFields[i];
                FieldBuilder[] fields = new FieldBuilder[] { interceptorsField, mixinField };

                foreach (var typeDefinitionToImplement in mixinDefinition.InterfacesToImplement)
                {
                    foreach (var propertyDefinition in typeDefinitionToImplement.PropertyDefinitions)
                        DefineProperty(typeBuilder, propertyDefinition, fields);

                    foreach (var methodDefinition in typeDefinitionToImplement.MethodDefinitions)
                        DefineMethod(typeBuilder, methodDefinition, fields);

                    foreach (var eventDefinition in typeDefinitionToImplement.EventDefinitions)
                        DefineEvent(typeBuilder, eventDefinition, fields);
                }
            }
        }

        protected void DefineProperty(TypeBuilder typeBuilder, PropertyDefinition propertyDefinition, FieldBuilder[] fields)
        {
            ProxyPropertyBuilder.CreateProperty(ModuleScope, typeBuilder, propertyDefinition, fields);
        }

        protected void DefineMethod(TypeBuilder typeBuilder, MethodDefinition methodDefinition, FieldBuilder[] fields)
        {
            ProxyMethodBuilder.CreateMethod(ModuleScope, typeBuilder, methodDefinition, methodDefinition.Method, fields);
        }

        protected void DefineEvent(TypeBuilder typeBuilder, EventDefinition eventDefinition, FieldBuilder[] fields)
        {
            ProxyEventBuilder.CreateEvent(ModuleScope, typeBuilder, eventDefinition, fields);
        }
    }
}
