using NIntercept.Helpers;
using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class EventDefinition
    {
        private TypeDefinition typeDefinition;
        private EventInfo @event;
        private AddMethodDefinition addMethodDefinition;
        private RemoveMethodDefinition removeMethodDefinition;
        private InterceptorAttributeDefinition[] addOnInterceptorAttributes;
        private InterceptorAttributeDefinition[] removeOnInterceptorAttributes;

        public EventDefinition(TypeDefinition typeDefinition, EventInfo @event)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            this.typeDefinition = typeDefinition;
            this.@event = @event;
        }

        public TypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public EventInfo Event
        {
            get { return @event; }
        }

        public  string Name
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

        public AddMethodDefinition AddMethodDefinition
        {
            get
            {
                if (@event.AddMethod != null)
                    addMethodDefinition = new AddMethodDefinition(typeDefinition, this, @event.AddMethod);
                return addMethodDefinition;
            }
        }

        public RemoveMethodDefinition RemoveMethodDefinition
        {
            get
            {
                if (@event.RemoveMethod != null)
                    removeMethodDefinition = new RemoveMethodDefinition(typeDefinition, this, @event.RemoveMethod);
                return removeMethodDefinition;
            }
        }

        public InterceptorAttributeDefinition[] AddOnInterceptorAttributes
        {
            get
            {
                if (addOnInterceptorAttributes == null)
                    addOnInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(@event, typeof(IAddOnInterceptorProvider));
                return addOnInterceptorAttributes;
            }
        }

        public InterceptorAttributeDefinition[] RemoveOnInterceptorAttributes
        {
            get
            {
                if (removeOnInterceptorAttributes == null)
                    removeOnInterceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(@event, typeof(IRemoveOnInterceptorProvider));
                return removeOnInterceptorAttributes;
            }
        }

        public string MemberFieldName
        {
            get { return $"{typeDefinition.Type.Name}_{Name}"; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
