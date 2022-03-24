using Dywham.Fabric.Microservices.Contracts.Messages;
using Dywham.Fabric.Microservices.Endpoint.Behaviors;
using Dywham.Fabric.Microservices.Extended.Contracts.Messages;
using NServiceBus.Pipeline;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Behaviors
{
    public class MessageDispatcherBehavior : IMessageDispatchedBehavior, IMessageReceivedBehavior
    {
        public void OnDywhamMessageProcessed(IOutgoingLogicalMessageContext context, EndpointMessage message)
        {
            if (message is ExtendedMessage templatedMessage &&
                context.TryGetIncomingPhysicalMessage(out var incomingMessage) &&
                incomingMessage.Headers.TryGetValue("ClientNotificationTracking", out var clientNotificationTracking))
            {
                templatedMessage.ClientNotificationTracking = clientNotificationTracking;
            }
        }

        public void OnDywhamMessageProcessed(IIncomingLogicalMessageContext context, EndpointMessage message)
        {
            if (message is ExtendedMessage templatedMessage)
            {
                context.Headers.Add("ClientNotificationTracking", templatedMessage.ClientNotificationTracking);
            }
        }
    }
}