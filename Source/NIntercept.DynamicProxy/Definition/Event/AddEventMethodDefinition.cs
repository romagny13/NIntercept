using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class AddEventMethodDefinition : ProxyMethodDefinition
    {
        private ProxyEventDefinition eventDefinition;

        public AddEventMethodDefinition(ProxyTypeDefinition typeDefinition, ProxyEventDefinition eventDefinition, MethodInfo method)
            : base(typeDefinition, method)
        {
            if (eventDefinition is null)
                throw new ArgumentNullException(nameof(eventDefinition));
            this.eventDefinition = eventDefinition;
        }

        public ProxyEventDefinition EventDefinition
        {
            get { return eventDefinition; }
        }

        public override string Name
        {
            get { return $"add_{eventDefinition.Name}"; }
        }
    }
}
