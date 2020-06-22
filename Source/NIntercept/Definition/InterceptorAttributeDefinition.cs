using System;
using System.Linq;
using System.Reflection;

namespace NIntercept.Definition
{
    public class InterceptorAttributeDefinition
    {
        private CustomAttributeData attributeData;
        private MemberInfo member;
        private readonly int index;
        private Type interceptorType;

        public InterceptorAttributeDefinition(CustomAttributeData attributeData, MemberInfo member, int index)
        {
            if (attributeData is null)
                throw new ArgumentNullException(nameof(attributeData));
            if (member is null)
                throw new ArgumentNullException(nameof(member));
            if (!typeof(IInterceptorProvider).IsAssignableFrom(attributeData.AttributeType))
                throw new ArgumentException("The attribute must implement IInterceptorProvider");

            this.attributeData = attributeData;
            this.member = member;
            this.index = index;
        }

        public int Index
        {
            get { return index; }
        }

        public CustomAttributeData AttributeData
        {
            get { return attributeData; }
        }

        public MemberInfo Member
        {
            get { return member; }
        }

        public Type AttributeType
        {
            get { return attributeData.AttributeType; }
        }

        public Type InterceptorType
        {
            get
            {
                if (interceptorType == null)
                    interceptorType = GetInterceptorType();
                return interceptorType;
            }
        }

        private Type GetInterceptorType()
        {
            var type = member as Type;
            if (type != null && type.IsInterface)
            {
                int index = 0;
                var attributes = type.GetCustomAttributes();
                foreach (var @attribute in attributes)
                {
                    if (index == Index)
                        return ((IInterceptorProvider)attribute).InterceptorType;

                    index++;
                }
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    var interfaceAttributes = @interface.GetCustomAttributes();
                    foreach (var @attribute in interfaceAttributes)
                    {
                        if (index == Index)
                            return ((IInterceptorProvider)attribute).InterceptorType;
                        index++;
                    }
                }

                throw new ArgumentException("Failed to resolve InterceptorType");
            }
            else
            {
                Attribute[] candidates = member.GetCustomAttributes().ToArray();
                return ((IInterceptorProvider)candidates[index]).InterceptorType;
            }
        }
    }
}
