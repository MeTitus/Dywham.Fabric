using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events
{
    public class LongRunningTaskCompleted : ExtendedEvent, ILongRunningTaskCompleted
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }
    }
}