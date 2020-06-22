using NIntercept.Helpers;
using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class MethodDefinition
    {
        private TypeDefinition typeDefinition;
        private MethodInfo method;
        private string name;
        private InterceptorAttributeDefinition[] interceptorAttributes;
        private Type[] genericArguments;
        private ParameterDefinition[] parameterDefinitions;
        private CallbackMethodDefinition methodCallbackDefinition;
        private InvocationTypeDefinition invocationTypeDefinition;

        public MethodDefinition(TypeDefinition typeDefinition, MethodInfo method)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            this.typeDefinition = typeDefinition;
            this.method = method;
        }

        public TypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public virtual string Name
        {
            get
            {
                if (name == null)
                    name = Method.Name;
                return name;
            }
        }

        public Type ReturnType
        {
            get { return method.ReturnType; }
        }

        public Type[] GenericArguments
        {
            get
            {
                if (genericArguments == null)
                    genericArguments = method.GetGenericArguments();
                return genericArguments;
            }
        }

        public ParameterDefinition[] ParameterDefinitions
        {
            get
            {
                if (parameterDefinitions == null)
                    parameterDefinitions = GetParameterDefinitions();
                return parameterDefinitions;
            }
        }

        public virtual MethodAttributes Attributes
        {
            get { return method.Attributes; }
        }

        protected virtual ParameterDefinition[] GetParameterDefinitions()
        {
            ParameterInfo[] parameters = Method.GetParameters();
            int length = parameters.Length;
            ParameterDefinition[] parameterDefinitions = new ParameterDefinition[length];
            for (int i = 0; i < length; i++)
                parameterDefinitions[i] = new ParameterDefinition(parameters[i], i);
            return parameterDefinitions;
        }

        public InterceptorAttributeDefinition[] InterceptorAttributes
        {
            get
            {
                if (interceptorAttributes == null)
                    interceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(method, typeof(IMethodInterceptorProvider));
                return interceptorAttributes;
            }
        }

        public virtual CallbackMethodDefinition MethodCallbackDefinition
        {
            get
            {
                if (methodCallbackDefinition == null)
                    methodCallbackDefinition = new CallbackMethodDefinition(typeDefinition, this);
                return methodCallbackDefinition;
            }
        }

        public virtual InvocationTypeDefinition InvocationTypeDefinition
        {
            get
            {
                if (invocationTypeDefinition == null)
                    invocationTypeDefinition = new InvocationTypeDefinition(typeDefinition, this);
                return invocationTypeDefinition;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
