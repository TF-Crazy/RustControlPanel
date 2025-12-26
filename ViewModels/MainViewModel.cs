// ════════════════════════════════════════════════════════════════════
// MainViewModel.cs - ViewModel for MainWindow
// ════════════════════════════════════════════════════════════════════

using System;
using System.Windows;
using System.Windows.Input;
using RustControlPanel.Core.Utils;
using RustControlPanel.Services;
using RustControlPanel.Views.Windows;

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

        #endregion

        #region Commands

        /// <summary>
        /// Command to disconnect from server.
        /// </summary>
        public ICommand DisconnectCommand { get; }

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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new MainViewModel.
        /// </summary>
        public MainViewModel()
        {
            // Initialize commands
            DisconnectCommand = new AsyncRelayCommand(ExecuteDisconnectAsync);
            MinimizeCommand = new RelayCommand(ExecuteMinimize);
            MaximizeCommand = new RelayCommand(ExecuteMaximize);
            CloseCommand = new RelayCommand(ExecuteClose);

            // Subscribe to connection events
            ConnectionService.Instance.ConnectionStateChanged += OnConnectionStateChanged;

            // Update initial connection state
            IsConnected = ConnectionService.Instance.IsConnected;

            Logger.Instance.Debug("MainViewModel created");
        }

        #endregion

        #region Command Implementations

        private async System.Threading.Tasks.Task ExecuteDisconnectAsync(object? parameter)
        {
            await ConnectionService.Instance.DisconnectAsync();

            // Return to login window
            Application.Current.Dispatcher.Invoke(() =>
            {
                var loginWindow = new Views.Windows.LoginWindow();
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

        #endregion
    }
}
