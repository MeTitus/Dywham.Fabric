using System;
using Dywham.Fabric.Microservices.Contracts.Messages;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events
{
    public class OperationalJobStateToggled : ExtendedEvent, ITrackEventAudit
    {
        public Guid Id { get; set; }

        public bool Enable { get; set; }

        public string EndpointName { get; set; }

        public Guid[] Identities { get; set; }
    }
}