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
            Logger.Instance.Debug("ServerInfoHandler v3 - Using EXACT old C# structure");

            try
            {
                var info = new ServerInfo();

                info.Hostname = reader.ReadString();
                info.MaxPlayers = reader.ReadInt32();
                info.PlayerCount = reader.ReadInt32();
                reader.ReadInt32(); // Queued
                reader.ReadInt32(); // Joining
                reader.ReadInt32(); // Reserved
                int entityCount = reader.ReadInt32(); // EntityCount
                reader.ReadString(); // GameTime
                int uptime = reader.ReadInt32(); // Uptime
                string mapName = reader.ReadString(); // MapName
                info.Fps = reader.ReadFloat(); // Framerate

                Logger.Instance.Info($"✅ ServerInfo: {info.Hostname} | Players: {info.PlayerCount}/{info.MaxPlayers} | FPS: {info.Fps:F0}");

                // Fire event
                ServerInfoReceived?.Invoke(this, info);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Failed to parse ServerInfo", ex);
            }
        }

        #endregion
    }
}
