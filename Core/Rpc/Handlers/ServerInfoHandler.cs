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
            try
            {
                var info = new ServerInfo
                {
                    Hostname = reader.ReadString(),
                    PlayerCount = reader.ReadInt32(),
                    MaxPlayers = reader.ReadInt32(),
                    Fps = reader.ReadFloat(),
                    GameTime = reader.ReadFloat(),
                    Uptime = reader.ReadFloat(),
                    MapSize = reader.ReadInt32(),
                    MapSeed = reader.ReadInt32()
                };

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
