// ════════════════════════════════════════════════════════════════════
// MainWindow.xaml.cs - Main window with debug panel
// ════════════════════════════════════════════════════════════════════

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using RustControlPanel.ViewModels;

namespace RustControlPanel.Views.Windows
{
    public partial class MainWindow : Window
    {
        private bool _isDebugVisible = false;
        private ScrollViewer? _debugScrollViewer;
        private bool _isUserScrolling = false;
        private const double DebugPanelHeight = 200;

        public MainWindow()
        {
            InitializeComponent();

            // Find ScrollViewer in debug panel
            this.Loaded += (s, e) =>
            {
                _debugScrollViewer = FindScrollViewer(DebugPanel);
                if (_debugScrollViewer != null)
                {
                    _debugScrollViewer.ScrollChanged += OnDebugScrollChanged;
                }
            };

            // Subscribe to ViewModel
            if (DataContext is MainViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(vm.ShowDebugPanel))
                    {
                        ToggleDebugPanel();
                    }
                    // Auto-scroll on DebugLog change
                    else if (e.PropertyName == nameof(vm.DebugLog))
                    {
                        AutoScrollDebug();
                    }
                };
            }
        }

        #region Window Controls

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnMaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaximizeButton.Content = "☐";
            }
            else
            {
                WindowState = WindowState.Maximized;
                MaximizeButton.Content = "❐";
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Navigation

        private void OnMapClicked(object sender, EventArgs e)
        {
            NavigateToMap();
        }

        private void OnStatsClicked(object sender, EventArgs e)
        {
            NavigateToStats();
        }

        private void OnPlayersClicked(object sender, EventArgs e)
        {
            NavigateToPlayers();
        }

        private void OnConsoleClicked(object sender, EventArgs e)
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

        #region Debug Panel

        private void ToggleDebugPanel()
        {
            if (DataContext is MainViewModel vm)
            {
                _isDebugVisible = vm.ShowDebugPanel;
                AnimateDebugPanel(_isDebugVisible ? DebugPanelHeight : 0);
            }
        }

        private void AnimateDebugPanel(double targetHeight)
        {
            var animation = new DoubleAnimation
            {
                To = targetHeight,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            DebugPanel.BeginAnimation(HeightProperty, animation);
        }

        private void OnClearDebugClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ClearDebugCommand?.Execute(null);
            }
        }

        private void OnCloseDebugClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ShowDebugPanel = false;
            }
        }

        private void OnDebugScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_debugScrollViewer == null) return;

            // User scrolled manually
            if (e.ExtentHeightChange == 0)
            {
                // Check if at bottom
                var atBottom = Math.Abs(_debugScrollViewer.VerticalOffset - _debugScrollViewer.ScrollableHeight) < 1.0;
                _isUserScrolling = !atBottom;
            }
        }

        private void AutoScrollDebug()
        {
            if (_debugScrollViewer == null || _isUserScrolling) return;

            // Scroll to bottom
            _debugScrollViewer.Dispatcher.InvokeAsync(() =>
            {
                _debugScrollViewer.ScrollToEnd();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private ScrollViewer? FindScrollViewer(DependencyObject parent)
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is ScrollViewer sv) return sv;

                var result = FindScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }

        #endregion
    }
}
