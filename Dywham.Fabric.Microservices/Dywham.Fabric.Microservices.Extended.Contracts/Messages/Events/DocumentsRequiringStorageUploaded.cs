using System;
using System.Collections.Generic;
using Dywham.Fabric.Microservices.Contracts.Messages;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events
{
    public class DocumentsRequiringStorageUploaded : ExtendedEvent, ITrackEventAudit
    {
        public List<DocumentRegistration> Documents { get; set; }

        public Guid[] Identities { get; set; }

        public string Reference { get; set; }
    }
}