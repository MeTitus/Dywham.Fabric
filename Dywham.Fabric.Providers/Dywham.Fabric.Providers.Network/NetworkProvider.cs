using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Dywham.Fabric.Providers.Network
{
    public class NetworkProvider : INetworkProvider
    {
        public int NextTcpPort
        {
            get
            {
                var listener = new TcpListener(IPAddress.Loopback, 0);

                try
                {
                    listener.Start();

                    return ((IPEndPoint)listener.LocalEndpoint).Port;
                }
                finally
                {
                    listener.Stop();
                }
            }
        }

        public string MacAddress
        {
            get
            {
                return NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();
            }
        }
    }
}