using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Server
{
    public interface IClientNotification
    {
        Task RegisterAsync(string connectionId);

        Task SendNotificationAsync(string type, string data);
    }
}