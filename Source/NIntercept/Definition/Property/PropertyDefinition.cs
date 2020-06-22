using NIntercept.Helpers;
using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class PropertyDefinition
    {
        private TypeDefinition typeDefinition;
        private PropertyInfo property;
        private ParameterInfo[] indexParameters;
        private PropertyGetMethodDefinition propertyGetMethodDefinition;
        private PropertySetMethodDefinition propertySetMethodDefinition;
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

        public PropertyGetMethodDefinition PropertyGetMethodDefinition
        {
            get
            {
                if (property.CanRead)
                {
                    if (propertyGetMethodDefinition == null)
                        propertyGetMethodDefinition = new PropertyGetMethodDefinition(TypeDefinition, this, property.GetMethod);

                }
                return propertyGetMethodDefinition;
            }
        }

        public PropertySetMethodDefinition PropertySetMethodDefinition
        {
            get
            {
                if (property.CanWrite)
                {
                    if (propertySetMethodDefinition == null)
                        propertySetMethodDefinition = new PropertySetMethodDefinition(TypeDefinition, this, property.SetMethod);
                }
                return propertySetMethodDefinition;
            }
        }


        public InterceptorAttributeDefinition[] PropertyGetInterceptorAttributes
        {
            get
            {
                if (propertyGetInterceptorAttributes == null)
                    propertyGetInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(property, typeof(IPropertyGetInterceptorProvider));
                return propertyGetInterceptorAttributes;
            }
        }

        public InterceptorAttributeDefinition[] PropertySetInterceptorAttributes
        {
            get
            {
                if (propertySetInterceptorAttributes == null)
                    propertySetInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(property, typeof(IPropertySetInterceptorProvider));
                return propertySetInterceptorAttributes;
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
