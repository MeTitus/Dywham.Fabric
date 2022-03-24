namespace Dywham.Fabric.Microservices.Contracts.Messages
{
    public class ServiceStoppedMessage : EndpointMessage
    {
        public string EndpointName { get; set; }
    }
}