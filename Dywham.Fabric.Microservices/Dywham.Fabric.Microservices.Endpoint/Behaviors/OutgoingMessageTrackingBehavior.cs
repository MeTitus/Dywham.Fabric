using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Dywham.Fabric.Microservices.Contracts.Messages;
using Dywham.Fabric.Microservices.Endpoint.Adapters.EventAudit;
using Newtonsoft.Json;
using NServiceBus.Pipeline;
using NServiceBus.Transport;

namespace Dywham.Fabric.Microservices.Endpoint.Behaviors
{
    public sealed class OutgoingMessageTrackingBehavior : Behavior<IOutgoingLogicalMessageContext>
    {
        private readonly ILifetimeScope _lifetimeScope;


        public OutgoingMessageTrackingBehavior(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }
        

        public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {
            if (context.Message.Instance is not EndpointMessage outgoingMessage)
            {
                await next().ConfigureAwait(false);

                return;
            }

            if (_lifetimeScope.TryResolve<IMessageDispatchedBehavior>(out var runOnMessageDispatched))
            {
                runOnMessageDispatched.OnDywhamMessageProcessed(context, outgoingMessage);
            }

            IncomingMessage incomingMessage = null;

            if (outgoingMessage.TrackingId == Guid.Empty)
            {
                if (context.TryGetIncomingPhysicalMessage(out incomingMessage) && incomingMessage.Headers.TryGetValue("TrackingId", out var trackingId))
                {
                    outgoingMessage.TrackingId = Guid.Parse(trackingId);
                }
				else
				{
					outgoingMessage.TrackingId = Guid.NewGuid();
				}
            }

            outgoingMessage.DateTimeProcessed = DateTime.UtcNow;

            if (!(context.Message.Instance is ITrackEventAudit trackEventAudit))
            {
                await next().ConfigureAwait(false);

                return;
            }

            var setting = _lifetimeScope.Resolve<EndpointSettings>();

            if (!setting.EnableMessageExecutionAudit)
            {
                await next().ConfigureAwait(false);

                return;
            }

            if (!_lifetimeScope.TryResolve<IEventAuditProvider>(out var auditProvider))
            {
                throw new EventAuditProviderNotRegisteredException();
            }

            var eventToStore = new EventAuditEntry
            {
                DateTime = DateTime.UtcNow,
                Identities = trackEventAudit.Identities,
                Payload = JsonConvert.SerializeObject(trackEventAudit),
                PayloadTypeName = trackEventAudit.GetType().FullName,
                TrackingId = outgoingMessage.TrackingId
            };

            if (incomingMessage != null || context.TryGetIncomingPhysicalMessage(out incomingMessage))
            {
                eventToStore.OriginatedInTheContextOfPayload = Encoding.UTF8.GetString(incomingMessage.Body, 0, incomingMessage.Body.Length);

                if (!incomingMessage.Headers.ContainsKey("OriginatedInTheContextOfPayloadTypeName"))
                {
                    eventToStore.OriginatedInTheContextOfPayloadTypeName = incomingMessage.Headers["OriginatedInTheContextOfPayloadTypeName"];
                }
            }

            await auditProvider.StoreAsync(eventToStore, CancellationToken.None);

            await next().ConfigureAwait(false);
        }
    }
}