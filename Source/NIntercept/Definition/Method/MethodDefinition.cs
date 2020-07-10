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
        private MethodAttributes? methodAttributes;
        private InterceptorAttributeDefinition[] interceptorAttributes;
        private Type[] genericArguments;
        private ParameterDefinition[] parameterDefinitions;
        private CallbackMethodDefinition callbackMethodDefinition;
        private InvocationTypeDefinition invocationTypeDefinition;
        private string callerMethodFieldName;
        private string memberFieldName;
        private string uniqueMethodName;
        private string interceptorSelectorFieldName;

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

        public virtual MethodDefinitionType MethodDefinitionType
        {
            get { return MethodDefinitionType.Method; }
        }

        public virtual MethodAttributes MethodAttributes
        {
            get
            {
                if (methodAttributes == null)
                {
                    if (method.DeclaringType.IsInterface)
                        methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
                    else
                        methodAttributes = MethodAttributesHelper.GetMethodAttributes(method);
                }
                return methodAttributes.Value;
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

        public virtual CallbackMethodDefinition CallbackMethodDefinition
        {
            get
            {
                if (callbackMethodDefinition == null)
                    callbackMethodDefinition = new CallbackMethodDefinition(typeDefinition, this);
                return callbackMethodDefinition;
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

        public string UniqueMethodName
        {
            get
            {
                if (uniqueMethodName == null)
                    uniqueMethodName = MethodDefinitionType == MethodDefinitionType.Method ? NamingHelper.GetUniqueName(method) : Name;
                return uniqueMethodName;
            }
        }

        public string InterceptorSelectorFieldName
        {
            get
            {
                if (interceptorSelectorFieldName == null)
                    interceptorSelectorFieldName = $"__interceptors_{UniqueMethodName}";
                return interceptorSelectorFieldName;
            }
        }

        public string CallerMethodFieldName
        {
            get
            {
                if (callerMethodFieldName == null)
                {
                    // MyClass_Proxy_MyMethod
                    if (typeDefinition.TypeDefinitionType == TypeDefinitionType.Mixin)
                        callerMethodFieldName = $"{((MixinDefinition)typeDefinition).ProxyTypeDefinition.Name}_{UniqueMethodName}";
                    else
                        callerMethodFieldName = $"{typeDefinition.Name}_{UniqueMethodName}";
                }
                return callerMethodFieldName;
            }
        }

        public virtual string MemberFieldName
        {
            get
            {
                // MyClass_MyMethod
                if (memberFieldName == null)
                    memberFieldName = $"{typeDefinition.Type.Name}_{NamingHelper.GetUniqueName(method)}";
                return memberFieldName;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
