using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using RustControlPanel.Core.Rpc;

namespace RustControlPanel.Core.Network
{
    public class BridgeClient(string connectionUri) : IDisposable
    {
        private ClientWebSocket _socket = default!;
        private CancellationTokenSource _cts = default!;
        private readonly Uri _uri = new(connectionUri);

        // Événements pour informer l'UI ou les ViewModels
        public event Action? OnConnected;
        public event Action<string>? OnDisconnected;
        public event Action<byte[]>? OnMessageReceived;
        public event Action<Exception>? OnError;

        public bool IsConnected => _socket?.State == WebSocketState.Open;

        public async Task ConnectAsync()
        {
            _socket = new ClientWebSocket();
            _cts = new CancellationTokenSource();

            try
            {
                await _socket.ConnectAsync(_uri, _cts.Token);
                OnConnected?.Invoke();

                // Lance la boucle de réception sur un thread séparé
                _ = Task.Run(ReceiveLoop, _cts.Token);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                throw;
            }
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[1024 * 128]; // Buffer de 128KB pour les grosses listes d'entités
            try
            {
                while (_socket.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
                {
                    var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await DisconnectAsync("Serveur a fermé la connexion");
                        break;
                    }

                    if (result.Count > 0)
                    {
                        var data = new byte[result.Count];
                        Array.Copy(buffer, data, result.Count);
                        OnMessageReceived?.Invoke(data);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!_cts.Token.IsCancellationRequested)
                    OnError?.Invoke(ex);
            }
        }

        public async Task SendRpcAsync(uint rpcId, string method, params object[] args)
        {
            if (!IsConnected) return;

            var packet = new RpcPacket { Id = rpcId, Method = method, Parameters = args };
            var data = RpcHelper.Encode(packet);

            await _socket.SendAsync(
                new ArraySegment<byte>(data),
                WebSocketMessageType.Binary,
                true,
                _cts.Token
            );
        }

        public async Task DisconnectAsync(string reason = "") // "" au lieu de null
        {
            if (_socket != null)
            {
                _cts?.Cancel();
                if (_socket.State == WebSocketState.Open)
                {
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
                }
                _socket.Dispose();
                _socket = null!; // On utilise ! pour dire au compilateur qu'on sait ce qu'on fait
            }
            OnDisconnected?.Invoke(reason);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Correction CA1816
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cts?.Cancel();
                _socket?.Dispose();
            }
        }
    }
}