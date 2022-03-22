using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Server
{
    public interface ISignalRServerProvider : IProvider
    {
        void AddMappings(List<Type> type);

        Task SendNotificationAsync(string connectionId, Type eventType, string data);

        void SubscribeToNotifications(string connectionId, List<Type> notifications);

        void UnsubscribeToNotifications(string connectionId, List<Type> notifications);
    }
}