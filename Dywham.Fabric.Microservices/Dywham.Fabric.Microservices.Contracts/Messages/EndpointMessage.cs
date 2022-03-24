using System;

namespace Dywham.Fabric.Microservices.Contracts.Messages
{
    public class EndpointMessage
    {
        public Guid TrackingId { get; set; }

        public DateTime DateTimeProcessed { get; set; }
    }
}