using NIntercept.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    internal class EventMapping
    {
        private EventDefinition eventDefinition;
        private EventBuilder eventBuilder;
        private FieldBuilder memberField;
        private FieldBuilder addMethodField;
        private FieldBuilder removeMethodField;
        private MethodBuilder addMethodBuilder;
        private MethodBuilder removeMethodBuilder;

        public EventMapping(EventDefinition eventDefinition, EventBuilder eventBuilder, FieldBuilder memberField, FieldBuilder addMethodField, FieldBuilder removeMethodField, MethodBuilder addMethodBuilder, MethodBuilder removeMethodBuilder)
        {
            if (eventDefinition is null)
                throw new ArgumentNullException(nameof(eventDefinition));
            if (eventBuilder is null)
                throw new ArgumentNullException(nameof(eventBuilder));
            if (memberField is null)
                throw new ArgumentNullException(nameof(memberField));
            if (addMethodField is null)
                throw new ArgumentNullException(nameof(addMethodField));
            if (removeMethodField is null)
                throw new ArgumentNullException(nameof(removeMethodField));
            if (addMethodBuilder is null)
                throw new ArgumentNullException(nameof(addMethodBuilder));
            if (removeMethodBuilder is null)
                throw new ArgumentNullException(nameof(removeMethodBuilder));

            this.eventDefinition = eventDefinition;
            this.eventBuilder = eventBuilder;
            this.memberField = memberField;
            this.addMethodField = addMethodField;
            this.removeMethodField = removeMethodField;
            this.addMethodBuilder = addMethodBuilder;
            this.removeMethodBuilder = removeMethodBuilder;
        }

        public EventDefinition EventDefinition
        {
            get { return eventDefinition; }
        }

        public EventInfo Event
        {
            get { return eventDefinition.Event; }
        }

        public EventBuilder EventBuilder
        {
            get { return eventBuilder; }
        }

        public FieldBuilder MemberField
        {
            get { return memberField; }
        }

        public FieldBuilder AddMethodField
        {
            get { return addMethodField; }
        }

        public FieldBuilder RemoveMethodField
        {
            get { return removeMethodField; }
        }

        public MethodBuilder AddMethodBuilder
        {
            get { return addMethodBuilder; }
        }

        public MethodBuilder RemoveMethodBuilder
        {
            get { return removeMethodBuilder; }
        }

        public override string ToString()
        {
            return eventDefinition.Name;
        }
    }
}
