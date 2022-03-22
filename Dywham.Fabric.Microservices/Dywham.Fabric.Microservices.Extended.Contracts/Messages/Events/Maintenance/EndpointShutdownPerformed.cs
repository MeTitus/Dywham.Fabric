using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events.Maintenance
{
    public class EndpointShutdownPerformed : ExtendedEvent
    {
        public string EndpointName { get; set; }

        public DateTime DateTime { get; set; }
    }
}