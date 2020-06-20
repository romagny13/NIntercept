using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class RemoveEventMethodDefinition : ProxyMethodDefinition
    {
        private ProxyEventDefinition eventDefinition;

        public RemoveEventMethodDefinition(ProxyTypeDefinition typeDefinition, ProxyEventDefinition eventDefinition, MethodInfo method)
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
            get { return $"remove_{eventDefinition.Name}"; }
        }
    }
}
