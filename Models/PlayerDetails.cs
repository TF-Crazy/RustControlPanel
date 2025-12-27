// ════════════════════════════════════════════════════════════════════
// PlayerDetails.cs - Detailed player information
// ════════════════════════════════════════════════════════════════════

using System.Collections.Generic;

namespace RustControlPanel.Models
{
    /// <summary>
    /// Detailed information about a player.
    /// </summary>
    public class PlayerDetails
    {
        /// <summary>
        /// SteamID (64-bit).
        /// </summary>
        public ulong SteamId { get; set; }

        /// <summary>
        /// Player display name.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Whether player is currently online.
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// Whether player is dead.
        /// </summary>
        public bool IsDead { get; set; }

        /// <summary>
        /// Player health (0-100).
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        /// Player ping in milliseconds.
        /// </summary>
        public int Ping { get; set; }

        /// <summary>
        /// Player position (normalized 0-1).
        /// </summary>
        public string Position { get; set; } = "0.0, 0.0";

        /// <summary>
        /// Player rotation in degrees.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Team ID (0 if no team).
        /// </summary>
        public ulong TeamId { get; set; }

        /// <summary>
        /// Whether player has a team.
        /// </summary>
        public bool HasTeam => TeamId > 0;

        /// <summary>
        /// Whether player is team leader.
        /// </summary>
        public bool IsTeamLeader { get; set; }

        /// <summary>
        /// List of teammate SteamIDs.
        /// </summary>
        public List<ulong> TeamMembers { get; set; } = new();

        /// <summary>
        /// Connection time in seconds.
        /// </summary>
        public int ConnectedSeconds { get; set; }

        /// <summary>
        /// Entity ID on the map.
        /// </summary>
        public ulong EntityId { get; set; }
    }
}
