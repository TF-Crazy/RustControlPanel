using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RustControlPanel.Views.Components
{
    public partial class Sidebar : UserControl
    {
        public event EventHandler? MapClicked;
        public event EventHandler? StatsClicked;
        public event EventHandler? PlayersClicked;
        public event EventHandler? ConsoleClicked;

        private bool _isCollapsed = false;

        public Sidebar()
        {
            InitializeComponent();
            SetActiveTab(0);
        }

        private void OnToggleClick(object sender, RoutedEventArgs e)
        {
            _isCollapsed = !_isCollapsed;
            this.Width = _isCollapsed ? 64 : 200;
            UpdateVisibility();
            AnimateChevron(); // ✅ Animate chevron
            AnimateChevron(); // ✅ Animate chevron
        }

        /// <summary>
        /// Animates chevron rotation (0° expanded, 180° collapsed).
        /// </summary>
        private void AnimateChevron()
        {
            if (ChevronRotate != null)
            {
                var animation = new DoubleAnimation
                {
                    To = _isCollapsed ? 180 : 0,
                    Duration = TimeSpan.FromMilliseconds(250),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                ChevronRotate.BeginAnimation(RotateTransform.AngleProperty, animation);
            }
        }

        private void UpdateVisibility()
        {
            var visibility = _isCollapsed ? Visibility.Collapsed : Visibility.Visible;
            if (MapText != null) MapText.Visibility = visibility;
            if (StatsText != null) StatsText.Visibility = visibility;
            if (PlayersText != null) PlayersText.Visibility = visibility;
            if (ConsoleText != null) ConsoleText.Visibility = visibility;
        }

        private void OnMapClick(object sender, MouseButtonEventArgs e)
        {
            SetActiveTab(0);
            MapClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnStatsClick(object sender, MouseButtonEventArgs e)
        {
            SetActiveTab(1);
            StatsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnPlayersClick(object sender, MouseButtonEventArgs e)
        {
            SetActiveTab(2);
            PlayersClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnConsoleClick(object sender, MouseButtonEventArgs e)
        {
            SetActiveTab(3);
            ConsoleClicked?.Invoke(this, EventArgs.Empty);
        }

        private void SetActiveTab(int index)
        {
            if (MapBorder != null) MapBorder.Background = System.Windows.Media.Brushes.Transparent;
            if (StatsBorder != null) StatsBorder.Background = System.Windows.Media.Brushes.Transparent;
            if (PlayersBorder != null) PlayersBorder.Background = System.Windows.Media.Brushes.Transparent;
            if (ConsoleBorder != null) ConsoleBorder.Background = System.Windows.Media.Brushes.Transparent;

            var activeBrush = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2800D9FF"));
            
            switch (index)
            {
                case 0: if (MapBorder != null) MapBorder.Background = activeBrush; break;
                case 1: if (StatsBorder != null) StatsBorder.Background = activeBrush; break;
                case 2: if (PlayersBorder != null) PlayersBorder.Background = activeBrush; break;
                case 3: if (ConsoleBorder != null) ConsoleBorder.Background = activeBrush; break;
            }
        }
    }
}

        /// <summary>
        /// Animates chevron rotation (0° expanded, 180° collapsed).
        /// </summary>
        private void AnimateChevron()
        {
            if (ChevronRotate != null)
            {
                var animation = new DoubleAnimation
                {
                    To = _isCollapsed ? 180 : 0,
                    Duration = TimeSpan.FromMilliseconds(250),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                ChevronRotate.BeginAnimation(RotateTransform.AngleProperty, animation);
            }
        }
