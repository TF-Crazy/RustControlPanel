// ════════════════════════════════════════════════════════════════════
// MapEntity.cs - Entity displayed on the map
// ════════════════════════════════════════════════════════════════════

namespace RustControlPanel.Models
{
    /// <summary>
    /// Types of entities that can be displayed on the map.
    /// </summary>
    public enum MapEntityType
    {
        ActivePlayer = 0,
        SleepingPlayer = 1,
        PatrolHelicopter = 2,
        Chinook = 3,
        CargoShip = 4,
        Bradley = 5,
        Airdrop = 6,
        LockedCrate = 7,
        Minicopter = 8,
        ScrapHelicopter = 9,
        RHIB = 10,
        ModularCar = 11
    }

    /// <summary>
    /// Represents an entity displayed on the map.
    /// </summary>
    public class MapEntity
    {
        /// <summary>
        /// Entity ID.
        /// </summary>
        public ulong EntityId { get; set; }

        /// <summary>
        /// SteamID (for players).
        /// </summary>
        public ulong SteamId { get; set; }

        /// <summary>
        /// Entity type.
        /// </summary>
        public MapEntityType Type { get; set; }

        /// <summary>
        /// Display label (player name, entity name).
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Normalized X position (0.0 to 1.0).
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Normalized Y position (0.0 to 1.0).
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Rotation in degrees (0-360).
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Whether player is online (for players).
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// Whether player is dead (for players).
        /// </summary>
        public bool IsDead { get; set; }

        /// <summary>
        /// Position as formatted string.
        /// </summary>
        public string Position => $"({X:F2}, {Y:F2})";
    }
}
