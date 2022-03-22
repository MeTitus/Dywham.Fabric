namespace Dywham.Fabric.Microservices.Extended.Contracts
{
    public class DocumentImageRegistration : DocumentRegistration
    {
        public bool RequiresThumbnail { get; set; }

        public int? ReductionPercentage { get; set; }
    }
}