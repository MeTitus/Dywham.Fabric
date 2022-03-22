using System;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Contracts.Messages;
using NServiceBus;

namespace Dywham.Fabric.Web.Api.Endpoint.Messaging.Providers.Messaging
{
    public class BusDispatcher : IBusDispatcher
    {
        public IEndpointInstance EndpointInstance { get; set; }


        public async Task SendAsync<T>(T message, CancellationToken token) where T : DywhamMessage
        {
            OnBeforeSending?.Invoke(message);

            await EndpointInstance.Send(message);
        }


        public Action<DywhamMessage> OnBeforeSending { get; set; }
    }
}