using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Client
{
    public interface IClientNotification
    {
        Task RegisterAsync(string connectionId);

        Task SendNotificationAsync(string type, string data);
    }
}