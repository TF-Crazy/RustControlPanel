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

            // Log current position before parsing
            Logger.Instance.Debug($"Starting parse at position: {reader.Position}");

            try
            {
                // Log each field as we read it
                var hostname = reader.ReadString();
                Logger.Instance.Debug($"[1] Hostname: '{hostname}' (pos: {reader.Position})");

                var maxPlayers = reader.ReadInt32();
                Logger.Instance.Debug($"[2] MaxPlayers: {maxPlayers} (pos: {reader.Position})");

                var playerCount = reader.ReadInt32();
                Logger.Instance.Debug($"[3] PlayerCount: {playerCount} (pos: {reader.Position})");

                var info = new ServerInfo
                {
                    Hostname = hostname,
                    MaxPlayers = maxPlayers,
                    PlayerCount = playerCount
                };

                // Skip additional fields that we don't need right now
                reader.ReadInt32(); // Queued
                Logger.Instance.Debug($"[4] Queued (pos: {reader.Position})");

                reader.ReadInt32(); // Joining
                Logger.Instance.Debug($"[5] Joining (pos: {reader.Position})");

                reader.ReadInt32(); // ReservedSlots
                Logger.Instance.Debug($"[6] ReservedSlots (pos: {reader.Position})");

                reader.ReadInt32(); // EntityCount
                Logger.Instance.Debug($"[7] EntityCount (pos: {reader.Position})");

                var gameTime = reader.ReadString(); // GameTime
                Logger.Instance.Debug($"[8] GameTime: '{gameTime}' (pos: {reader.Position})");

                reader.ReadInt32(); // Uptime
                Logger.Instance.Debug($"[9] Uptime (pos: {reader.Position})");

                var map = reader.ReadString(); // Map
                Logger.Instance.Debug($"[10] Map: '{map}' (pos: {reader.Position})");

                info.Fps = reader.ReadFloat();
                Logger.Instance.Debug($"[11] FPS: {info.Fps} (pos: {reader.Position})");

                // Skip even more fields
                reader.ReadInt32(); // Memory
                Logger.Instance.Debug($"[12] Memory (pos: {reader.Position})");

                reader.ReadInt32(); // MemoryUsageSystem
                Logger.Instance.Debug($"[13] MemoryUsageSystem (pos: {reader.Position})");

                reader.ReadInt32(); // Collections
                Logger.Instance.Debug($"[14] Collections (pos: {reader.Position})");

                reader.ReadInt32(); // NetworkIn
                Logger.Instance.Debug($"[15] NetworkIn (pos: {reader.Position})");

                reader.ReadInt32(); // NetworkOut
                Logger.Instance.Debug($"[16] NetworkOut (pos: {reader.Position})");

                reader.ReadBoolean(); // Restarting
                Logger.Instance.Debug($"[17] Restarting (pos: {reader.Position})");

                var saveTime = reader.ReadString(); // SaveCreatedTime
                Logger.Instance.Debug($"[18] SaveCreatedTime: '{saveTime}' (pos: {reader.Position})");

                reader.ReadInt32(); // Version
                Logger.Instance.Debug($"[19] Version (pos: {reader.Position})");

                var protocol = reader.ReadString(); // Protocol
                Logger.Instance.Debug($"[20] Protocol: '{protocol}' (pos: {reader.Position})");

                Logger.Instance.Info($"✅ ServerInfo: {info.Hostname} | Players: {info.PlayerCount}/{info.MaxPlayers} | FPS: {info.Fps:F0}");

                // Fire event
                ServerInfoReceived?.Invoke(this, info);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Failed to parse ServerInfo at position {reader.Position}", ex);
            }
        }

        #endregion
    }
}
