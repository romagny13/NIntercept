﻿using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class AddMethodDefinition : MethodDefinition
    {
        private EventDefinition eventDefinition;
        private string callerMethodFieldName;

        public AddMethodDefinition(TypeDefinition typeDefinition, EventDefinition eventDefinition, MethodInfo method)
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
            get { return $"add_{eventDefinition.Name}"; }
        }

        public override MethodDefinitionType MethodDefinitionType
        {
            get { return MethodDefinitionType.AddOn; }
        }

        public override string MemberFieldName
        {
            get { return eventDefinition.MemberFieldName; }
        }
    }
}
