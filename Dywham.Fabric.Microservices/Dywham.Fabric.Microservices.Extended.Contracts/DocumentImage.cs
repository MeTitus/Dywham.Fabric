using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts
{
    public class DocumentImage : Document
    {
        public Guid? ThumbnailId { get; set; }

        public int? ReductionPercentage { get; set; }
    }
}