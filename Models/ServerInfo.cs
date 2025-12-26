// ════════════════════════════════════════════════════════════════════
// ServerInfo.cs - Server information model
// ════════════════════════════════════════════════════════════════════

namespace RustControlPanel.Models
{
    /// <summary>
    /// Server information received from ServerInfo RPC.
    /// </summary>
    public class ServerInfo
    {
        #region Properties

        /// <summary>
        /// Server hostname.
        /// </summary>
        public string Hostname { get; set; } = string.Empty;

        /// <summary>
        /// Current player count.
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// Maximum players.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Server FPS.
        /// </summary>
        public float Fps { get; set; }

        /// <summary>
        /// In-game time.
        /// </summary>
        public float GameTime { get; set; }

        /// <summary>
        /// Server uptime in seconds.
        /// </summary>
        public float Uptime { get; set; }

        /// <summary>
        /// Map size.
        /// </summary>
        public int MapSize { get; set; }

        /// <summary>
        /// Map seed.
        /// </summary>
        public int MapSeed { get; set; }

        #endregion
    }
}
