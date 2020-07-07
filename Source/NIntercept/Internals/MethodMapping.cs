using NIntercept.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    internal class MethodMapping
    {
        private MethodDefinition methodDefinition;
        private MethodBuilder methodBuilder;
        private FieldBuilder memberField;
        private FieldBuilder methodField;

        public MethodMapping(MethodDefinition methodDefinition, FieldBuilder memberField, FieldBuilder methodField, MethodBuilder methodBuilder)
        {
            if (methodDefinition is null)
                throw new ArgumentNullException(nameof(methodDefinition));
            if (memberField is null)
                throw new ArgumentNullException(nameof(memberField));
            if (methodField is null)
                throw new ArgumentNullException(nameof(methodField));
            if (methodBuilder is null)
                throw new ArgumentNullException(nameof(methodBuilder));

            this.methodDefinition = methodDefinition;
            this.memberField = memberField;
            this.methodField = methodField;
            this.methodBuilder = methodBuilder;
        }

        public MethodDefinition MethodDefinition
        {
            get { return methodDefinition; }
        }

        public MethodInfo Method
        {
            get { return methodDefinition.Method; }
        }

        public MethodBuilder MethodBuilder
        {
            get { return methodBuilder; }
        }

        public FieldBuilder MemberField
        {
            get { return memberField; }
        }

        public FieldBuilder MethodField
        {
            get { return methodField; }
        }

        public override string ToString()
        {
            return methodDefinition.Name;
        }
    }
}
