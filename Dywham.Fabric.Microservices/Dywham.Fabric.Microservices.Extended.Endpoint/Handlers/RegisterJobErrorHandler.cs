using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Endpoint.Handlers;
using Dywham.Fabric.Microservices.Extended.Contracts.Messages.Commands;
using Dywham.Fabric.Microservices.Extended.Endpoint.Jobs;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Handlers
{
    public class RegisterJobErrorHandler : DywhamMessageHandler<RegisterJobError>
    {

        protected override Task HandleAsync(CancellationToken token)
        {
            throw new ExtendedJobRunnerException(Message.Error);
        }
    }
}