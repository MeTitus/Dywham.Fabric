using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts
{
    public class Document
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Reference { get; set; }

        public string Description { get; set; }

        public string Extension { get; set; }

        public DateTime? DateTime { get; set; }

        public string UploadToken { get; set; }
    }
}