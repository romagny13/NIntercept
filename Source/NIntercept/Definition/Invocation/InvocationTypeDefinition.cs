using NIntercept.Helpers;
using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class InvocationTypeDefinition
    {
        private const string DefaultInvocationsNamespace = "NIntercept.Invocations";
        private TypeDefinition typeDefinition;
        private MethodDefinition methodDefinition;
        private string @namespace;
        private string name;
        private InvokeMethodOnTargetDefinition methodOnTargetDefinition;

        public InvocationTypeDefinition(TypeDefinition typeDefinition, MethodDefinition methodDefinition)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (methodDefinition is null)
                throw new ArgumentNullException(nameof(methodDefinition));
            this.typeDefinition = typeDefinition;
            this.methodDefinition = methodDefinition;
            this.@namespace = DefaultInvocationsNamespace;
        }

        public TypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public MethodDefinition MethodDefinition
        {
            get { return methodDefinition; }
        }

        public object Target
        {
            get { return typeDefinition.Target; }
        }

        public virtual string Name
        {
            get
            {
                if (name == null)
                    name = GetInvocationTypeName();
                return name;
            }
        }

        public virtual TypeAttributes Attributes
        {
            get { return TypeAttributes.Public; }
        }

        public string Namespace
        {
            get { return @namespace; }
            set { @namespace = value; }
        }

        public virtual string FullName
        {
            get { return !string.IsNullOrWhiteSpace(Namespace) ? $"{Namespace}.{Name}" : Name; }
        }

        public Type[] GenericArguments
        {
            get { return methodDefinition.GenericArguments; }
        }

        public InvokeMethodOnTargetDefinition MethodOnTargetDefinition
        {
            get
            {
                if (methodOnTargetDefinition == null)
                    methodOnTargetDefinition = new InvokeMethodOnTargetDefinition(methodDefinition.Method);
                return methodOnTargetDefinition;
            }
        }

        protected virtual string GetInvocationTypeName()
        {
            MethodInfo method = methodDefinition.Method;
            TypeDefinition typeDefinition = methodDefinition.TypeDefinition;
            string proxyPartName;
            if (typeDefinition.TypeDefinitionType == TypeDefinitionType.Mixin)
                proxyPartName = ((MixinDefinition)typeDefinition).ProxyTypeDefinition.Name;
            else
                proxyPartName = typeDefinition.Name;

            if (method.DeclaringType != methodDefinition.TypeDefinition.Type)
                return $"{proxyPartName}_{method.DeclaringType.Name}_{NamingHelper.GetUniqueName(method)}_Invocation";
            else
                return $"{proxyPartName}_{NamingHelper.GetUniqueName(method)}_Invocation";
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
