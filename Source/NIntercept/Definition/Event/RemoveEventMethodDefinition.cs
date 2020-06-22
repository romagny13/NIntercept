using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class RemoveEventMethodDefinition : MethodDefinition
    {
        private EventDefinition eventDefinition;

        public RemoveEventMethodDefinition(TypeDefinition typeDefinition, EventDefinition eventDefinition, MethodInfo method)
            : base(typeDefinition, method)
        {
            if (eventDefinition is null)
                throw new ArgumentNullException(nameof(eventDefinition));
            this.eventDefinition = eventDefinition;
        }

        public EventDefinition EventDefinition
        {
            get { return eventDefinition; }
        }

        public override string Name
        {
            get { return $"remove_{eventDefinition.Name}"; }
        }
    }
}
