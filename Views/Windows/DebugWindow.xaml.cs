// ════════════════════════════════════════════════════════════════════
// DebugWindow.xaml.cs - Debug window code-behind
// ════════════════════════════════════════════════════════════════════

using System;
using System.Text;
using System.Windows;
using RustControlPanel.Core.Utils;

namespace RustControlPanel.Views.Windows
{
    /// <summary>
    /// Debug window for displaying RPC logs.
    /// </summary>
    public partial class DebugWindow : Window
    {
        #region Fields

        private readonly StringBuilder _logBuilder = new();
        private int _logCount = 0;

        #endregion

        #region Constructor

        public DebugWindow()
        {
            InitializeComponent();

            // Subscribe to logger
            Logger.Instance.LogEntryAdded += OnLogEntryAdded;
        }

        #endregion

        #region Event Handlers

        private void OnLogEntryAdded(object? sender, string logEntry)
        {
            // Append to builder
            _logBuilder.AppendLine(logEntry);
            _logCount++;

            // Update UI on dispatcher
            Dispatcher.Invoke(() =>
            {
                LogTextBox.Text = _logBuilder.ToString();
                LogCountText.Text = $"{_logCount} logs";

                // Auto-scroll to bottom
                LogScrollViewer.ScrollToEnd();
            });
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            _logBuilder.Clear();
            _logCount = 0;
            LogTextBox.Text = string.Empty;
            LogCountText.Text = "0 logs";
        }

        #endregion

        #region Window Events

        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe from logger
            Logger.Instance.LogEntryAdded -= OnLogEntryAdded;
            base.OnClosed(e);
        }

        #endregion
    }
}
