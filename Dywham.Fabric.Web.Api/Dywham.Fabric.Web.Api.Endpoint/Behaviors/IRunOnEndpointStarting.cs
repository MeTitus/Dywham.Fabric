using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace Dywham.Fabric.Web.Api.Endpoint.Behaviors
{
    public interface IRunOnEndpointStarting
    {
        Task OnEndpointStartingAsync(ILifetimeScope scope, CancellationToken token);
    }
}
