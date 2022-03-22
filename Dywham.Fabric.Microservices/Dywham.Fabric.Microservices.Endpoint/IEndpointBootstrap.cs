namespace Dywham.Fabric.Microservices.Endpoint
{
    public interface IEndpointBootstrap
    {
        bool Start();

        void Stop();
    }
}