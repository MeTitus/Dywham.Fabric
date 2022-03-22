using Dywham.Fabric.Microservices.Contracts.Messages;

namespace Dywham.Fabric.Microservices.Extended.Contracts.Messages
{
    public class ExtendedMessage : DywhamMessage
    {
        public string ClientNotificationTracking { get; set; }
    }
}