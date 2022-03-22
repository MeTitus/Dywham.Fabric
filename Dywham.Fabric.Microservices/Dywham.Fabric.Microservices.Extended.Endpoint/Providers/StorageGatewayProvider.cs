using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Providers.IO;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Providers
{
    public class StorageGatewayProvider : IStorageGatewayProvider
    {
        public ExtendedEndpointDynamicSettings EndpointDynamicSettings { get; set; }

        public IIOProvider IOProvider { get; set; }


        public Stream OpenRead(Guid id)
        {
            return File.OpenRead(Path.Combine(EndpointDynamicSettings.AssetsLocation, id.ToString()));
        }

        public Task<string> ReadAsBase64Async(Guid id, CancellationToken token = default)
        {
            return IOProvider.ReadAsBase64Async(Path.Combine(EndpointDynamicSettings.AssetsLocation, id.ToString()), token);
        }

        public Stream OpenReadFromStaging(Guid id)
        {
            return File.OpenRead(Path.Combine(EndpointDynamicSettings.StagingAssetsLocation, id.ToString()));
        }

        public Task<string> ReadAsBase64FromStagingAsync(Guid id, CancellationToken token = default)
        {
            return IOProvider.ReadAsBase64Async(Path.Combine(EndpointDynamicSettings.StagingAssetsLocation, id.ToString()), token);
        }
    }
}