using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class CustomAttributeDefinition
    {
        private ConstructorInfo constructor;
        private object[] args;

        public CustomAttributeDefinition(ConstructorInfo constructor, object[] args)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            this.constructor = constructor;
            this.args = args;
        }

        public ConstructorInfo Constructor
        {
            get { return constructor; }
        }

        public object[] Args
        {
            get { return args; }
        }

    }
}
