using System;
using System.Collections.Generic;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events
{
    public class CompositeDocumentGenerated : ExtendedEvent
    {
        public DateTime DateTime { get; set; }

        public string TemplateName { get; set; }

        public bool SendEmail { get; set; }

        public Dictionary<string, object> Params { get; set; }
    }
}