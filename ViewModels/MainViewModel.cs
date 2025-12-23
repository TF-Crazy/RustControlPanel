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
        private BridgeClient _currentClient;
        private ServerConfig _selectedServer;
        private string _connectionStatus = "Déconnecté";

        public ObservableCollection<ServerConfig> Servers { get; }
        public System.Windows.Input.ICommand ConnectCommand { get; }

        public ServerConfig SelectedServer
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
            _configService = new ConfigService();
            // Chargement initial des serveurs depuis le JSON
            Servers = new ObservableCollection<ServerConfig>(_configService.LoadServers());
            SelectedServer = Servers.FirstOrDefault();
            ConnectCommand = new RelayCommand(async () => await ConnectAsync());
        }

        /// <summary>
        /// Initialise la connexion au serveur sélectionné
        /// </summary>
        public async Task ConnectAsync()
        {
            if (SelectedServer == null) return;

            // Si un client existe déjà, on le nettoie
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
            catch (Exception ex)
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
            _configService.SaveServers(Servers.ToList());
        }
    }
}