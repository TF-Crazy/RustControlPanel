// ════════════════════════════════════════════════════════════════════
// Sidebar.xaml.cs - Sidebar component code-behind
// ════════════════════════════════════════════════════════════════════

using System;
using System.Windows;
using System.Windows.Controls;

namespace RustControlPanel.Views.Components
{
    public partial class Sidebar : UserControl
    {
        public event EventHandler? MapClicked;
        public event EventHandler? StatsClicked;
        public event EventHandler? PlayersClicked;
        public event EventHandler? ConsoleClicked;

        public Sidebar()
        {
            InitializeComponent();
        }

        private void OnMapClick(object sender, RoutedEventArgs e)
        {
            MapClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnStatsClick(object sender, RoutedEventArgs e)
        {
            StatsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnPlayersClick(object sender, RoutedEventArgs e)
        {
            PlayersClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnConsoleClick(object sender, RoutedEventArgs e)
        {
            ConsoleClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
