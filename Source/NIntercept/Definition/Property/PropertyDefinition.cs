using NIntercept.Helpers;
using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class PropertyDefinition
    {
        private TypeDefinition typeDefinition;
        private PropertyInfo property;
        private ParameterInfo[] indexParameters;
        private GetMethodDefinition getMethodDefinition;
        private SetMethodDefinition setMethodDefinition;
        private InterceptorAttributeDefinition[] getterInterceptorAttributes;
        private InterceptorAttributeDefinition[] setterInterceptorAttributes;

        public PropertyDefinition(TypeDefinition typeDefinition, PropertyInfo property)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            this.typeDefinition = typeDefinition;
            this.property = property;
        }

        public TypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public string Name
        {
            get { return property.Name; }
        }

        public Type PropertyType
        {
            get { return property.PropertyType; }
        }

        public PropertyAttributes Attributes
        {
            get { return property.Attributes; }
        }

        public ParameterInfo[] IndexParameters
        {
            get
            {
                if (indexParameters == null)
                    indexParameters = property.GetIndexParameters();
                return indexParameters;
            }
        }

        public GetMethodDefinition GetMethodDefinition
        {
            get
            {
                if (property.CanRead)
                {
                    if (getMethodDefinition == null)
                        getMethodDefinition = new GetMethodDefinition(TypeDefinition, this, property.GetMethod);

                }
                return getMethodDefinition;
            }
        }

        public SetMethodDefinition SetMethodDefinition
        {
            get
            {
                if (property.CanWrite)
                {
                    if (setMethodDefinition == null)
                        setMethodDefinition = new SetMethodDefinition(TypeDefinition, this, property.SetMethod);
                }
                return setMethodDefinition;
            }
        }


        public InterceptorAttributeDefinition[] GettterInterceptorAttributes
        {
            get
            {
                if (getterInterceptorAttributes == null)
                    getterInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(property, typeof(IGetterInterceptorProvider));
                return getterInterceptorAttributes;
            }
        }

        public InterceptorAttributeDefinition[] SetterInterceptorAttributes
        {
            get
            {
                if (setterInterceptorAttributes == null)
                    setterInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(property, typeof(ISetterInterceptorProvider));
                return setterInterceptorAttributes;
            }
        }

        public string MemberFieldName
        {
            get { return $"{typeDefinition.Type.Name}_{Name}"; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
