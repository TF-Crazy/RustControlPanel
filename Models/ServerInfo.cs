// ════════════════════════════════════════════════════════════════════
// ServerInfo.cs - COMPLETE model with ALL properties
// ════════════════════════════════════════════════════════════════════

namespace RustControlPanel.Models
{
    public class ServerInfo
    {
        public string Hostname { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public int MaxPlayers { get; set; }
        public int QueuedPlayers { get; set; }      // ✅ ADDED
        public int JoiningPlayers { get; set; }     // ✅ ADDED
        public int EntityCount { get; set; }        // ✅ ADDED
        public string GameTime { get; set; } = "00:00";  // ✅ CHANGED to string
        public int Uptime { get; set; }             // ✅ CHANGED to int (seconds)
        public string MapName { get; set; } = string.Empty;  // ✅ ADDED
        public float Fps { get; set; }
        public int MapSize { get; set; }
        public int MapSeed { get; set; }
    }
}
