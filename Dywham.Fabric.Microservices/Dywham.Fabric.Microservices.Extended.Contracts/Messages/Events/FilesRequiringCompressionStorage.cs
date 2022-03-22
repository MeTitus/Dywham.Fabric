using System;
using System.Collections.Generic;
using Dywham.Fabric.Microservices.Contracts.Messages;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events
{
    public class FilesRequiringCompressionStorage : ExtendedEvent, ITrackEventAudit
    {
        public Dictionary<Guid, string> Documents { get; set; }

        public Guid[] Identities { get; set; }

        public string Reference { get; set; }

        public string FileName { get; set; }

        public string Tags { get; set; }
    }
}