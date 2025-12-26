// ════════════════════════════════════════════════════════════════════
// Logger.cs - Centralized logging singleton
// ════════════════════════════════════════════════════════════════════

using System;
using System.IO;
using System.Text;

namespace RustControlPanel.Core.Utils
{
    /// <summary>
    /// Log level enumeration.
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Centralized logging system (Singleton).
    /// Writes logs to both console and file with timestamp and level.
    /// </summary>
    public sealed class Logger
    {
        #region Singleton

        private static readonly Lazy<Logger> _instance = new(() => new Logger());

        /// <summary>
        /// Gets the singleton instance of the Logger.
        /// </summary>
        public static Logger Instance => _instance.Value;

        #endregion

        #region Fields

        private readonly string _logFilePath;
        private readonly object _lock = new();

        #endregion

        #region Events

        /// <summary>
        /// Fired when a log entry is added.
        /// </summary>
        public event EventHandler<string>? LogEntryAdded;

        #endregion

        #region Constructor

        private Logger()
        {
            // Log file in application directory
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            _logFilePath = Path.Combine(appDir, "RustControlPanel.log");

            // Create new log file on each app start
            try
            {
                File.WriteAllText(_logFilePath, $"=== Rust Control Panel - Log Started at {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}");
            }
            catch
            {
                // Silently fail if can't write to log file
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Debug(string message) => Log(LogLevel.Debug, message);

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Info(string message) => Log(LogLevel.Info, message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Warning(string message) => Log(LogLevel.Warning, message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message) => Log(LogLevel.Error, message);

        /// <summary>
        /// Logs an error with exception details.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="ex">Exception to log</param>
        public void Error(string message, Exception ex) => Log(LogLevel.Error, $"{message} | Exception: {ex.Message}\n{ex.StackTrace}");

        /// <summary>
        /// Logs a critical message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Critical(string message) => Log(LogLevel.Critical, message);

        /// <summary>
        /// Logs a critical message with exception details.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="ex">Exception to log</param>
        public void Critical(string message, Exception ex) => Log(LogLevel.Critical, $"{message} | Exception: {ex.Message}\n{ex.StackTrace}");

        #endregion

        #region Private Methods

        /// <summary>
        /// Internal logging method.
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="message">Message to log</param>
        private void Log(LogLevel level, string message)
        {
            lock (_lock)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"[{timestamp}] [{level,-8}] {message}";

                // Write to console
                Console.WriteLine(logEntry);

                // Write to file
                try
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
                catch
                {
                    // Silently fail if can't write to log file
                }

                // Fire event for debug window
                LogEntryAdded?.Invoke(this, logEntry);
            }
        }

        #endregion
    }
}
