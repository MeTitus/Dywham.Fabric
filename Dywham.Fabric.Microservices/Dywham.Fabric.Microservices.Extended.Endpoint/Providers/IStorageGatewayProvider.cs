using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Providers
{
    public interface IStorageGatewayProvider
    {
        Stream OpenRead(Guid id);

        Task<string> ReadAsBase64Async(Guid id, CancellationToken token = default);

        Stream OpenReadFromStaging(Guid id);

        Task<string> ReadAsBase64FromStagingAsync(Guid id, CancellationToken token = default);
    }
}