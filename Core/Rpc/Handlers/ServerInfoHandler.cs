// ════════════════════════════════════════════════════════════════════
// ServerInfoHandler.cs - Handles ServerInfo RPC responses
// ════════════════════════════════════════════════════════════════════

using System;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Rpc;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.Core.Rpc.Handlers
{
    /// <summary>
    /// Handles ServerInfo RPC responses.
    /// Updates server information (hostname, FPS, players, etc.).
    /// </summary>
    public class ServerInfoHandler : IRpcHandler
    {
        #region Events

        /// <summary>
        /// Fired when server info is received.
        /// </summary>
        public event EventHandler<ServerInfo>? ServerInfoReceived;

        #endregion

        #region IRpcHandler Implementation

        /// <summary>
        /// Gets the RPC ID for ServerInfo.
        /// </summary>
        public uint RpcId => RpcHelper.GetRpcId(RpcNames.SERVER_INFO);

        /// <summary>
        /// Handles the ServerInfo RPC response.
        /// </summary>
        public void Handle(BridgeReader reader)
        {
            Logger.Instance.Debug("ServerInfoHandler v2 - FIXED VERSION with all fields");

            try
            {
                var info = new ServerInfo
                {
                    Hostname = reader.ReadString(),
                    MaxPlayers = reader.ReadInt32(),
                    PlayerCount = reader.ReadInt32()
                };

                // Skip additional fields that we don't need right now
                reader.ReadInt32(); // Queued
                reader.ReadInt32(); // Joining
                reader.ReadInt32(); // ReservedSlots
                reader.ReadInt32(); // EntityCount
                reader.ReadString(); // GameTime
                reader.ReadInt32(); // Uptime
                reader.ReadString(); // Map

                info.Fps = reader.ReadFloat();

                // Skip even more fields
                reader.ReadInt32(); // Memory
                reader.ReadInt32(); // MemoryUsageSystem
                reader.ReadInt32(); // Collections
                reader.ReadInt32(); // NetworkIn
                reader.ReadInt32(); // NetworkOut
                reader.ReadBoolean(); // Restarting
                reader.ReadString(); // SaveCreatedTime
                reader.ReadInt32(); // Version
                reader.ReadString(); // Protocol

                Logger.Instance.Debug($"ServerInfo: {info.Hostname} | Players: {info.PlayerCount}/{info.MaxPlayers} | FPS: {info.Fps:F0}");

                // Fire event
                ServerInfoReceived?.Invoke(this, info);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Failed to parse ServerInfo", ex);
            }
        }

        #endregion
    }
}
