using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint.Providers.ClientNotifications
{
    public interface IClientNotificationsProvider
    {
        void RegisterEventTypes(List<Type> eventTypes);

        Task SendNotificationAsync(string eventName, string data, string clientNotificationTracking = null);

        void SubscribeToNotifications(string clientNotificationTracking, List<string> notifications);

        void UnsubscribeToNotifications(string clientNotificationTracking, List<string> notifications);
    }
}