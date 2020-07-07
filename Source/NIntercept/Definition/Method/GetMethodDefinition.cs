using System;
using System.Linq;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class GetMethodDefinition : MethodDefinition
    {
        private PropertyDefinition propertyDefinition;
        private Type[] parameterTypes;
        private string callerMethodFieldName;

        public GetMethodDefinition(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition, MethodInfo method) : base(typeDefinition, method)
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
                    parameterTypes = propertyDefinition.IndexParameters.Length == 0 ? Type.EmptyTypes : propertyDefinition.IndexParameters.Select(p => p.ParameterType).ToArray();
                return parameterTypes;

            }
        }

        public override string Name
        {
            get { return $"get_{propertyDefinition.Name}"; }
        }

        public override MethodDefinitionType MethodDefinitionType
        {
            get { return MethodDefinitionType.Getter; }
        }

        public override string MemberFieldName
        {
            get { return propertyDefinition.MemberFieldName; }
        }
    }
}
