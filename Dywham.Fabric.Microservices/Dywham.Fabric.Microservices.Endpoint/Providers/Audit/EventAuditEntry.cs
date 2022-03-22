using System;

namespace Dywham.Fabric.Microservices.Endpoint.Providers.Audit
{
    public class EventAuditEntry
    {
        public Guid TrackingId { get; set; }

        public string Payload { get; set; }

        public string PayloadTypeName { get; set; }

        public DateTime? DateTime { get; set; }

        public string OriginatedInTheContextOfPayload { get; set; }

        public string OriginatedInTheContextOfPayloadTypeName { get; set; }

        public Guid[] Identities { get; set; }
    }
}