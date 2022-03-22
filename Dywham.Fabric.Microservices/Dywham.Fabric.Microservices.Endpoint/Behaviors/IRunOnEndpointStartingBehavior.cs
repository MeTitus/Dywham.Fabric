using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;

namespace Dywham.Fabric.Microservices.Endpoint.Behaviors
{
    public interface IRunOnEndpointStartingBehavior
    {
        Task OnEndpointStartingAsync(IEndpointInstance endpointInstance, ILifetimeScope scope, CancellationToken token);
    }
}
