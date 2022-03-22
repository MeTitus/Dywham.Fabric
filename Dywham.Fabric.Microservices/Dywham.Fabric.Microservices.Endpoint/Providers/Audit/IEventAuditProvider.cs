using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Microservices.Endpoint.Providers.Audit
{
    public interface IEventAuditProvider
    {
        Task StoreAsync(EventAuditEntry @event, CancellationToken token = default);
    }
}