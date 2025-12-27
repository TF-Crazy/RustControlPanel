// ════════════════════════════════════════════════════════════════════
// Monument.cs - Monument position and info
// ════════════════════════════════════════════════════════════════════

namespace RustControlPanel.Models
{
    /// <summary>
    /// Represents a monument on the Rust map.
    /// </summary>
    public class Monument
    {
        /// <summary>
        /// Monument name (e.g., "Dome", "Launch Site").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Display label (same as Name, for compatibility).
        /// </summary>
        public string Label => Name;

        /// <summary>
        /// Normalized X position (0.0 to 1.0).
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Normalized Y position (0.0 to 1.0).
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Creates a new Monument instance.
        /// </summary>
        public Monument()
        {
        }

        /// <summary>
        /// Creates a new Monument with specified values.
        /// </summary>
        public Monument(string name, float x, float y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }
}
