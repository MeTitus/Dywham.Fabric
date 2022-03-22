using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Client
{
    public interface ISignalRClientProvider : IProvider
    {
        event EventHandler<string> TokenReceived;

        event EventHandler<KeyValuePair<string, string>> MessageReceived;

        bool IsConnected { get; }


        Task StartAsync();

        Task StopAsync();
    }
}