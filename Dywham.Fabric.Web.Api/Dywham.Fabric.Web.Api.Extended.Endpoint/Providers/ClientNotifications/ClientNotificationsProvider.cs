using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dywham.Fabric.Providers.Web.Comms.SignalR.Server;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint.Providers.ClientNotifications
{
    public class ClientNotificationsProvider : IClientNotificationsProvider
    {
        private readonly ISignalRServerProvider _provider;
        private List<Type> _registeredEvents;


        public ClientNotificationsProvider(ISignalRServerProvider provider)
        {
            _provider = provider;
        }

        
        public void RegisterEventTypes(List<Type> eventTypes)
        {
            _registeredEvents = eventTypes;

            _provider.AddMappings(eventTypes);
        }

        public async Task SendNotificationAsync(string eventName, string data, string clientNotificationTracking = null)
        {
            foreach (var type in _registeredEvents.Where(x => x.Name.Equals(eventName, StringComparison.OrdinalIgnoreCase)))
            {
                await _provider.SendNotificationAsync(clientNotificationTracking, type, data);
            }
        }

        public void SubscribeToNotifications(string clientNotificationTracking, List<string> notifications)
        {
            // ReSharper disable once PossibleNullReferenceException
            var types = _registeredEvents.Where(x => notifications.Any(y => x.FullName.Equals(y, StringComparison.InvariantCultureIgnoreCase))).ToList();

            _provider.SubscribeToNotifications(clientNotificationTracking, types);
        }

        public void UnsubscribeToNotifications(string clientNotificationTracking, List<string> notifications)
        {
            // ReSharper disable once PossibleNullReferenceException
            var types = _registeredEvents.Where(x => notifications.Any(y => x.FullName.Equals(y, StringComparison.InvariantCultureIgnoreCase))).ToList();

            _provider.UnsubscribeToNotifications(clientNotificationTracking, types);
        }
    }
}