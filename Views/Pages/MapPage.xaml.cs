// ════════════════════════════════════════════════════════════════════
// MapPage.xaml.cs - Map page code-behind
// ════════════════════════════════════════════════════════════════════

using System.Linq;
using System.Windows.Controls;
using RustControlPanel.Models;
using RustControlPanel.ViewModels;

namespace RustControlPanel.Views.Pages
{
    /// <summary>
    /// Map page showing interactive map with players and entities.
    /// </summary>
    public partial class MapPage : UserControl
    {
        public MapPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles entity click on the map canvas.
        /// Selects the player if it's a player entity.
        /// </summary>
        private void OnEntityClicked(object sender, MapEntity entity)
        {
            if (DataContext is not MapViewModel viewModel) return;

            // If clicked entity is a player, select them
            if (entity.Type == MapEntityType.ActivePlayer || 
                entity.Type == MapEntityType.SleepingPlayer)
            {
                // Find player by SteamId
                var player = viewModel.Players.FirstOrDefault(p => p.SteamId == entity.SteamId);
                if (player != null)
                {
                    viewModel.SelectedPlayer = player;
                }
            }
        }
    }
}
