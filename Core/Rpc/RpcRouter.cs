// ════════════════════════════════════════════════════════════════════
// RpcRouter.cs - Routes incoming RPC messages to handlers
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Utils;

namespace RustControlPanel.Core.Rpc
{
    /// <summary>
    /// Routes incoming RPC messages to registered handlers (Singleton).
    /// </summary>
    public sealed class RpcRouter
    {
        #region Singleton

        private static readonly Lazy<RpcRouter> _instance = new(() => new RpcRouter());

        /// <summary>
        /// Gets the singleton instance of RpcRouter.
        /// </summary>
        public static RpcRouter Instance => _instance.Value;

        #endregion

        #region Fields

        private readonly Dictionary<uint, IRpcHandler> _handlers = new();
        private readonly object _lock = new();

        #endregion

        #region Constructor

        private RpcRouter()
        {
            Logger.Instance.Debug("RpcRouter instance created");

            // Subscribe to BridgeClient messages
            BridgeClient.Instance.MessageReceived += OnMessageReceived;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers an RPC handler.
        /// </summary>
        /// <param name="handler">Handler to register</param>
        public void RegisterHandler(IRpcHandler handler)
        {
            lock (_lock)
            {
                _handlers[handler.RpcId] = handler;
                Logger.Instance.Debug($"Registered handler for RPC ID: {handler.RpcId}");
            }
        }

        /// <summary>
        /// Unregisters an RPC handler.
        /// </summary>
        /// <param name="rpcId">RPC ID to unregister</param>
        public void UnregisterHandler(uint rpcId)
        {
            lock (_lock)
            {
                if (_handlers.Remove(rpcId))
                {
                    Logger.Instance.Debug($"Unregistered handler for RPC ID: {rpcId}");
                }
            }
        }

        /// <summary>
        /// Clears all registered handlers.
        /// </summary>
        public void ClearHandlers()
        {
            lock (_lock)
            {
                _handlers.Clear();
                Logger.Instance.Debug("Cleared all RPC handlers");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles incoming messages from BridgeClient.
        /// </summary>
        private void OnMessageReceived(object? sender, byte[] data)
        {
            try
            {
                using var reader = new BridgeReader(data);

                // Read RPC header
                reader.ReadRpcHeader(out int channel, out uint rpcId);

                Logger.Instance.Debug($"Received RPC - Channel: {channel}, ID: {rpcId}");

                // Find handler
                IRpcHandler? handler;
                lock (_lock)
                {
                    _handlers.TryGetValue(rpcId, out handler);
                }

                if (handler != null)
                {
                    // Execute on UI thread
                    System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        try
                        {
                            handler.Handle(reader);
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Error($"Handler error for RPC ID {rpcId}", ex);
                            
                            // Log raw data for debugging
                            Handlers.RawRpcLogger.LogRawData(rpcId, data, 8);
                        }
                    });
                }
                else
                {
                    Logger.Instance.Warning($"No handler registered for RPC ID: {rpcId}");
                    
                    // Log raw data for unknown RPC
                    Handlers.RawRpcLogger.LogRawData(rpcId, data, 8);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error routing RPC message", ex);
            }
        }

        #endregion
    }
}
