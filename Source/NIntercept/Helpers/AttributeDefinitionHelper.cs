using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NIntercept.Helpers
{

    public class AttributeDefinitionHelper
    {
        public static InterceptorAttributeDefinition[] GetInterceptorDefinitions(Type type, Type interceptorType)
        {
            var interceptorAttributeDefinitions = new List<InterceptorAttributeDefinition>();

            if (type.IsInterface)
            {
                int index = 0;
                var attributes = type.GetCustomAttributesData();
                foreach (var @attribute in attributes)
                {
                    if (interceptorType.IsAssignableFrom(attribute.AttributeType))
                        interceptorAttributeDefinitions.Add(new InterceptorAttributeDefinition(attribute, type, index));

                    index++;
                }
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    var interfaceAttributes = @interface.GetCustomAttributesData();
                    foreach (var @attribute in interfaceAttributes)
                    {
                        if (interceptorType.IsAssignableFrom(@attribute.AttributeType))
                            interceptorAttributeDefinitions.Add(new InterceptorAttributeDefinition(@attribute, type, index));
                        index++;
                    }
                }
            }
            else
            {
                var attributes = type.GetCustomAttributesData();
                for (int i = 0; i < attributes.Count; i++)
                {
                    var attribute = attributes[i];
                    if (interceptorType.IsAssignableFrom(attribute.AttributeType))
                        interceptorAttributeDefinitions.Add(new InterceptorAttributeDefinition(attribute, type, i));
                }
            }
            return interceptorAttributeDefinitions.ToArray();
        }

        public static InterceptorAttributeDefinition[] GetInterceptorDefinitions(MemberInfo member, Type interceptorType)
        {
            var interceptorAttributeDefinitions = new List<InterceptorAttributeDefinition>();
            var customAttributeDatas = member.GetCustomAttributesData();
            for (int i = 0; i < customAttributeDatas.Count; i++)
            {
                var customAttributeData = customAttributeDatas[i];
                if (interceptorType.IsAssignableFrom(customAttributeData.AttributeType))
                    interceptorAttributeDefinitions.Add(new InterceptorAttributeDefinition(customAttributeData, member, i));
            }
            return interceptorAttributeDefinitions.ToArray();
        }
    }
}
