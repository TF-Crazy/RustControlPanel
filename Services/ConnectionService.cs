// ════════════════════════════════════════════════════════════════════
// ConnectionService.cs - Connection management service (Singleton)
// ════════════════════════════════════════════════════════════════════

using System;
using System.Threading.Tasks;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Rpc;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.Services
{
    /// <summary>
    /// Manages server connection lifecycle (Singleton).
    /// Handles connection, disconnection, and reconnection.
    /// </summary>
    public sealed class ConnectionService
    {
        #region Singleton

        private static readonly Lazy<ConnectionService> _instance = new(() => new ConnectionService());

        /// <summary>
        /// Gets the singleton instance of ConnectionService.
        /// </summary>
        public static ConnectionService Instance => _instance.Value;

        #endregion

        #region Fields

        private ServerConfig? _currentConfig;

        #endregion

        #region Events

        /// <summary>
        /// Fired when connection state changes.
        /// </summary>
        public event EventHandler<bool>? ConnectionStateChanged;

        /// <summary>
        /// Fired when a connection error occurs.
        /// </summary>
        public event EventHandler<string>? ConnectionError;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether currently connected to a server.
        /// </summary>
        public bool IsConnected => BridgeClient.Instance.IsConnected;

        /// <summary>
        /// Gets the current server configuration.
        /// </summary>
        public ServerConfig? CurrentConfig => _currentConfig;

        #endregion

        #region Constructor

        private ConnectionService()
        {
            Logger.Instance.Debug("ConnectionService instance created");

            // Subscribe to BridgeClient events
            BridgeClient.Instance.ConnectionStateChanged += OnBridgeConnectionStateChanged;
            BridgeClient.Instance.ErrorOccurred += OnBridgeError;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connects to a server using the provided configuration.
        /// </summary>
        /// <param name="config">Server configuration</param>
        /// <returns>True if connection successful</returns>
        public async Task<bool> ConnectAsync(ServerConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            Logger.Instance.Info($"Connecting to {config.Host}:{config.Port}...");

            _currentConfig = config.Clone();

            // Build WebSocket URI
            var uri = config.GetWebSocketUri();

            // Attempt connection with password
            var success = await BridgeClient.Instance.ConnectAsync(uri, config.Password);

            if (success)
            {
                Logger.Instance.Info("Connection established");

                // Save to settings if requested
                if (config.SaveCredentials)
                {
                    SettingsService.Instance.AddOrUpdateServer(config);
                }

                // Update last connected host
                SettingsService.Instance.LastConnectedHost = config.Host;
            }
            else
            {
                Logger.Instance.Error("Connection failed");
                _currentConfig = null;
            }

            return success;
        }

        /// <summary>
        /// Disconnects from the current server.
        /// </summary>
        public async Task DisconnectAsync()
        {
            Logger.Instance.Info("Disconnecting from server...");

            await BridgeClient.Instance.DisconnectAsync();

            _currentConfig = null;

            Logger.Instance.Info("Disconnected");
        }

        /// <summary>
        /// Sends an RPC request to the server.
        /// </summary>
        /// <param name="writer">Writer containing the RPC message</param>
        public async Task SendRpcAsync(BridgeWriter writer)
        {
            if (!IsConnected)
            {
                Logger.Instance.Warning("Cannot send RPC: not connected");
                return;
            }

            var data = writer.ToArray();
            await BridgeClient.Instance.SendAsync(data);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles BridgeClient connection state changes.
        /// </summary>
        private void OnBridgeConnectionStateChanged(object? sender, bool isConnected)
        {
            Logger.Instance.Info($"Connection state changed: {(isConnected ? "Connected" : "Disconnected")}");
            ConnectionStateChanged?.Invoke(this, isConnected);
        }

        /// <summary>
        /// Handles BridgeClient errors.
        /// </summary>
        private void OnBridgeError(object? sender, Exception ex)
        {
            Logger.Instance.Error("Bridge error occurred", ex);
            ConnectionError?.Invoke(this, ex.Message);
        }

        #endregion
    }
}
