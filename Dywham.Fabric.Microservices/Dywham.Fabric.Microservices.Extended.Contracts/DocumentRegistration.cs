namespace Dywham.Fabric.Microservices.Extended.Contracts
{
    public class DocumentRegistration
    {
        public string UploadToken { get; set; }

        public string Base64 { get; set; }

        public string Reference { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Extension { get; set; }

        public string Tags { get; set; }
    }
}