using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Client
{
    public class SignalRClientProvider : ISignalRClientProvider, IDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly Timer _connectionStatusTimer = new Timer(500);


        public event EventHandler<bool> ConnectionStateChanged;

        public event EventHandler<string> TokenReceived;

        public event EventHandler<KeyValuePair<string, string>> MessageReceived;

        public bool IsConnected { get; private set; }


        public SignalRClientProvider(string ip)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(ip)
                .Build();

            _hubConnection.On<string>(nameof(IClientNotification.RegisterAsync), x =>
            {
                TokenReceived?.Invoke(this, x);
            });

            _hubConnection.On<string, string>(nameof(IClientNotification.SendNotificationAsync),
                (x, y) => MessageReceived?.Invoke(this, new KeyValuePair<string, string>(x, y)));

            _connectionStatusTimer.Elapsed += ConnectionStatusTimerOnElapsed;
        }

        private async void ConnectionStatusTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            await StartConnection();
        }


        public async Task StartAsync()
        {
            await StartConnection();

            _connectionStatusTimer.Start();
        }


        private async Task StartConnection()
        {
            try
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    ChangeConnectionState(false);

                    await _hubConnection.StartAsync();

                    if (_hubConnection.State == HubConnectionState.Connected)
                    {
                        ChangeConnectionState(true);
                    }
                }
            }
            catch
            {
                ChangeConnectionState(false);
            }
        }

        private void ChangeConnectionState(bool connectedStatus)
        {
            if (IsConnected == connectedStatus) return;

            // It was connected
            ConnectionStateChanged?.Invoke(this, connectedStatus);

            IsConnected = connectedStatus;
        }

        public async Task StopAsync()
        {
            await _hubConnection.StopAsync();
        }

        public void Dispose()
        {
            _connectionStatusTimer.Stop();
            _connectionStatusTimer.Elapsed -= ConnectionStatusTimerOnElapsed;
            _connectionStatusTimer.Dispose();

            StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}