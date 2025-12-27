// ════════════════════════════════════════════════════════════════════
// MapInfo.cs - Map image and world information
// ════════════════════════════════════════════════════════════════════

using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace RustControlPanel.Models
{
    /// <summary>
    /// Contains map image and world information received from the server.
    /// </summary>
    public class MapInfo
    {
        /// <summary>
        /// Map image as BitmapImage for WPF rendering.
        /// </summary>
        public BitmapImage? ImageSource { get; set; }

        /// <summary>
        /// World size in meters (e.g., 4000 for a 4000x4000 map).
        /// </summary>
        public uint WorldSize { get; set; }

        /// <summary>
        /// List of monuments on the map.
        /// </summary>
        public ObservableCollection<Monument> Monuments { get; set; }

        /// <summary>
        /// Creates a new MapInfo instance.
        /// </summary>
        public MapInfo()
        {
            WorldSize = 4000; // Default
            Monuments = new ObservableCollection<Monument>();
        }
    }
}
