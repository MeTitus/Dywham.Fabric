using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Server
{
    public class NotificationsHub : Hub<IClientNotification>
    {
        private readonly IConnectionMappings _connectionMappings;


        public NotificationsHub(IConnectionMappings connectionMappings)
        {
            _connectionMappings = connectionMappings;
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connectionMappings.RemoveNotifications(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public override async Task OnConnectedAsync()
        {
            if (!_connectionMappings.VerifyExists(Context.ConnectionId))
            {
                await Clients.Caller.RegisterAsync(Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }
    }
}