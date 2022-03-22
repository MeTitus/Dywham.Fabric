using System;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Contracts.Messages;

namespace Dywham.Fabric.Web.Api.Endpoint.Messaging.Providers.Messaging
{
    public interface IBusDispatcher
    {
        Action<DywhamMessage> OnBeforeSending { set; }


        Task SendAsync<T>(T command, CancellationToken token = default) where T : DywhamMessage;
    }
}