using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class RemoveMethodDefinition : MethodDefinition
    {
        private EventDefinition eventDefinition;
        private string callerMethodFieldName;

        public RemoveMethodDefinition(TypeDefinition typeDefinition, EventDefinition eventDefinition, MethodInfo method)
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

        public override MethodDefinitionType MethodDefinitionType
        {
            get { return MethodDefinitionType.RemoveOn; }
        }

        //public override string CallerMethodFieldName
        //{
        //    get
        //    {
        //        if (callerMethodFieldName == null)
        //            callerMethodFieldName = $"{TypeDefinition.Name}_{Name}";
        //        return callerMethodFieldName;
        //    }
        //}

        public override string MemberFieldName
        {
            get { return eventDefinition.MemberFieldName; }
        }
    }
}
