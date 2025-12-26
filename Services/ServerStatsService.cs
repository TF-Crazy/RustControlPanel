// ════════════════════════════════════════════════════════════════════
// ServerStatsService.cs - Manages server statistics polling
// ════════════════════════════════════════════════════════════════════

using System;
using System.Timers;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Rpc;
using RustControlPanel.Core.Rpc.Handlers;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.Services
{
    /// <summary>
    /// Manages server statistics polling and updates (Singleton).
    /// Requests ServerInfo at regular intervals.
    /// </summary>
    public sealed class ServerStatsService : IDisposable
    {
        #region Singleton

        private static readonly Lazy<ServerStatsService> _instance = new(() => new ServerStatsService());

        /// <summary>
        /// Gets the singleton instance of ServerStatsService.
        /// </summary>
        public static ServerStatsService Instance => _instance.Value;

        #endregion

        #region Constants

        private const int SERVER_INFO_INTERVAL = 10000; // 10 seconds

        #endregion

        #region Fields

        private readonly Timer _serverInfoTimer;
        private readonly ServerInfoHandler _serverInfoHandler;
        private bool _isRunning = false;

        #endregion

        #region Events

        /// <summary>
        /// Fired when server info is updated.
        /// </summary>
        public event EventHandler<ServerInfo>? ServerInfoUpdated;

        #endregion

        #region Constructor

        private ServerStatsService()
        {
            // Create handler
            _serverInfoHandler = new ServerInfoHandler();
            _serverInfoHandler.ServerInfoReceived += OnServerInfoReceived;

            // Create timer
            _serverInfoTimer = new Timer(SERVER_INFO_INTERVAL);
            _serverInfoTimer.Elapsed += OnServerInfoTimerElapsed;
            _serverInfoTimer.AutoReset = true;

            // Subscribe to connection events
            ConnectionService.Instance.ConnectionStateChanged += OnConnectionStateChanged;

            Logger.Instance.Debug("ServerStatsService instance created");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts polling server statistics.
        /// </summary>
        public void Start()
        {
            if (_isRunning)
                return;

            Logger.Instance.Info("Starting server stats polling...");

            // Register handler
            RpcRouter.Instance.RegisterHandler(_serverInfoHandler);

            // Start timer
            _serverInfoTimer.Start();

            // Request immediately
            RequestServerInfo();

            _isRunning = true;
        }

        /// <summary>
        /// Stops polling server statistics.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
                return;

            Logger.Instance.Info("Stopping server stats polling...");

            _serverInfoTimer.Stop();

            _isRunning = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Requests server info from the server.
        /// </summary>
        private void RequestServerInfo()
        {
            try
            {
                using var writer = new BridgeWriter();
                writer.WriteRpcHeader(RpcNames.SERVER_INFO);

                _ = ConnectionService.Instance.SendRpcAsync(writer);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Failed to request ServerInfo", ex);
            }
        }

        private void OnServerInfoTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            RequestServerInfo();
        }

        private void OnServerInfoReceived(object? sender, ServerInfo info)
        {
            ServerInfoUpdated?.Invoke(this, info);
        }

        private void OnConnectionStateChanged(object? sender, bool isConnected)
        {
            if (isConnected)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Stop();
            _serverInfoTimer?.Dispose();
        }

        #endregion
    }
}
