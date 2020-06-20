using NIntercept.Definition;
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

        public virtual EventBuilder CreateEvent(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, ProxyEventDefinition eventDefinition, FieldBuilder[] fields)
        {
            EventBuilder eventBuilder = DefineEvent(typeBuilder, eventDefinition);

            AddEventMethodDefinition addEventMethodDefinition = eventDefinition.AddEventMethodDefinition;
            if (addEventMethodDefinition != null)
            {
                MethodBuilder addMethodBuilder = ProxyMethodBuilder.CreateMethod(moduleBuilder, typeBuilder, addEventMethodDefinition, eventDefinition.Event, fields);
                // copy attributes to method
                MethodBuilderHelper.AddInterceptionAttributes(addMethodBuilder, eventDefinition.AddEventInterceptorAttributes);
                eventBuilder.SetAddOnMethod(addMethodBuilder);
            }

            RemoveEventMethodDefinition removeEventMethodDefinition = eventDefinition.RemoveEventMethodDefinition;
            if (removeEventMethodDefinition != null)
            {
                MethodBuilder removeMethodBuilder = ProxyMethodBuilder.CreateMethod(moduleBuilder, typeBuilder, removeEventMethodDefinition, eventDefinition.Event, fields);
                MethodBuilderHelper.AddInterceptionAttributes(removeMethodBuilder, eventDefinition.RemoveEventInterceptorAttributes);
                eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
            }

            return eventBuilder;
        }

        protected virtual EventBuilder DefineEvent(TypeBuilder typeBuilder, ProxyEventDefinition eventDefinition)
        {
            return typeBuilder.DefineEvent(eventDefinition.Name, eventDefinition.Attributes, eventDefinition.EventHandlerType);
        }
    }
}
