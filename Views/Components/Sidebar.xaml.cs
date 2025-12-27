// ════════════════════════════════════════════════════════════════════
// Sidebar.xaml.cs - V3 avec chevron rotate
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
        public event EventHandler? MapClicked;
        public event EventHandler? StatsClicked;
        public event EventHandler? PlayersClicked;
        public event EventHandler? ConsoleClicked;

        private const double ExpandedWidth = 200;
        private const double CollapsedWidth = 64;
        private bool _isCollapsed = false;

        public Sidebar()
        {
            InitializeComponent();
        }

        private void OnToggleClick(object sender, RoutedEventArgs e)
        {
            _isCollapsed = !_isCollapsed;
            AnimateWidth(_isCollapsed ? CollapsedWidth : ExpandedWidth);
            AnimateChevron(_isCollapsed ? 180 : 0);
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
            this.BeginAnimation(WidthProperty, animation);
        }

        private void AnimateChevron(double targetAngle)
        {
            var rotateTransform = (RotateTransform)ChevronIcon.RenderTransform;
            var animation = new DoubleAnimation
            {
                To = targetAngle,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        private void UpdateVisibility()
        {
            var visibility = _isCollapsed ? Visibility.Collapsed : Visibility.Visible;
            
            MapText.Visibility = visibility;
            StatsText.Visibility = visibility;
            PlayersText.Visibility = visibility;
            ConsoleText.Visibility = visibility;
        }

        private void OnMapClick(object sender, MouseButtonEventArgs e)
        {
            UpdateSelectedTab(MapItem);
            MapClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnStatsClick(object sender, MouseButtonEventArgs e)
        {
            UpdateSelectedTab(StatsItem);
            StatsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnPlayersClick(object sender, MouseButtonEventArgs e)
        {
            UpdateSelectedTab(PlayersItem);
            PlayersClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnConsoleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateSelectedTab(ConsoleItem);
            ConsoleClicked?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateSelectedTab(Border selectedItem)
        {
            // Reset all
            MapItem.Background = System.Windows.Media.Brushes.Transparent;
            StatsItem.Background = System.Windows.Media.Brushes.Transparent;
            PlayersItem.Background = System.Windows.Media.Brushes.Transparent;
            ConsoleItem.Background = System.Windows.Media.Brushes.Transparent;

            // Highlight selected
            selectedItem.Background = (System.Windows.Media.Brush)Application.Current.Resources["ThemeAccentBackground"];
        }
    }
}
