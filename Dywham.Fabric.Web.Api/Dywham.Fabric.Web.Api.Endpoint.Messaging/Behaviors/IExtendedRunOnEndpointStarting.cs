using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;

namespace Dywham.Fabric.Web.Api.Endpoint.Messaging.Behaviors
{
    public interface IExtendedRunOnEndpointStarting
    {
        Task OnEndpointStartingAsync(IEndpointInstance endpointInstance, ILifetimeScope scope, CancellationToken token);
    }
}
