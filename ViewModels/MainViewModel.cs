// ════════════════════════════════════════════════════════════════════
// MainViewModel.cs - ViewModel for MainWindow
// ════════════════════════════════════════════════════════════════════

using RustControlPanel.Core.Utils;
using RustControlPanel.Services;
using RustControlPanel.Views.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RustControlPanel.ViewModels
{
    /// <summary>
    /// ViewModel for the main window.
    /// Manages navigation and overall application state.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region Fields

        private int _selectedTabIndex = 0;
        private bool _isConnected = false;
        private string _connectionStatus = "Déconnecté";
        private string _serverName = "Not Connected";
        private int _playerCount = 0;
        private int _maxPlayers = 0;
        private float _serverFps = 0f;
        private int _queueCount = 0;
        private int _joiningCount = 0;
        private double _entityTime = 0;
        private string _gameTime = "00:00";
        private string _uptime = "0d 0h 0m";
        private bool _showDebugPanel = false;
        private string _debugLog = "";
        private int _reconnectCountdown = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Currently selected tab index.
        /// </summary>
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        /// <summary>
        /// Whether connected to a server.
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        /// <summary>
        /// Connection status message.
        /// </summary>
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        /// <summary>
        /// Server hostname.
        /// </summary>
        public string ServerName
        {
            get => _serverName;
            set => SetProperty(ref _serverName, value);
        }

        /// <summary>
        /// Current player count.
        /// </summary>
        public int PlayerCount
        {
            get => _playerCount;
            set => SetProperty(ref _playerCount, value);
        }

        /// <summary>
        /// Maximum players.
        /// </summary>
        public int MaxPlayers
        {
            get => _maxPlayers;
            set => SetProperty(ref _maxPlayers, value);
        }

        /// <summary>
        /// Server FPS.
        /// </summary>
        public float ServerFps
        {
            get => _serverFps;
            set => SetProperty(ref _serverFps, value);
        }

        public int QueueCount
        {
            get => _queueCount;
            set => SetProperty(ref _queueCount, value);
        }

        public int JoiningCount
        {
            get => _joiningCount;
            set => SetProperty(ref _joiningCount, value);
        }

        public double EntityTime
        {
            get => _entityTime;
            set => SetProperty(ref _entityTime, value);
        }

        public string GameTime
        {
            get => _gameTime;
            set => SetProperty(ref _gameTime, value);
        }

        public string Uptime
        {
            get => _uptime;
            set => SetProperty(ref _uptime, value);
        }

        // Debug panel
        public bool ShowDebugPanel
        {
            get => _showDebugPanel;
            set => SetProperty(ref _showDebugPanel, value);
        }

        public string DebugLog
        {
            get => _debugLog;
            set => SetProperty(ref _debugLog, value);
        }

        public int ReconnectCountdown
        {
            get => _reconnectCountdown;
            set => SetProperty(ref _reconnectCountdown, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to disconnect from server.
        /// </summary>
        public ICommand DisconnectCommand { get; }

        /// <summary>
        /// Command to change server (disconnect and return to login).
        /// </summary>
        public ICommand ChangeServerCommand { get; }

        /// <summary>
        /// Command to minimize window.
        /// </summary>
        public ICommand MinimizeCommand { get; }

        /// <summary>
        /// Command to maximize/restore window.
        /// </summary>
        public ICommand MaximizeCommand { get; }

        /// <summary>
        /// Command to close window.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Command to open debug window.
        /// </summary>
        public ICommand OpenDebugCommand { get; }

        /// <summary>
        /// Command to toggle debug panel.
        /// </summary>
        public ICommand ToggleDebugCommand { get; }

        /// <summary>
        /// Command to clear debug log.
        /// </summary>
        public ICommand ClearDebugCommand { get; }

        /// <summary>
        /// Command to reconnect to server.
        /// </summary>
        public ICommand ReconnectCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            // Initialize commands
            DisconnectCommand = new AsyncRelayCommand(ExecuteDisconnectAsync);
            ChangeServerCommand = new AsyncRelayCommand(ExecuteChangeServerAsync);
            MinimizeCommand = new RelayCommand(ExecuteMinimize);
            MaximizeCommand = new RelayCommand(ExecuteMaximize);
            CloseCommand = new RelayCommand(ExecuteClose);
            OpenDebugCommand = new RelayCommand(ExecuteOpenDebug);
            ToggleDebugCommand = new RelayCommand(ExecuteToggleDebug);
            ClearDebugCommand = new RelayCommand(ExecuteClearDebug);
            ReconnectCommand = new AsyncRelayCommand(ExecuteReconnectAsync);

            // Subscribe to connection events
            ConnectionService.Instance.ConnectionStateChanged += OnConnectionStateChanged;

            // Subscribe to server stats updates
            ServerStatsService.Instance.ServerInfoUpdated += OnServerInfoUpdated;

            // Update initial connection state
            IsConnected = ConnectionService.Instance.IsConnected;
            
            // Load server name if connected
            if (IsConnected)
            {
                var config = ConnectionService.Instance.CurrentConfig;
                if (config != null)
                {
                    ServerName = config.DisplayName ?? $"{config.Host}:{config.Port}";
                }
            }

            Logger.Instance.Debug("MainViewModel created");

            // Subscribe to logger
            Logger.Instance.LogEntryAdded += (sender, logMessage) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(DebugLog))
                        DebugLog += Environment.NewLine + logMessage;
                    else
                        DebugLog = logMessage;

                    // Limit 500 lines
                    var lines = DebugLog.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    if (lines.Length > 500)
                        DebugLog = string.Join(Environment.NewLine, lines.Skip(lines.Length - 500));
                });
            };

            Logger.Instance.Info("Debug panel ready");
        }

        #endregion

        #region Command Implementations

        private async System.Threading.Tasks.Task ExecuteDisconnectAsync(object? parameter)
        {
            await ConnectionService.Instance.DisconnectAsync();

            // Return to login window
            Application.Current.Dispatcher.Invoke(() =>
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                
                // Close MainWindow
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            });
        }

        private async System.Threading.Tasks.Task ExecuteChangeServerAsync(object? parameter)
        {
            // Same as disconnect but for clarity
            await ExecuteDisconnectAsync(parameter);
        }

        private void ExecuteMinimize(object? parameter)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void ExecuteMaximize(object? parameter)
        {
            var window = Application.Current.MainWindow;
            window.WindowState = window.WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }

        private void ExecuteClose(object? parameter)
        {
            Application.Current.MainWindow?.Close();
        }

        private void ExecuteOpenDebug(object? parameter)
        {
            // Toggle debug panel instead of opening window
            ShowDebugPanel = !ShowDebugPanel;
        }

        private void ExecuteToggleDebug(object? parameter)
        {
            ShowDebugPanel = !ShowDebugPanel;
        }

        private void ExecuteClearDebug(object? parameter)
        {
            DebugLog = "";
        }

        private async System.Threading.Tasks.Task ExecuteReconnectAsync(object? parameter)
        {
            // Disconnect then reconnect with same config
            var currentConfig = ConnectionService.Instance.CurrentConfig;
            
            if (currentConfig != null)
            {
                await ConnectionService.Instance.DisconnectAsync();
                await System.Threading.Tasks.Task.Delay(1000); // Wait 1 second
                await ConnectionService.Instance.ConnectAsync(currentConfig);
            }
        }

        #endregion

        #region Event Handlers

        private void OnConnectionStateChanged(object? sender, bool isConnected)
        {
            IsConnected = isConnected;

            if (isConnected)
            {
                ConnectionStatus = "Connecté";
                
                // Get server config if available
                var config = ConnectionService.Instance.CurrentConfig;
                if (config != null)
                {
                    ServerName = config.DisplayName ?? $"{config.Host}:{config.Port}";
                }
            }
            else
            {
                ConnectionStatus = "Déconnecté";
                ServerName = "Disconnected";
                PlayerCount = 0;
                MaxPlayers = 0;
                ServerFps = 0f;
            }
        }

        private void OnServerInfoUpdated(object? sender, Models.ServerInfo info)
        {
            // Update server name
            if (!string.IsNullOrEmpty(info.Hostname))
            {
                ServerName = info.Hostname;

                var config = ConnectionService.Instance.CurrentConfig;
                if (config != null && config.DisplayName != info.Hostname)
                {
                    config.DisplayName = info.Hostname;
                    if (config.SaveCredentials)
                    {
                        SettingsService.Instance.AddOrUpdateServer(config);
                    }
                }
            }

            // Update ALL stats
            PlayerCount = info.PlayerCount;
            MaxPlayers = info.MaxPlayers;
            ServerFps = info.Fps;
            QueueCount = info.QueuedPlayers;
            JoiningCount = info.JoiningPlayers;
            EntityTime = info.EntityCount;

            // GameTime - Parse DateTime and show only time since wipe
            // Format: "10/03/1984 11:27:20" → Calculate days/hours since epoch
            if (DateTime.TryParse(info.GameTime, out var gameDateTime))
            {
                var epoch = new DateTime(1984, 3, 10, 0, 0, 0); // Rust epoch
                var elapsed = gameDateTime - epoch;
                var days = (int)elapsed.TotalDays;
                var hours = elapsed.Hours;
                var mins = elapsed.Minutes;
                GameTime = $"{days}d {hours}h {mins}m";
            }
            else
            {
                GameTime = info.GameTime; // Fallback
            }

            // Uptime (convert seconds to "Xd Xh Xm")
            var totalSeconds = info.Uptime;
            var days2 = totalSeconds / 86400;
            var hours2 = (totalSeconds % 86400) / 3600;
            var mins2 = (totalSeconds % 3600) / 60;
            Uptime = $"{days2}d {hours2}h {mins2}m";
        }

        #endregion
    }
}
