using NIntercept.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyBuilder : IProxyBuilder
    {
        private static readonly IProxyPropertyBuilder DefaultProxyPropertyBuilder;
        private static readonly IProxyMethodBuilder DefaultProxyMethodBuilder;
        private static readonly IProxyEventBuilder DefaultProxyEventBuilder;
        private IProxyModuleBuilder proxyModuleBuilder;
        private IProxyPropertyBuilder proxyPropertyBuilder;
        private IProxyMethodBuilder proxyMethodBuilder;
        private IProxyEventBuilder proxyEventBuilder;

        static ProxyBuilder()
        {
            DefaultProxyPropertyBuilder = new ProxyPropertyBuilder();
            DefaultProxyMethodBuilder = new ProxyMethodBuilder();
            DefaultProxyEventBuilder = new ProxyEventBuilder();
        }

        public ProxyBuilder(IProxyModuleBuilder proxyModuleBuilder)
        {
            if (proxyModuleBuilder is null)
                throw new ArgumentNullException(nameof(proxyModuleBuilder));

            this.proxyModuleBuilder = proxyModuleBuilder;
        }

        public ProxyBuilder()
            : this(new ProxyModuleBuilder())
        { }

        public IProxyModuleBuilder ProxyModuleBuilder
        {
            get { return proxyModuleBuilder; }
        }

        protected ModuleBuilder ModuleBuilder
        {
            get { return proxyModuleBuilder.Module; }
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
            DefineConstructors(typeBuilder, typeDefinition, fields);
            DefineProperties(typeBuilder, typeDefinition, fields);
            DefineMethods(typeBuilder, typeDefinition, fields);
            DefineEvents(typeBuilder, typeDefinition, fields);

            return typeBuilder;
        }

        protected virtual TypeBuilder DefineType(ProxyTypeDefinition typeDefinition)
        {
            TypeBuilder typeBuilder = ModuleBuilder.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            TypeBuilderHelper.AddCustomAttribute(typeBuilder, typeof(SerializableAttribute));

            if (typeDefinition.IsInterface)
            {
                typeBuilder.AddInterfaceImplementation(typeDefinition.Type);

                AddInterfacesToImplement(typeDefinition, typeBuilder);

                TypeBuilderHelper.AddInterceptionAttributes(typeBuilder, typeDefinition.InterceptorAttributes);
            }
            else
                typeBuilder.SetParent(typeDefinition.Type); // inherit attributes from parent

            return typeBuilder;
        }

        protected void AddInterfacesToImplement(ProxyTypeDefinition typeDefinition, TypeBuilder typeBuilder)
        {
            if (typeDefinition.IsInterface)
            {
                foreach (var @interface in typeDefinition.Interfaces)
                    typeBuilder.AddInterfaceImplementation(@interface);
            }
        }

        protected virtual FieldBuilder[] DefineFields(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {
            FieldBuilder interceptorsField = typeBuilder.DefineField("_interceptors", typeof(IInterceptor[]), FieldAttributes.Private);
            if (typeDefinition.Target != null)
            {
                FieldBuilder targetField = typeBuilder.DefineField("_target", typeDefinition.TargetType, FieldAttributes.Private);
                return new FieldBuilder[] { interceptorsField, targetField };
            }
            return new FieldBuilder[] { interceptorsField };
        }

        protected virtual void DefineConstructors(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition, FieldBuilder[] fields)
        {
            if (typeDefinition.IsInterface)
                TypeBuilderHelper.AddConstructor(typeBuilder, fields);
            else
            {
                ConstructorInfo baseCtor = typeDefinition.Type
                    .GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (baseCtor == null || baseCtor.IsPrivate)
                    throw new NotSupportedException($"No parameterless constructor found for '{typeDefinition.Type.Name}'");

                TypeBuilderHelper.AddConstructor(typeBuilder, fields, baseCtor);
            }
        }

        protected virtual void DefineProperties(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition, FieldBuilder[] fields)
        {
            foreach (var propertyDefinition in typeDefinition.PropertyDefinitions)
                DefineProperty(typeBuilder, propertyDefinition, fields);

            if (typeDefinition.IsInterface)
            {
                foreach (var typeDefinitionToImplement in typeDefinition.TypesToImplement)
                {
                    foreach (var propertyDefinition in typeDefinitionToImplement.PropertyDefinitions)
                        DefineProperty(typeBuilder, propertyDefinition, fields);
                }
            }
        }

        protected virtual void DefineMethods(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition, FieldBuilder[] fields)
        {
            foreach (var methodDefinition in typeDefinition.MethodDefinitions)
                DefineMethod(typeBuilder, methodDefinition, fields);

            if (typeDefinition.IsInterface)
            {
                foreach (var typeDefinitionToImplement in typeDefinition.TypesToImplement)
                {
                    foreach (var methodDefinition in typeDefinitionToImplement.MethodDefinitions)
                        DefineMethod(typeBuilder, methodDefinition, fields);
                }
            }
        }

        protected virtual void DefineEvents(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition, FieldBuilder[] fields)
        {
            foreach (var eventDefinition in typeDefinition.EventDefinitions)
                DefineEvent(typeBuilder, eventDefinition, fields);

            if (typeDefinition.IsInterface)
            {
                foreach (var typeDefinitionToImplement in typeDefinition.TypesToImplement)
                {
                    foreach (var eventDefinition in typeDefinitionToImplement.EventDefinitions)
                        DefineEvent(typeBuilder, eventDefinition, fields);
                }
            }
        }

        protected virtual void DefineProperty(TypeBuilder typeBuilder, ProxyPropertyDefinition propertyDefinition, FieldBuilder[] fields)
        {
            ProxyPropertyBuilder.CreateProperty(ModuleBuilder, typeBuilder, propertyDefinition, fields);
        }

        protected virtual void DefineMethod(TypeBuilder typeBuilder, ProxyMethodDefinition methodDefinition, FieldBuilder[] fields)
        {
            ProxyMethodBuilder.CreateMethod(ModuleBuilder, typeBuilder, methodDefinition, methodDefinition.Method, fields);
        }

        protected virtual void DefineEvent(TypeBuilder typeBuilder, ProxyEventDefinition eventDefinition, FieldBuilder[] fields)
        {
            ProxyEventBuilder.CreateEvent(ModuleBuilder, typeBuilder, eventDefinition, fields);
        }
    }

}
