using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Server
{
    public class SignalRServerProvider : ISignalRServerProvider
    {
        private readonly IHubContext<NotificationsHub, IClientNotification> _hubContext;
        private readonly IConnectionMappings _connectionMappingHolder;


        public SignalRServerProvider(IHubContext<NotificationsHub, IClientNotification> hubContext, IConnectionMappings connectionMappingHolder)
        {
            _hubContext = hubContext;
            _connectionMappingHolder = connectionMappingHolder;
        }

        public void AddMappings(List<Type> type)
        {
            _connectionMappingHolder.AddMappings(type);
        }

        public async Task SendNotificationAsync(string connectionId, Type type, string data)
        {
            var connectionIds = new List<string>();

            if (!string.IsNullOrWhiteSpace(connectionId))
            {
                connectionIds.Add(connectionId);
            }

            connectionIds.AddRange(_connectionMappingHolder.GetConnectionsForType(type));

            await SendNotificationImplAsync(connectionIds, type, data);
        }

        private async Task SendNotificationImplAsync(IReadOnlyList<string> connectionIds, Type type, string data)
        {
            if (connectionIds.Any())
            {
                var client = _hubContext.Clients.Clients(connectionIds);

                await client.SendNotificationAsync(type.FullName, data);
            }
        }

        public void SubscribeToNotifications(string connectionId, List<Type> notifications)
        {
            _connectionMappingHolder.AddNotifications(connectionId, notifications);
        }

        public void UnsubscribeToNotifications(string connectionId, List<Type> notifications)
        {
            _connectionMappingHolder.RemoveNotifications(connectionId, notifications);
        }
    }
}
