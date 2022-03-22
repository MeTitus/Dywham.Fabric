using System;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Jobs
{
    public class ExtendedJob
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public int Version { get; set; }
    }
}