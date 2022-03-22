using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Endpoint.Handlers;
using Dywham.Fabric.Microservices.Extended.Contracts.Messages.Commands.Maintenance;
using Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events.Maintenance;
using Dywham.Fabric.Providers;
using NServiceBus;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Handlers.Maintenance
{
    public class PerformEndpointShutdownHandler : DywhamMessageHandler<PerformEndpointShutdown>
    {
        public IDateTimeProvider DateTimeProvider { get; set; }

        public IExtendedEndpointSettings EndpointSettings { get; set; }


        protected override async Task HandleAsync(CancellationToken token)
        {
            await Context.Publish(new EndpointShutdownPerformed
            {
                DateTime = DateTimeProvider.GetUtcNow(),
                EndpointName = EndpointSettings.EndpointName
            });

            DywhamEndpointInstance.Stop();
        }
    }
}