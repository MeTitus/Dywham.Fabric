using System;
using System.Threading.Tasks;
using Autofac;
using Dywham.Fabric.Microservices.Contracts.Messages;
using NServiceBus.Pipeline;

namespace Dywham.Fabric.Microservices.Endpoint.Behaviors
{
    public sealed class IncomingMessageTrackingBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        private readonly ILifetimeScope _lifetimeScope;


        public IncomingMessageTrackingBehavior(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }
        

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            if (context.Message.Instance is not EndpointMessage incomingMessage)
            {
                await next().ConfigureAwait(false);

                return;
            }
            
            if (incomingMessage.TrackingId == Guid.Empty)
            {
                incomingMessage.TrackingId = Guid.NewGuid();
            }

            if (!context.Headers.ContainsKey("TrackingId"))
            {
                context.Headers.Add("TrackingId", incomingMessage.TrackingId.ToString());
            }

            if (!context.Headers.ContainsKey("OriginatedInTheContextOfPayloadTypeName"))
            {
                context.Headers.Add("OriginatedInTheContextOfPayloadTypeName", incomingMessage.GetType().FullName);
            }

            if (_lifetimeScope.TryResolve<IMessageReceivedBehavior>(out var runOnMessageDispatched))
            {
                runOnMessageDispatched.OnDywhamMessageProcessed(context, incomingMessage);
            }

            await next().ConfigureAwait(false);
        }
    }
}