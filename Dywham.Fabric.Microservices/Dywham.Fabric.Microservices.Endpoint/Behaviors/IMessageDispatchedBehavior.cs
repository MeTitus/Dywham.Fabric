using Dywham.Fabric.Microservices.Contracts.Messages;
using NServiceBus.Pipeline;

namespace Dywham.Fabric.Microservices.Endpoint.Behaviors
{
    public interface IMessageDispatchedBehavior
    {
        void OnDywhamMessageProcessed(IOutgoingLogicalMessageContext context, EndpointMessage dywhamMessage);
    }
}