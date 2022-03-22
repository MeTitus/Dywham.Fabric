using NServiceBus;

namespace Dywham.Fabric.Microservices.Endpoint
{
    public interface IDywhamEndpointInstance
    {
        IEndpointInstance EndpointInstance { get; }

        string Name { get; }

        void Stop();
    }
}