using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events
{
    public class LongRunningTaskStarted : ExtendedEvent, ILongRunningTaskStarted
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime DateTime { get; set; }

        public string EndpointName { get; set; }
    }
}