using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class InvocationTypeDefinition
    {
        private const string DefaultInvocationsNamespace = "Interception.Invocations";
        private ProxyMethodDefinition methodDefinition;
        private string @namespace;
        private string name;
        private InvokeMethodOnTargetDefinition methodOnTargetDefinition;

        public InvocationTypeDefinition(ProxyMethodDefinition methodDefinition)
        {
            if (methodDefinition is null)
                throw new ArgumentNullException(nameof(methodDefinition));
            this.methodDefinition = methodDefinition;
            this.@namespace = DefaultInvocationsNamespace;
        }

        public ProxyMethodDefinition MethodDefinition
        {
            get { return methodDefinition; }
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

        public object Target
        {
            get { return methodDefinition.Target; }
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
            if (method.DeclaringType != methodDefinition.TypeDefinition.Type)
                return $"{methodDefinition.TypeDefinition.Name}_{method.DeclaringType.Name}_{NamingHelper.GetUniqueName(method)}_Invocation";
            else
                return $"{methodDefinition.TypeDefinition.Name}_{NamingHelper.GetUniqueName(method)}_Invocation";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
