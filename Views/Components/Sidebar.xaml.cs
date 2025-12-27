using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            SetActiveTab(0); // Map par défaut
        }

        private void OnToggleClick(object sender, RoutedEventArgs e)
        {
            _isCollapsed = !_isCollapsed;
            AnimateWidth(_isCollapsed ? CollapsedWidth : ExpandedWidth);
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
            // Reset all
            if (MapBorder != null) MapBorder.Background = Brushes.Transparent;
            if (StatsBorder != null) StatsBorder.Background = Brushes.Transparent;
            if (PlayersBorder != null) PlayersBorder.Background = Brushes.Transparent;
            if (ConsoleBorder != null) ConsoleBorder.Background = Brushes.Transparent;

            // Set active
            var activeBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2800D9FF"));
            switch (index)
            {
                case 0: if (MapBorder != null) MapBorder.Background = activeBrush; break;
                case 1: if (StatsBorder != null) StatsBorder.Background = activeBrush; break;
                case 2: if (PlayersBorder != null) PlayersBorder.Background = activeBrush; break;
                case 3: if (ConsoleBorder != null) ConsoleBorder.Background = activeBrush; break;
            }
        }

        public void SetActive(int index)
        {
            SetActiveTab(index);
        }
    }
}
