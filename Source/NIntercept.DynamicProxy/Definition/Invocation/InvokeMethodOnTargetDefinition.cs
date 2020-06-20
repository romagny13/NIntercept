using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class InvokeMethodOnTargetDefinition
    {
        private MethodInfo method;
        private ProxyParameterDefinition[] parameterDefinitions;

        public InvokeMethodOnTargetDefinition(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            this.method = method;
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public string Name
        {
            get { return "InvokeMethodOnTarget"; }
        }

        public MethodAttributes Attributes
        {
            get { return MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual; }
        }

        public Type ReturnType
        {
            get { return method.ReturnType; }
        }


        public ProxyParameterDefinition[] ParameterDefinitions
        {
            get
            {
                if (parameterDefinitions == null)
                    parameterDefinitions = GetParameterDefinitions();
                return parameterDefinitions;
            }
        }

        protected virtual ProxyParameterDefinition[] GetParameterDefinitions()
        {
            ParameterInfo[] parameters = Method.GetParameters();
            int length = parameters.Length;
            ProxyParameterDefinition[] parameterDefinitions = new ProxyParameterDefinition[length];
            for (int i = 0; i < length; i++)
                parameterDefinitions[i] = new ProxyParameterDefinition(parameters[i], i);
            return parameterDefinitions;
        }
    }
}
