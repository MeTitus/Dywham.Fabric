using System;
using System.Collections.Generic;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Server
{
    public interface IConnectionMappings
    {
        void AddMappings(List<Type> types);

        bool VerifyExists(string connectionId);

        void AddNotifications(string connectionId, List<Type> types);

        void RemoveNotifications(string connectionId, List<Type> types = null);

        List<string> GetConnectionsForType(Type type);
    }
}