// ════════════════════════════════════════════════════════════════════
// Sidebar.xaml.cs - Collapsible sidebar with animations
// ════════════════════════════════════════════════════════════════════

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace RustControlPanel.Views.Components
{
    public partial class Sidebar : UserControl
    {
        private bool _isExpanded = true;
        private const double ExpandedWidth = 200;
        private const double CollapsedWidth = 64;

        public event EventHandler? MapClicked;
        public event EventHandler? StatsClicked;
        public event EventHandler? PlayersClicked;
        public event EventHandler? ConsoleClicked;

        public string SelectedTab { get; set; } = "Map";

        public Sidebar()
        {
            InitializeComponent();
            Width = ExpandedWidth;
            UpdateSelectedTab();
        }

        private void OnToggleClick(object sender, RoutedEventArgs e)
        {
            _isExpanded = !_isExpanded;
            AnimateWidth(_isExpanded ? ExpandedWidth : CollapsedWidth);
            UpdateVisibility();
        }

        private void AnimateWidth(double targetWidth)
        {
            var animation = new DoubleAnimation
            {
                To = targetWidth,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            BeginAnimation(WidthProperty, animation);
        }

        private void UpdateVisibility()
        {
            var visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;
            
            if (MapText != null) MapText.Visibility = visibility;
            if (StatsText != null) StatsText.Visibility = visibility;
            if (PlayersText != null) PlayersText.Visibility = visibility;
            if (ConsoleText != null) ConsoleText.Visibility = visibility;
            if (BottomInfo != null) BottomInfo.Visibility = visibility;
        }

        private void OnMapClick(object sender, MouseButtonEventArgs e)
        {
            SelectedTab = "Map";
            UpdateSelectedTab();
            MapClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnStatsClick(object sender, MouseButtonEventArgs e)
        {
            SelectedTab = "Stats";
            UpdateSelectedTab();
            StatsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnPlayersClick(object sender, MouseButtonEventArgs e)
        {
            SelectedTab = "Players";
            UpdateSelectedTab();
            PlayersClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnConsoleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedTab = "Console";
            UpdateSelectedTab();
            ConsoleClicked?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateSelectedTab()
        {
            // Reset all
            if (MapBorder != null) MapBorder.Background = System.Windows.Media.Brushes.Transparent;
            if (StatsBorder != null) StatsBorder.Background = System.Windows.Media.Brushes.Transparent;
            if (PlayersBorder != null) PlayersBorder.Background = System.Windows.Media.Brushes.Transparent;
            if (ConsoleBorder != null) ConsoleBorder.Background = System.Windows.Media.Brushes.Transparent;

            // Highlight selected
            var selectedBrush = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(40, 0, 217, 255)); // #2800D9FF

            switch (SelectedTab)
            {
                case "Map":
                    if (MapBorder != null) MapBorder.Background = selectedBrush;
                    break;
                case "Stats":
                    if (StatsBorder != null) StatsBorder.Background = selectedBrush;
                    break;
                case "Players":
                    if (PlayersBorder != null) PlayersBorder.Background = selectedBrush;
                    break;
                case "Console":
                    if (ConsoleBorder != null) ConsoleBorder.Background = selectedBrush;
                    break;
            }
        }
    }
}
