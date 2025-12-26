// ════════════════════════════════════════════════════════════════════
// LoginViewModel.cs - ViewModel for LoginWindow
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;
using RustControlPanel.Services;
using RustControlPanel.Views.Windows;

namespace RustControlPanel.ViewModels
{
    /// <summary>
    /// ViewModel for the login window.
    /// Manages connection to Carbon Bridge server.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        #region Fields

        private string _host = "127.0.0.1";
        private int _port = 3050;
        private string _password = string.Empty;
        private bool _saveCredentials = false;
        private bool _isConnecting = false;
        private string? _statusMessage;
        private ServerConfig? _selectedServer;

        #endregion

        #region Properties

        /// <summary>
        /// Server host or IP address.
        /// </summary>
        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        /// <summary>
        /// Server port.
        /// </summary>
        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        /// <summary>
        /// Connection password.
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Whether to save credentials.
        /// </summary>
        public bool SaveCredentials
        {
            get => _saveCredentials;
            set => SetProperty(ref _saveCredentials, value);
        }

        /// <summary>
        /// Whether currently connecting.
        /// </summary>
        public bool IsConnecting
        {
            get => _isConnecting;
            set => SetProperty(ref _isConnecting, value);
        }

        /// <summary>
        /// Status message to display.
        /// </summary>
        public string? StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// Selected saved server.
        /// </summary>
        public ServerConfig? SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (SetProperty(ref _selectedServer, value) && value != null)
                {
                    LoadServerConfig(value);
                }
            }
        }

        /// <summary>
        /// List of saved servers.
        /// </summary>
        public ObservableCollection<ServerConfig> SavedServers { get; }

        #endregion

        #region Commands

        /// <summary>
        /// Command to connect to server.
        /// </summary>
        public ICommand ConnectCommand { get; }

        /// <summary>
        /// Command to remove a saved server.
        /// </summary>
        public ICommand RemoveServerCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new LoginViewModel.
        /// </summary>
        public LoginViewModel()
        {
            // Load saved servers
            SavedServers = new ObservableCollection<ServerConfig>(SettingsService.Instance.SavedServers);

            // Initialize commands
            ConnectCommand = new AsyncRelayCommand(ExecuteConnectAsync, CanExecuteConnect);
            RemoveServerCommand = new RelayCommand(ExecuteRemoveServer);

            // Load last connected server
            var lastHost = SettingsService.Instance.LastConnectedHost;
            if (!string.IsNullOrEmpty(lastHost))
            {
                var lastServer = SavedServers.FirstOrDefault(s => s.Host == lastHost);
                if (lastServer != null)
                {
                    SelectedServer = lastServer;
                }
            }

            Logger.Instance.Debug("LoginViewModel created");
        }

        #endregion

        #region Command Implementations

        private bool CanExecuteConnect(object? parameter)
        {
            return !IsConnecting && !string.IsNullOrWhiteSpace(Host) && Port > 0;
        }

        private async Task ExecuteConnectAsync(object? parameter)
        {
            IsConnecting = true;
            StatusMessage = "Connexion en cours...";

            try
            {
                var config = new ServerConfig
                {
                    Host = Host.Trim(),
                    Port = Port,
                    Password = Password,
                    SaveCredentials = SaveCredentials,
                    DisplayName = string.IsNullOrWhiteSpace(Host) ? $"Server {Port}" : Host
                };

                var success = await ConnectionService.Instance.ConnectAsync(config);

                if (success)
                {
                    StatusMessage = "Connecté !";
                    Logger.Instance.Info("Login successful");

                    // Close login window and open main window
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var mainWindow = new Views.Windows.MainWindow();
                        mainWindow.Show();

                        // Close login window
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is LoginWindow)
                            {
                                window.Close();
                                break;
                            }
                        }
                    });
                }
                else
                {
                    StatusMessage = "Échec de la connexion";
                    MessageBox.Show("Impossible de se connecter au serveur.\nVérifiez l'adresse, le port et que le serveur est bien démarré.",
                        "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Connection error", ex);
                StatusMessage = "Erreur de connexion";
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsConnecting = false;
            }
        }

        private void ExecuteRemoveServer(object? parameter)
        {
            if (parameter is ServerConfig config)
            {
                SavedServers.Remove(config);
                SettingsService.Instance.RemoveServer(config.Host, config.Port);
                Logger.Instance.Info($"Removed saved server: {config.Host}:{config.Port}");
            }
        }

        #endregion

        #region Private Methods

        private void LoadServerConfig(ServerConfig config)
        {
            Host = config.Host;
            Port = config.Port;
            
            // Load password only if credentials were saved
            if (config.SaveCredentials && !string.IsNullOrEmpty(config.Password))
            {
                Password = config.Password;
                SaveCredentials = true;
            }
            else
            {
                Password = string.Empty;
                SaveCredentials = false;
            }
        }

        #endregion
    }
}
