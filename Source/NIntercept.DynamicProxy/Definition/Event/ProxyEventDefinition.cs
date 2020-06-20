using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class ProxyEventDefinition
    {
        private ProxyTypeDefinition typeDefinition;
        private EventInfo @event;
        private AddEventMethodDefinition addEventMethodDefinition;
        private RemoveEventMethodDefinition removeEventMethodDefinition;
        private InterceptorAttributeDefinition[] addEventInterceptorAttributes;
        private InterceptorAttributeDefinition[] removeEventInterceptorAttributes;

        public ProxyEventDefinition(ProxyTypeDefinition typeDefinition, EventInfo @event)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            this.typeDefinition = typeDefinition;
            this.@event = @event;
        }

        public ProxyTypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public EventInfo Event
        {
            get { return @event; }
        }

        public virtual string Name
        {
            get { return @event.Name; }
        }

        public EventAttributes Attributes
        {
            get { return @event.Attributes; }
        }

        public Type EventHandlerType
        {
            get { return @event.EventHandlerType; }
        }

        public AddEventMethodDefinition AddEventMethodDefinition
        {
            get
            {
                if (@event.AddMethod != null)
                    addEventMethodDefinition = new AddEventMethodDefinition(typeDefinition, this, @event.AddMethod);
                return addEventMethodDefinition;
            }
        }

        public RemoveEventMethodDefinition RemoveEventMethodDefinition
        {
            get
            {
                if (@event.RemoveMethod != null)
                    removeEventMethodDefinition = new RemoveEventMethodDefinition(typeDefinition, this, @event.RemoveMethod);
                return removeEventMethodDefinition;
            }
        }

        public InterceptorAttributeDefinition[] AddEventInterceptorAttributes
        {
            get
            {
                if (addEventInterceptorAttributes == null)
                    addEventInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(@event, typeof(IAddEventInterceptorProvider));
                return addEventInterceptorAttributes;
            }
        }

        public InterceptorAttributeDefinition[] RemoveEventInterceptorAttributes
        {
            get
            {
                if (removeEventInterceptorAttributes == null)
                    removeEventInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(@event, typeof(IRemoveEventInterceptorProvider));
                return removeEventInterceptorAttributes;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
