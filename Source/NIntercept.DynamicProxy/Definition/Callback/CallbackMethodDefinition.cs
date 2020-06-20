using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class CallbackMethodDefinition
    {
        private string name;
        private ProxyMethodDefinition methodDefinition;
        private ProxyTypeDefinition typeDefinition;

        public CallbackMethodDefinition(ProxyTypeDefinition typeDefinition, ProxyMethodDefinition methodDefinition)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (methodDefinition is null)
                throw new ArgumentNullException(nameof(methodDefinition));

            this.typeDefinition = typeDefinition;
            this.methodDefinition = methodDefinition;
        }

        public ProxyTypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
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
                    name = $"{MethodDefinition.Name}_Callback";
                return name;
            }
        }

        public virtual MethodAttributes MethodAttributes
        {
            get { return MethodAttributes.Public | MethodAttributes.HideBySig; }
        }

        public Type ReturnType
        {
            get { return methodDefinition.ReturnType; }
        }

        public Type[] GenericArguments
        {
            get { return methodDefinition.GenericArguments; }
        }

        public ProxyParameterDefinition[] ParameterDefinitions
        {
            get { return methodDefinition.ParameterDefinitions; }
        }

        public object Target
        {
            get { return typeDefinition.Target; }
        }

        public MethodInfo Method
        {
            get { return methodDefinition.Method; }
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
