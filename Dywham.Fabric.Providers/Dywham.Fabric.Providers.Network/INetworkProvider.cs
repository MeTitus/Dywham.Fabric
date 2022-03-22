namespace Dywham.Fabric.Providers.Network
{
    public interface INetworkProvider : IProvider
    {
        int NextTcpPort { get; }

        string MacAddress { get; }
    }
}