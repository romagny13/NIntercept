using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class CallbackMethodDefinition
    {
        private string name;
        private MethodDefinition methodDefinition;
        private TypeDefinition typeDefinition;

        public CallbackMethodDefinition(TypeDefinition typeDefinition, MethodDefinition methodDefinition)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (methodDefinition is null)
                throw new ArgumentNullException(nameof(methodDefinition));

            this.typeDefinition = typeDefinition;
            this.methodDefinition = methodDefinition;
        }

        public TypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public MethodDefinition MethodDefinition
        {
            get { return methodDefinition; }
        }

        public virtual string Name
        {
            get
            {
                if (name == null)
                    name = $"{MethodDefinition.Name}_Callback";
                return name;
            }
        }

        public virtual MethodAttributes MethodAttributes
        {
            get { return MethodAttributes.Public | MethodAttributes.HideBySig; }
        }

        public Type ReturnType
        {
            get { return methodDefinition.ReturnType; }
        }

        public Type[] GenericArguments
        {
            get { return methodDefinition.GenericArguments; }
        }

        public ParameterDefinition[] ParameterDefinitions
        {
            get { return methodDefinition.ParameterDefinitions; }
        }

        public MethodInfo Method
        {
            get { return methodDefinition.Method; }
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
