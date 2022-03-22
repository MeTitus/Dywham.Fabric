using System.Threading.Tasks;
using NServiceBus;
using Quartz;

namespace Dywham.Fabric.Microservices.Endpoint.JobScheduling
{
    public interface IDywhamJob
    {
        ITrigger Trigger { get; }


        Task ExecuteAsync(IJobExecutionContext context, IEndpointInstance endpointInstance);
    }
}