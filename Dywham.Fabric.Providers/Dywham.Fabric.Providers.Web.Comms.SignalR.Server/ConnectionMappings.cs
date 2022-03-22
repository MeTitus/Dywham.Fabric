using System;
using System.Collections.Generic;
using System.Linq;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Server
{
    public class ConnectionMappings : IConnectionMappings
    {
        public readonly Dictionary<Type, SynchronizedCollection<string>> _connectionsMappings = new Dictionary<Type, SynchronizedCollection<string>>();


        public void AddMappings(List<Type> types)
        {
            foreach (var type in types)
            {
                _connectionsMappings.Add(type, new SynchronizedCollection<string>());
            }
        }

        public bool VerifyExists(string connectionId)
        {
            return _connectionsMappings.SelectMany(x => x.Value)
                .Any(x => x.Equals(connectionId, StringComparison.OrdinalIgnoreCase));
        }

        public void AddNotifications(string connectionId, List<Type> types)
        {
            foreach (var type in types)
            {
                _connectionsMappings[type].Add(connectionId);
            }
        }

        public void RemoveNotifications(string connectionId, List<Type> types)
        {
            if (types != null)
            {
                foreach (var type in types)
                {
                    _connectionsMappings[type].Remove(connectionId);
                }
            }
            else
            {
                foreach (var connectionsMappingsValue in _connectionsMappings.Values)
                {
                    connectionsMappingsValue.Remove(connectionId);
                }
            }

        }

        public List<string> GetConnectionsForType(Type type)
        {
            return _connectionsMappings[type].ToList();
        }
    }
}