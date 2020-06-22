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

        public ProxyBuilder(ModuleScope proxyModuleBuilder)
        {
            if (proxyModuleBuilder is null)
                throw new ArgumentNullException(nameof(proxyModuleBuilder));

            this.moduleScope = proxyModuleBuilder;
        }

        public ProxyBuilder()
            : this(new ModuleScope())
        { }

        public ModuleScope ModuleScope
        {
            get { return moduleScope; }
        }

        protected ModuleBuilder ModuleBuilder
        {
            get { return moduleScope.Module; }
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

        public virtual TypeBuilder CreateType(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors)
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

        protected virtual TypeBuilder DefineType(TypeDefinition typeDefinition)
        {
            TypeBuilder typeBuilder = ModuleBuilder.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            typeBuilder.AddCustomAttribute(typeof(SerializableAttribute));

            InterfaceProxyDefinition interfaceDefinition = typeDefinition as InterfaceProxyDefinition;
            if (interfaceDefinition != null)
            {
                typeBuilder.AddInterfaceImplementation(typeDefinition.Type);

                typeBuilder.AddInterfacesToImplement(interfaceDefinition.Interfaces);

                InterceptorAttributeHelper.AddInterceptorAttributes(typeBuilder, typeDefinition.InterceptorAttributes);
            }
            else
                typeBuilder.SetParent(typeDefinition.Type); // inherit attributes from parent

            return typeBuilder;
        }

        protected FieldBuilder[] DefineFields(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {
            FieldBuilder interceptorsField = typeBuilder.DefineField("_interceptors", typeof(IInterceptor[]), FieldAttributes.Private);
            if (typeDefinition.Target != null)
            {
                FieldBuilder targetField = typeBuilder.DefineField("_target", typeDefinition.TargetType, FieldAttributes.Private);
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
                fields[i] = typeBuilder.DefineField($"_{NamingHelper.ToCamelCase(mixinDefinition.Name)}", mixinDefinition.Type, FieldAttributes.Private);
            }
            return fields;
        }

        protected virtual void DefineConstructors(TypeBuilder typeBuilder, TypeDefinition typeDefinition, FieldBuilder[] fields)
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

        protected virtual void DefineProperties(TypeBuilder typeBuilder, TypeDefinition typeDefinition, FieldBuilder[] fields)
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

        protected virtual void DefineMethods(TypeBuilder typeBuilder, TypeDefinition typeDefinition, FieldBuilder[] fields)
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

        protected virtual void DefineEvents(TypeBuilder typeBuilder, TypeDefinition typeDefinition, FieldBuilder[] fields)
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

        protected virtual void DefineMixins(TypeBuilder typeBuilder, MixinDefinition[] mixinDefinitions, FieldBuilder[] mixinFields, FieldBuilder interceptorsField)
        {
            for (int i = 0; i < mixinFields.Length; i++)
            {
                MixinDefinition mixinDefinition = mixinDefinitions[i];

                typeBuilder.AddInterfacesToImplement(mixinDefinition.Interfaces);

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

        protected virtual void DefineProperty(TypeBuilder typeBuilder, PropertyDefinition propertyDefinition, FieldBuilder[] fields)
        {
            ProxyPropertyBuilder.CreateProperty(ModuleScope, typeBuilder, propertyDefinition, fields);
        }

        protected virtual void DefineMethod(TypeBuilder typeBuilder, MethodDefinition methodDefinition, FieldBuilder[] fields)
        {
            ProxyMethodBuilder.CreateMethod(ModuleScope, typeBuilder, methodDefinition, methodDefinition.Method, fields);
        }

        protected virtual void DefineEvent(TypeBuilder typeBuilder, EventDefinition eventDefinition, FieldBuilder[] fields)
        {
            ProxyEventBuilder.CreateEvent(ModuleScope, typeBuilder, eventDefinition, fields);
        }
    }

}
