using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RustControlPanel.Models;
using RustControlPanel.Services;
using RustControlPanel.Core.Network;

namespace RustControlPanel.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ConfigService _configService;
        private BridgeClient? _currentClient; // Autoriser le null car on n'est pas tjs connecté
        private ServerConfig? _selectedServer; // Idem
        private string _connectionStatus = "Déconnecté";

        public ObservableCollection<ServerConfig> Servers { get; }
        public System.Windows.Input.ICommand ConnectCommand { get; }

        public ServerConfig? SelectedServer
        {
            get => _selectedServer;
            set => SetProperty(ref _selectedServer, value);
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        public MainViewModel()
        {
            LoggerService.Log("Initialisation du MainViewModel...");
            _configService = new ConfigService();

            try
            {
                var loadedServers = _configService.LoadServers();
                Servers = [.. loadedServers];

                // Sécurité : n'assigner que si la liste n'est pas vide
                if (Servers.Count > 0)
                {
                    SelectedServer = Servers[0];
                    LoggerService.Log($"Serveur par défaut sélectionné : {SelectedServer.Name}");
                }

                ConnectCommand = new RelayCommand(async () => await ConnectAsync());
                LoggerService.Log("MainViewModel prêt.");
            }
            catch (Exception ex)
            {
                LoggerService.Error("Erreur fatale dans le constructeur MainViewModel", ex);
                throw; // On laisse remonter pour que App.xaml.cs le logge aussi
            }
        }

        /// <summary>
        /// Initialise la connexion au serveur sélectionné
        /// </summary>
        public async Task ConnectAsync()
        {
            if (SelectedServer == null) return;

            if (_currentClient != null)
            {
                await _currentClient.DisconnectAsync();
                _currentClient.Dispose();
            }

            ConnectionStatus = $"Connexion à {SelectedServer.Name}...";
            _currentClient = new BridgeClient(SelectedServer.ConnectionUri);
            _currentClient.OnConnected += () => ConnectionStatus = "Connecté";
            _currentClient.OnDisconnected += (r) => ConnectionStatus = $"Déconnecté : {r}";
            _currentClient.OnError += (ex) => ConnectionStatus = $"Erreur : {ex.Message}";

            try
            {
                await _currentClient.ConnectAsync();
            }
            catch (Exception)
            {
                ConnectionStatus = "Échec de connexion";
            }
        }

        /// <summary>
        /// Ajoute un nouveau serveur et sauvegarde la config
        /// </summary>
        public void AddServer(ServerConfig config)
        {
            Servers.Add(config);
            _configService.SaveServers([.. Servers]);
        }
    }
}