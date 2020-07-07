using NIntercept.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    internal class PropertyMapping
    {
        private PropertyDefinition propertyDefinition;
        private FieldBuilder memberField;
        private FieldBuilder getMethodField;
        private FieldBuilder setMethodField;
        private PropertyBuilder propertyBuilder;
        private MethodBuilder getMethodBuilder;
        private MethodBuilder setMethodBuilder;

        public PropertyMapping(PropertyDefinition propertyDefinition, FieldBuilder memberField, FieldBuilder getMethodField, FieldBuilder setMethodField, PropertyBuilder propertyBuilder, MethodBuilder getMethodBuilder, MethodBuilder setMethodBuilder)
        {
            if (propertyDefinition is null)
                throw new ArgumentNullException(nameof(propertyDefinition));
            if (memberField is null)
                throw new ArgumentNullException(nameof(memberField));
            if (propertyBuilder is null)
                throw new ArgumentNullException(nameof(propertyBuilder));

            this.propertyDefinition = propertyDefinition;
            this.propertyBuilder = propertyBuilder;
            this.getMethodBuilder = getMethodBuilder;
            this.setMethodBuilder = setMethodBuilder;
            this.memberField = memberField;
            this.getMethodField = getMethodField;
            this.setMethodField = setMethodField;
        }

        public PropertyDefinition PropertyDefinition
        {
            get { return propertyDefinition; }
        }

        public PropertyBuilder PropertyBuilder
        {
            get { return propertyBuilder; }
        }

        public FieldBuilder MemberField
        {
            get { return memberField; }
        }

        public FieldBuilder GetMethodField
        {
            get { return getMethodField; }
        }

        public FieldBuilder SetMethodField
        {
            get { return setMethodField; }
        }

        public PropertyInfo Property
        {
            get { return propertyDefinition.Property; }
        }

        public MethodBuilder GetMethodBuilder
        {
            get { return getMethodBuilder; }
        }

        public MethodBuilder SetMethodBuilder
        {
            get { return setMethodBuilder; }
        }

        public override string ToString()
        {
            return propertyDefinition.Name;
        }
    }
}
