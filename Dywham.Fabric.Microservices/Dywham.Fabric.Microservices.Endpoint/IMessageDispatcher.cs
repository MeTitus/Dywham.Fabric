using NServiceBus;

namespace Dywham.Fabric.Microservices.Endpoint
{
    public interface IMessageDispatcher
    {
        IEndpointInstance EndpointInstance { get; }

        string Name { get; }

        void Stop();
    }
}