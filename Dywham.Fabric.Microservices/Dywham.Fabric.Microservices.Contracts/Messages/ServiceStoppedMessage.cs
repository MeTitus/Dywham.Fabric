namespace Dywham.Fabric.Microservices.Contracts.Messages
{
    public class ServiceStoppedMessage : DywhamMessage
    {
        public string EndpointName { get; set; }
    }
}