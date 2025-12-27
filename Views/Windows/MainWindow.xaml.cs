// ════════════════════════════════════════════════════════════════════
// MainWindow.xaml.cs - Main window with debug panel
// ════════════════════════════════════════════════════════════════════

using RustControlPanel.Core.Utils;
using RustControlPanel.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RustControlPanel.Views.Windows
{
    public partial class MainWindow : Window
    {
        private bool _isDebugVisible = false;
        private ScrollViewer? _debugScrollViewer;
        private bool _isUserScrolling = false;
        private const double DebugPanelHeight = 200;
        private string _currentFilter = "All";

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                _debugScrollViewer = DebugScrollViewer;
                if (_debugScrollViewer != null)
                {
                    _debugScrollViewer.ScrollChanged += OnDebugScrollChanged;
                }
            };

            if (DataContext is MainViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(vm.ShowDebugPanel))
                    {
                        ToggleDebugPanel();
                    }
                };

                // Subscribe directly to Logger
                Logger.Instance.LogEntryAdded += OnLogEntryAdded;
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

        private void OnCloseDebugClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ShowDebugPanel = false;
            }
        }

        private void OnLogEntryAdded(object? sender, string logMessage)
        {
            Dispatcher.InvokeAsync(() =>
            {
                // Parse log: [HH:mm:ss.fff] [LEVEL  ] Message
                var parts = logMessage.Split(new[] { "] [", "] " }, StringSplitOptions.None);
                if (parts.Length < 3) return;

                var timestamp = parts[0].Replace("[", "").Substring(9); // Keep only HH:mm:ss
                var level = parts[1].Trim();
                var message = string.Join("] ", parts.Skip(2));

                // Filter check
                if (_currentFilter != "All" && !level.Contains(_currentFilter.ToUpper()))
                    return;

                // Color based on level
                var color = level switch
                {
                    "DEBUG  " => Colors.Gray,
                    "INFO   " => Colors.LightGreen,
                    "WARNING" => Colors.Orange,
                    "ERROR  " => Colors.Red,
                    "CRITICAL" => Colors.DarkRed,
                    _ => Colors.White
                };

                // Add to RichTextBox
                var paragraph = DebugTextBox.Document.Blocks.LastBlock as Paragraph;
                if (paragraph == null)
                {
                    paragraph = new Paragraph();
                    DebugTextBox.Document.Blocks.Add(paragraph);
                }

                // Timestamp (gray)
                paragraph.Inlines.Add(new Run($"[{timestamp}] ") { Foreground = new SolidColorBrush(Colors.DarkGray) });

                // Level (colored)
                paragraph.Inlines.Add(new Run($"[{level}] ") { Foreground = new SolidColorBrush(color), FontWeight = FontWeights.Bold });

                // Message
                paragraph.Inlines.Add(new Run(message + "\n") { Foreground = new SolidColorBrush(Colors.LightGray) });

                // Limit to 500 lines
                while (DebugTextBox.Document.Blocks.Count > 500)
                {
                    DebugTextBox.Document.Blocks.Remove(DebugTextBox.Document.Blocks.FirstBlock);
                }

                // Auto-scroll
                if (!_isUserScrolling && _debugScrollViewer != null)
                {
                    _debugScrollViewer.ScrollToEnd();
                }
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void OnDebugScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_debugScrollViewer == null) return;

            if (e.ExtentHeightChange == 0)
            {
                var atBottom = Math.Abs(_debugScrollViewer.VerticalOffset - _debugScrollViewer.ScrollableHeight) < 1.0;
                _isUserScrolling = !atBottom;
            }
        }

        private void OnClearDebugClick(object sender, RoutedEventArgs e)
        {
            DebugTextBox.Document.Blocks.Clear();
        }

        // Filter clicks
        private void OnFilterAllClick(object sender, MouseButtonEventArgs e)
        {
            SetFilter("All", FilterAll);
        }

        private void OnFilterDebugClick(object sender, MouseButtonEventArgs e)
        {
            SetFilter("Debug", FilterDebug);
        }

        private void OnFilterInfoClick(object sender, MouseButtonEventArgs e)
        {
            SetFilter("Info", FilterInfo);
        }

        private void OnFilterWarningClick(object sender, MouseButtonEventArgs e)
        {
            SetFilter("Warning", FilterWarning);
        }

        private void OnFilterErrorClick(object sender, MouseButtonEventArgs e)
        {
            SetFilter("Error", FilterError);
        }

        private void SetFilter(string filter, Border activeTab)
        {
            _currentFilter = filter;

            // Reset all tabs
            FilterAll.Background = Brushes.Transparent;
            FilterAll.BorderBrush = Brushes.Transparent;
            FilterDebug.Background = Brushes.Transparent;
            FilterDebug.BorderBrush = Brushes.Transparent;
            FilterInfo.Background = Brushes.Transparent;
            FilterInfo.BorderBrush = Brushes.Transparent;
            FilterWarning.Background = Brushes.Transparent;
            FilterWarning.BorderBrush = Brushes.Transparent;
            FilterError.Background = Brushes.Transparent;
            FilterError.BorderBrush = Brushes.Transparent;

            // Set active
            activeTab.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2800D9FF"));
            activeTab.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D9FF"));

            // Rebuild log with filter
            RebuildLogWithFilter();
        }

        private void RebuildLogWithFilter()
        {
            // Clear and rebuild from ViewModel.DebugLog if needed
            // For now, just clear - new logs will be filtered automatically
            DebugTextBox.Document.Blocks.Clear();
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
