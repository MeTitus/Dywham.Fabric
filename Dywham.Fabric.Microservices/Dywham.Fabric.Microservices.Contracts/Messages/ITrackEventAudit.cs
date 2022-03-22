using System;

namespace Dywham.Fabric.Microservices.Contracts.Messages
{
    public interface ITrackEventAudit
    {
        public Guid[] Identities { get; set; }
    }
}