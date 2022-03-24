using Dywham.Fabric.Microservices.Contracts.Messages;
using NServiceBus.Pipeline;

namespace Dywham.Fabric.Microservices.Endpoint.Behaviors
{
    public interface IMessageReceivedBehavior
    {
        void OnDywhamMessageProcessed(IIncomingLogicalMessageContext context, EndpointMessage dywhamMessage);
    }
}