using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyGeneratorOptions
    {
        private List<object> mixinInstances;
        private IClassProxyMemberSelector classProxyMemberSelector;
        private List<CustomAttributeBuilder> additionalTypeAttributes;
        private IConstructorSelector constructorSelector;
        private IConstructorInjectionResolver constructorInjectionResolver;
        private CodeGenerator codeGenerator;

        public ProxyGeneratorOptions()
        {
            mixinInstances = new List<object>();
            additionalTypeAttributes = new List<CustomAttributeBuilder>();
        }

        public IConstructorSelector ConstructorSelector
        {
            get { return constructorSelector; }
            set { constructorSelector = value; }
        }

        public IConstructorInjectionResolver ConstructorInjectionResolver
        {
            get { return constructorInjectionResolver ; }
            set { constructorInjectionResolver = value; }
        }

        public CodeGenerator CodeGenerator
        {
            get { return codeGenerator; }
            set { codeGenerator = value; }
        }

        protected internal List<object> MixinInstances
        {
            get { return mixinInstances; }
        }

        public IClassProxyMemberSelector ClassProxyMemberSelector
        {
            get { return classProxyMemberSelector; }
            set { classProxyMemberSelector = value; }
        }

        public List<CustomAttributeBuilder> AdditionalTypeAttributes
        {
            get { return additionalTypeAttributes; }
        }

        public void AddMixinInstance(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            this.mixinInstances.Add(instance);
        }
    }

    public class CodeGenerator
    {
        public virtual void Define(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {

        }

        public virtual void BeforeInvoke(ILGenerator il, TypeBuilder typeBuilder, CallbackMethodDefinition callbackMethodDefinition, FieldBuilder[] fields)
        {

        }

        public virtual void AfterInvoke(ILGenerator il, TypeBuilder typeBuilder, CallbackMethodDefinition callbackMethodDefinition, FieldBuilder[] fields)
        {

        }
    }
}
