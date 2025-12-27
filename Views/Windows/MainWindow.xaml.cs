// ════════════════════════════════════════════════════════════════════
// MainWindow.xaml.cs - Main window code-behind (FIXED)
// ════════════════════════════════════════════════════════════════════

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(125, 133, 144))
            };
        }

        private void NavigateToStats()
        {
            PageContent.Content = new System.Windows.Controls.TextBlock
            {
                Text = "📊 Stats Page",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(125, 133, 144))
            };
        }

        private void NavigateToPlayers()
        {
            PageContent.Content = new System.Windows.Controls.TextBlock
            {
                Text = "👥 Players Page",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(125, 133, 144))
            };
        }

        private void NavigateToConsole()
        {
            PageContent.Content = new System.Windows.Controls.TextBlock
            {
                Text = "💻 Console Page",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(125, 133, 144))
            };
        }

        #endregion
    }
}
