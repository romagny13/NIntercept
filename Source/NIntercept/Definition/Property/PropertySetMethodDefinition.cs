using System;
using System.Linq;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class PropertySetMethodDefinition : MethodDefinition
    {
        private PropertyDefinition propertyDefinition;
        private Type[] parameterTypes;

        public PropertySetMethodDefinition(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition, MethodInfo method) : base(typeDefinition, method)
        {
            if (propertyDefinition is null)
            {
                throw new ArgumentNullException(nameof(propertyDefinition));
            }

            this.propertyDefinition = propertyDefinition;
        }

        public PropertyDefinition PropertyDefinition
        {
            get { return propertyDefinition; }
        }

        public Type[] ParameterTypes
        {
            get
            {
                if (parameterTypes == null)
                {
                    parameterTypes = propertyDefinition.IndexParameters.Length == 0 ?
                        new Type[] { propertyDefinition.PropertyType }
                    : propertyDefinition.IndexParameters.Select(p => p.ParameterType).ToArray().Concat(new Type[] { propertyDefinition.PropertyType }).ToArray();
                }
                return parameterTypes;
            }
        }

        public override string Name
        {
            get { return $"set_{propertyDefinition.Name}"; }
        }

        public override MethodDefinitionType MethodDefinitionType
        {
            get { return MethodDefinitionType.Setter; }
        }
    }
}
