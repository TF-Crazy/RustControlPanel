// ════════════════════════════════════════════════════════════════════
// BridgeClient.cs - WebSocket client singleton for Carbon Bridge
// ════════════════════════════════════════════════════════════════════

using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using RustControlPanel.Core.Utils;

namespace RustControlPanel.Core.Bridge
{
    /// <summary>
    /// WebSocket client for Carbon WebControlPanel Bridge (Singleton).
    /// Manages connection, message sending, and receiving.
    /// </summary>
    public sealed class BridgeClient : IDisposable
    {
        #region Singleton

        private static readonly Lazy<BridgeClient> _instance = new(() => new BridgeClient());

        /// <summary>
        /// Gets the singleton instance of BridgeClient.
        /// </summary>
        public static BridgeClient Instance => _instance.Value;

        #endregion

        #region Constants

        private const int BUFFER_SIZE = 8192;
        private const int MAX_MESSAGE_SIZE = 1024 * 1024; // 1MB

        #endregion

        #region Fields

        private ClientWebSocket? _webSocket;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _receiveTask;
        private readonly object _lock = new();

        #endregion

        #region Events

        /// <summary>
        /// Fired when connection state changes.
        /// </summary>
        public event EventHandler<bool>? ConnectionStateChanged;

        /// <summary>
        /// Fired when a binary message is received.
        /// </summary>
        public event EventHandler<byte[]>? MessageReceived;

        /// <summary>
        /// Fired when an error occurs.
        /// </summary>
        public event EventHandler<Exception>? ErrorOccurred;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the client is connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                lock (_lock)
                {
                    return _webSocket?.State == WebSocketState.Open;
                }
            }
        }

        #endregion

        #region Constructor

        private BridgeClient()
        {
            Logger.Instance.Debug("BridgeClient instance created");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connects to the Carbon Bridge server.
        /// </summary>
        /// <param name="uri">WebSocket URI (e.g. ws://127.0.0.1:3050)</param>
        /// <param name="password">Connection password</param>
        /// <returns>True if connection successful</returns>
        public async Task<bool> ConnectAsync(string uri, string password)
        {
            try
            {
                // Add password to URI path if provided
                var fullUri = string.IsNullOrEmpty(password) 
                    ? uri 
                    : $"{uri}/{password}";

                Logger.Instance.Info($"Connecting to {fullUri}...");

                // Disconnect if already connected
                await DisconnectAsync();

                lock (_lock)
                {
                    _webSocket = new ClientWebSocket();
                    _cancellationTokenSource = new CancellationTokenSource();
                }

                await _webSocket.ConnectAsync(new Uri(fullUri), _cancellationTokenSource.Token);

                Logger.Instance.Info("Connected successfully");
                ConnectionStateChanged?.Invoke(this, true);

                // Start receiving messages
                _receiveTask = Task.Run(ReceiveLoopAsync);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Connection failed", ex);
                ErrorOccurred?.Invoke(this, ex);
                await DisconnectAsync();
                return false;
            }
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public async Task DisconnectAsync()
        {
            lock (_lock)
            {
                if (_webSocket == null)
                    return;
            }

            try
            {
                Logger.Instance.Info("Disconnecting...");

                // Cancel receive loop
                _cancellationTokenSource?.Cancel();

                // Wait for receive task to complete
                if (_receiveTask != null)
                    await _receiveTask;

                // Close WebSocket
                if (_webSocket?.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
                }

                _webSocket?.Dispose();
                _cancellationTokenSource?.Dispose();

                Logger.Instance.Info("Disconnected");
                ConnectionStateChanged?.Invoke(this, false);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error during disconnect", ex);
            }
            finally
            {
                lock (_lock)
                {
                    _webSocket = null;
                    _cancellationTokenSource = null;
                    _receiveTask = null;
                }
            }
        }

        /// <summary>
        /// Sends a binary message to the server.
        /// </summary>
        /// <param name="data">Binary data to send</param>
        public async Task SendAsync(byte[] data)
        {
            if (!IsConnected)
            {
                Logger.Instance.Warning("Cannot send: not connected");
                return;
            }

            try
            {
                await _webSocket!.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, _cancellationTokenSource!.Token);
                Logger.Instance.Debug($"Sent {data.Length} bytes");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Send failed", ex);
                ErrorOccurred?.Invoke(this, ex);
                await DisconnectAsync();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Receive loop that continuously listens for messages.
        /// </summary>
        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[BUFFER_SIZE];
            var messageBuffer = new byte[MAX_MESSAGE_SIZE];
            var messageLength = 0;

            try
            {
                while (!_cancellationTokenSource!.Token.IsCancellationRequested && _webSocket!.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Logger.Instance.Info("Server closed connection");
                        await DisconnectAsync();
                        break;
                    }

                    // Copy received data to message buffer
                    Array.Copy(buffer, 0, messageBuffer, messageLength, result.Count);
                    messageLength += result.Count;

                    // If message is complete
                    if (result.EndOfMessage)
                    {
                        var message = new byte[messageLength];
                        Array.Copy(messageBuffer, message, messageLength);

                        Logger.Instance.Debug($"Received {messageLength} bytes");

                        // Fire event
                        MessageReceived?.Invoke(this, message);

                        // Reset buffer
                        messageLength = 0;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Instance.Debug("Receive loop cancelled");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Receive loop error", ex);
                ErrorOccurred?.Invoke(this, ex);
                await DisconnectAsync();
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the BridgeClient.
        /// </summary>
        public void Dispose()
        {
            DisconnectAsync().Wait();
        }

        #endregion
    }
}
