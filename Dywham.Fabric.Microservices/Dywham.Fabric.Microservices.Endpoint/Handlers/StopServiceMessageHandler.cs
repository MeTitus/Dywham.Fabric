using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Contracts.Messages;
using NServiceBus;

namespace Dywham.Fabric.Microservices.Endpoint.Handlers
{
    public class StopServiceMessageHandler : MessageHandler<StopServiceMessage>
    {
        protected override async Task HandleAsync(CancellationToken token)
        {
            await Context.Publish(new ServiceStoppedMessage
            {
                EndpointName = DywhamEndpointInstance.Name
            });

            ThreadPool.QueueUserWorkItem(_ =>
            {
                DywhamEndpointInstance.Stop();
            });
        }
    }
}