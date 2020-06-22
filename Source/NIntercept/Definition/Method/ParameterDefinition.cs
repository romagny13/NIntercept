using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class ParameterDefinition
    {
        private ParameterInfo parameter;
        private int index;
        private bool? isByRef;
        private Type elementType;

        public ParameterDefinition(ParameterInfo parameter, int index)
        {
            if (parameter is null)
                throw new ArgumentNullException(nameof(parameter));

            this.parameter = parameter;
            this.index = index;
        }

        public ParameterInfo Parameter
        {
            get { return parameter; }
        }

        public string Name
        {
            get { return parameter.Name; }
        }

        public int Index
        {
            get { return index; }
        }

        public Type ParameterType
        {
            get { return parameter.ParameterType; }
        }

        public ParameterAttributes Attributes
        {
            get { return parameter.Attributes; }
        }

        public Type ElementType
        {
            get
            {
                if (IsByRef && elementType == null)
                    elementType = parameter.ParameterType.GetElementType();
                return elementType;
            }
        }

        public bool IsByRef
        {
            get
            {
                if (!isByRef.HasValue)
                    isByRef = ParameterType.IsByRef;
                return isByRef.Value;
            }
        }

        public bool IsOut
        {
            get { return parameter.IsOut; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
