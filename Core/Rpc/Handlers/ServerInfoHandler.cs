// ════════════════════════════════════════════════════════════════════
// ServerInfoHandler.cs - FIXED - Parse ALL data
// ════════════════════════════════════════════════════════════════════

using System;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Rpc;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.Core.Rpc.Handlers
{
    public class ServerInfoHandler : IRpcHandler
    {
        public event EventHandler<ServerInfo>? ServerInfoReceived;

        public uint RpcId => RpcHelper.GetRpcId(RpcNames.SERVER_INFO);

        public void Handle(BridgeReader reader)
        {
            try
            {
                var info = new ServerInfo();

                info.Hostname = reader.ReadString();
                info.MaxPlayers = reader.ReadInt32();
                info.PlayerCount = reader.ReadInt32();
                info.QueuedPlayers = reader.ReadInt32();      // ✅ PARSE
                info.JoiningPlayers = reader.ReadInt32();     // ✅ PARSE
                reader.ReadInt32(); // Reserved
                info.EntityCount = reader.ReadInt32();        // ✅ PARSE
                info.GameTime = reader.ReadString();          // ✅ PARSE (string)
                info.Uptime = reader.ReadInt32();             // ✅ PARSE (seconds)
                info.MapName = reader.ReadString();           // ✅ PARSE
                info.Fps = reader.ReadFloat();

                Logger.Instance.Debug($"ServerInfo: {info.Hostname} | {info.PlayerCount}/{info.MaxPlayers} | Queue:{info.QueuedPlayers} | Join:{info.JoiningPlayers} | Entities:{info.EntityCount} | Time:{info.GameTime} | Uptime:{info.Uptime}s | FPS:{info.Fps:F0}");

                ServerInfoReceived?.Invoke(this, info);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Failed to parse ServerInfo", ex);
            }
        }
    }
}
