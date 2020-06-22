using NIntercept.Definition;
using NIntercept.Helpers;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyEventBuilder : IProxyEventBuilder
    {
        private static readonly IProxyMethodBuilder DefaultProxyMethodBuilder;
        private IProxyMethodBuilder proxyMethodBuilder;

        static ProxyEventBuilder()
        {
            DefaultProxyMethodBuilder = new ProxyMethodBuilder();
        }

        public virtual IProxyMethodBuilder ProxyMethodBuilder
        {
            get { return proxyMethodBuilder ?? DefaultProxyMethodBuilder; }
            set { proxyMethodBuilder = value; }
        }

        public virtual EventBuilder CreateEvent(ModuleScope moduleScope, TypeBuilder typeBuilder, EventDefinition eventDefinition, FieldBuilder[] fields)
        {
            EventBuilder eventBuilder = DefineEvent(typeBuilder, eventDefinition);

            AddEventMethodDefinition addEventMethodDefinition = eventDefinition.AddEventMethodDefinition;
            if (addEventMethodDefinition != null)
            {
                MethodBuilder addMethodBuilder = ProxyMethodBuilder.CreateMethod(moduleScope, typeBuilder, addEventMethodDefinition, eventDefinition.Event, fields);
                // copy attributes to method
                InterceptorAttributeHelper.AddInterceptorAttributes(addMethodBuilder, eventDefinition.AddEventInterceptorAttributes);
                eventBuilder.SetAddOnMethod(addMethodBuilder);
            }

            RemoveEventMethodDefinition removeEventMethodDefinition = eventDefinition.RemoveEventMethodDefinition;
            if (removeEventMethodDefinition != null)
            {
                MethodBuilder removeMethodBuilder = ProxyMethodBuilder.CreateMethod(moduleScope, typeBuilder, removeEventMethodDefinition, eventDefinition.Event, fields);
                InterceptorAttributeHelper.AddInterceptorAttributes(removeMethodBuilder, eventDefinition.RemoveEventInterceptorAttributes);
                eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
            }

            return eventBuilder;
        }

        protected virtual EventBuilder DefineEvent(TypeBuilder typeBuilder, EventDefinition eventDefinition)
        {
            return typeBuilder.DefineEvent(eventDefinition.Name, eventDefinition.Attributes, eventDefinition.EventHandlerType);
        }
    }
}
