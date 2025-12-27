// ════════════════════════════════════════════════════════════════════
// MainWindow.xaml.cs - Main window with debug panel
// ════════════════════════════════════════════════════════════════════

using RustControlPanel.Core.Utils;
using RustControlPanel.ViewModels;
using System;
using System.Collections.Generic;
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

        private List<LogEntry> _logCache = new List<LogEntry>();

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

                // Subscribe to Logger
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

        /// <summary>
        /// Handles new log entries from Logger.
        /// Parses, caches, and displays with color coding.
        /// </summary>
        private void OnLogEntryAdded(object? sender, string logMessage)
        {
            Dispatcher.InvokeAsync(() =>
            {
                // Parse: [2025-12-27 12:45:30.123] [INFO   ] Message
                var entry = ParseLogEntry(logMessage);
                if (entry == null) return;

                // ✅ Add to cache
                _logCache.Add(entry);

                // Limit cache to 1000 entries
                if (_logCache.Count > 1000)
                {
                    _logCache.RemoveAt(0);
                }

                // Display if matches filter
                if (MatchesFilter(entry))
                {
                    AppendLogEntry(entry);
                }

                // Auto-scroll
                if (!_isUserScrolling && _debugScrollViewer != null)
                {
                    _debugScrollViewer.ScrollToEnd();
                }
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        /// <summary>
        /// Parses log message into LogEntry object.
        /// Format: [YYYY-MM-DD HH:mm:ss.fff] [LEVEL  ] Message
        /// </summary>
        private LogEntry? ParseLogEntry(string logMessage)
        {
            try
            {
                // Split: [timestamp] [level] message
                var firstClose = logMessage.IndexOf(']');
                if (firstClose == -1) return null;

                var timestamp = logMessage.Substring(1, firstClose - 1);
                var rest = logMessage.Substring(firstClose + 1).TrimStart();

                var secondClose = rest.IndexOf(']');
                if (secondClose == -1) return null;

                var level = rest.Substring(1, secondClose - 1).Trim();
                var message = rest.Substring(secondClose + 1).TrimStart();

                // Extract just time (HH:mm:ss)
                var timePart = timestamp.Length >= 19 ? timestamp.Substring(11, 8) : timestamp;

                return new LogEntry
                {
                    Timestamp = timePart,
                    Level = level,
                    Message = message
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if log entry matches current filter.
        /// </summary>
        private bool MatchesFilter(LogEntry entry)
        {
            if (_currentFilter == "All") return true;
            // ✅ FIX: Exact comparison instead of Contains
            return entry.Level.Equals(_currentFilter.ToUpper(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Appends log entry to RichTextBox with color coding.
        /// </summary>
        private void AppendLogEntry(LogEntry entry)
        {
            var paragraph = DebugTextBox.Document.Blocks.LastBlock as Paragraph;
            if (paragraph == null)
            {
                paragraph = new Paragraph { Margin = new Thickness(0) };
                DebugTextBox.Document.Blocks.Add(paragraph);
            }

            // Color based on level
            var color = entry.Level switch
            {
                "DEBUG" => Colors.Gray,
                "INFO" => Colors.LightGreen,
                "WARNING" => Colors.Orange,
                "ERROR" => Colors.Red,
                "CRITICAL" => Colors.DarkRed,
                _ => Colors.White
            };

            // [HH:mm:ss] (gray)
            paragraph.Inlines.Add(new Run($"[{entry.Timestamp}] ")
            {
                Foreground = new SolidColorBrush(Colors.DarkGray)
            });

            // [LEVEL] (colored + bold)
            paragraph.Inlines.Add(new Run($"[{entry.Level}] ")
            {
                Foreground = new SolidColorBrush(color),
                FontWeight = FontWeights.Bold
            });

            // Message
            paragraph.Inlines.Add(new Run(entry.Message + "\n")
            {
                Foreground = new SolidColorBrush(Colors.LightGray)
            });

            // Limit display to 500 lines
            while (DebugTextBox.Document.Blocks.Count > 500)
            {
                DebugTextBox.Document.Blocks.Remove(DebugTextBox.Document.Blocks.FirstBlock);
            }
        }

        /// <summary>
        /// Handles debug scroll changes to detect manual scrolling.
        /// </summary>
        private void OnDebugScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_debugScrollViewer == null) return;

            if (e.ExtentHeightChange == 0)
            {
                var atBottom = Math.Abs(_debugScrollViewer.VerticalOffset - _debugScrollViewer.ScrollableHeight) < 1.0;
                _isUserScrolling = !atBottom;
            }
        }

        /// <summary>
        /// Clears debug log cache and display.
        /// </summary>
        private void OnClearDebugClick(object sender, RoutedEventArgs e)
        {
            _logCache.Clear();
            DebugTextBox.Document.Blocks.Clear();
        }

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

        /// <summary>
        /// Sets active filter and rebuilds log display from cache.
        /// </summary>
        private void SetFilter(string filter, Border activeTab)
        {
            _currentFilter = filter;

            // Reset all tabs
            ResetFilterTabs();

            // Set active tab
            activeTab.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2800D9FF"));
            activeTab.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D9FF"));

            // Rebuild display from cache
            RebuildLogWithFilter();
        }

        /// <summary>
        /// Resets all filter tab styles.
        /// </summary>
        private void ResetFilterTabs()
        {
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
        }

        /// <summary>
        /// Rebuilds log display from cache with current filter.
        /// </summary>
        private void RebuildLogWithFilter()
        {
            DebugTextBox.Document.Blocks.Clear();

            // Display all cached entries that match filter
            foreach (var entry in _logCache)
            {
                if (MatchesFilter(entry))
                {
                    AppendLogEntry(entry);
                }
            }

            // Scroll to bottom
            _debugScrollViewer?.ScrollToEnd();
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

        /// <summary>
        /// Represents a single log entry with parsed components.
        /// </summary>
        private class LogEntry
        {
            public string Timestamp { get; set; } = "";
            public string Level { get; set; } = "";
            public string Message { get; set; } = "";
        }

        #endregion
    }
}
