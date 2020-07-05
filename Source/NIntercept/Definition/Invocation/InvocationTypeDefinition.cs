using NIntercept.Helpers;
using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class InvocationTypeDefinition
    {
        private const string InvocationNamespace = "NIntercept.Invocations";
        private TypeDefinition typeDefinition;
        private MethodDefinition methodDefinition;
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
        }

        public TypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public MethodDefinition MethodDefinition
        {
            get { return methodDefinition; }
        }

        public string Name
        {
            get
            {
                if (name == null)
                    name = GetInvocationTypeName();
                return name;
            }
        }

        public TypeAttributes Attributes
        {
            get { return TypeAttributes.Public; }
        }

        public string FullName
        {
            get { return $"{InvocationNamespace}.{Name}"; }
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

        private string GetInvocationTypeName()
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
