using Dywham.Fabric.Microservices.Contracts.Messages;
using NServiceBus.Pipeline;

namespace Dywham.Fabric.Microservices.Endpoint.Behaviors
{
    public interface IRunOnMessageDispatchedBehavior
    {
        void OnDywhamMessageProcessed(IOutgoingLogicalMessageContext context, DywhamMessage dywhamMessage);
    }
}