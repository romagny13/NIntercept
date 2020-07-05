using NIntercept.Definition;
using NIntercept.Helpers;
using System.Reflection.Emit;

namespace NIntercept.Builder
{
    public class ProxyEventBuilder : IProxyEventBuilder
    {
        public virtual EventBuilder CreateEvent(ProxyScope proxyScope, EventDefinition eventDefinition)
        {
            EventBuilder eventBuilder = DefineEvent(proxyScope.TypeBuilder, eventDefinition);

            DefineAddOnMethod(proxyScope, eventDefinition, eventBuilder);

            DefineRemoveOnMethod(proxyScope, eventDefinition, eventBuilder);

            return eventBuilder;
        }

        protected EventBuilder DefineEvent(TypeBuilder typeBuilder, EventDefinition eventDefinition)
        {
            return typeBuilder.DefineEvent(eventDefinition.Name, eventDefinition.Attributes, eventDefinition.EventHandlerType);
        }

        protected virtual void DefineAddOnMethod(ProxyScope proxyScope, EventDefinition eventDefinition, EventBuilder eventBuilder)
        {
            AddEventMethodDefinition addEventMethodDefinition = eventDefinition.AddEventMethodDefinition;
            if (addEventMethodDefinition != null)
            {
                MethodBuilder addMethodBuilder = proxyScope.CreateMethod(addEventMethodDefinition, eventDefinition.Event);
                if (ShouldAddInterceptionAttributes(addEventMethodDefinition))
                    AttributeHelper.AddInterceptorAttributes(addMethodBuilder, eventDefinition.AddEventInterceptorAttributes);
                eventBuilder.SetAddOnMethod(addMethodBuilder);
            }
        }

        protected virtual void DefineRemoveOnMethod(ProxyScope proxyScope, EventDefinition eventDefinition, EventBuilder eventBuilder)
        {
            RemoveEventMethodDefinition removeEventMethodDefinition = eventDefinition.RemoveEventMethodDefinition;
            if (removeEventMethodDefinition != null)
            {
                MethodBuilder removeMethodBuilder = proxyScope.CreateMethod(removeEventMethodDefinition, eventDefinition.Event);
                if (ShouldAddInterceptionAttributes(removeEventMethodDefinition))
                    AttributeHelper.AddInterceptorAttributes(removeMethodBuilder, eventDefinition.RemoveEventInterceptorAttributes);
                eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
            }
        }

        protected virtual bool ShouldAddInterceptionAttributes(MethodDefinition methodDefinition)
        {
            return true;
        }
    }
}
