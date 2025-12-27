// ════════════════════════════════════════════════════════════════════
// MainWindow.xaml.cs - Main window code-behind (FIXED)
// ════════════════════════════════════════════════════════════════════

using System;
using System.Windows;
using System.Windows.Input;

namespace RustControlPanel.Views.Windows
{
    /// <summary>
    /// Main application window
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            
            // Load default page
            NavigateToStats();
        }

        #endregion

        #region TitleBar

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnMaximizeClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                MaximizeButton.Content = "☐";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                MaximizeButton.Content = "❐";
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Navigation

        private void OnNavigateToMap(object? sender, EventArgs e)
        {
            NavigateToMap();
        }

        private void OnNavigateToStats(object? sender, EventArgs e)
        {
            NavigateToStats();
        }

        private void OnNavigateToPlayers(object? sender, EventArgs e)
        {
            NavigateToPlayers();
        }

        private void OnNavigateToConsole(object? sender, EventArgs e)
        {
            NavigateToConsole();
        }

        private void NavigateToMap()
        {
            PageContent.Content = new System.Windows.Controls.TextBlock
            {
                Text = "🗺️ Map Page",
                Style = (Style)FindResource("Heading2"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("ThemeTextMuted")
            };
        }

        private void NavigateToStats()
        {
            PageContent.Content = new System.Windows.Controls.TextBlock
            {
                Text = "📊 Stats Page",
                Style = (Style)FindResource("Heading2"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("ThemeTextMuted")
            };
        }

        private void NavigateToPlayers()
        {
            PageContent.Content = new System.Windows.Controls.TextBlock
            {
                Text = "👥 Players Page",
                Style = (Style)FindResource("Heading2"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("ThemeTextMuted")
            };
        }

        private void NavigateToConsole()
        {
            PageContent.Content = new System.Windows.Controls.TextBlock
            {
                Text = "💻 Console Page",
                Style = (Style)FindResource("Heading2"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("ThemeTextMuted")
            };
        }

        #endregion
    }
}
