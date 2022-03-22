using Dywham.Fabric.Microservices.Contracts.Messages;
using NServiceBus.Pipeline;

namespace Dywham.Fabric.Microservices.Endpoint.Behaviors
{
    public interface IRunOnMessageReceivedBehavior
    {
        void OnDywhamMessageProcessed(IIncomingLogicalMessageContext context, DywhamMessage dywhamMessage);
    }
}