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
        private PropertyGetMethodDefinition getMethodDefinition;
        private PropertySetMethodDefinition setMethodDefinition;
        private InterceptorAttributeDefinition[] propertyGetInterceptorAttributes;
        private InterceptorAttributeDefinition[] propertySetInterceptorAttributes;

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

        public PropertyGetMethodDefinition GetMethodDefinition
        {
            get
            {
                if (property.CanRead)
                {
                    if (getMethodDefinition == null)
                        getMethodDefinition = new PropertyGetMethodDefinition(TypeDefinition, this, property.GetMethod);

                }
                return getMethodDefinition;
            }
        }

        public PropertySetMethodDefinition SetMethodDefinition
        {
            get
            {
                if (property.CanWrite)
                {
                    if (setMethodDefinition == null)
                        setMethodDefinition = new PropertySetMethodDefinition(TypeDefinition, this, property.SetMethod);
                }
                return setMethodDefinition;
            }
        }


        public InterceptorAttributeDefinition[] PropertyGetInterceptorAttributes
        {
            get
            {
                if (propertyGetInterceptorAttributes == null)
                    propertyGetInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(property, typeof(IGetterInterceptorProvider));
                return propertyGetInterceptorAttributes;
            }
        }

        public InterceptorAttributeDefinition[] PropertySetInterceptorAttributes
        {
            get
            {
                if (propertySetInterceptorAttributes == null)
                    propertySetInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(property, typeof(ISetterInterceptorProvider));
                return propertySetInterceptorAttributes;
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
