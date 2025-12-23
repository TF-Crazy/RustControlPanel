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
            try
            {
                _configService = new ConfigService();
                var loaded = _configService.LoadServers();
                Servers = loaded != null ? new ObservableCollection<ServerConfig>(loaded) : new ObservableCollection<ServerConfig>();

                if (Servers.Count > 0) SelectedServer = Servers[0];

                ConnectCommand = new RelayCommand(async () => await ConnectAsync());
            }
            catch (Exception ex)
            {
                // On logge mais on ne bloque pas l'UI
                LoggerService.Error("Erreur dans le constructeur MainViewModel", ex);
                Servers = new ObservableCollection<ServerConfig>();
                ConnectCommand = new RelayCommand(() => { });
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