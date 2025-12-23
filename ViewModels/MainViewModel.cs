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
        private readonly Services.ConfigService _configService;
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
            _configService = new ConfigService();
            try
            {
                var loaded = _configService.LoadServers();
                Servers = loaded != null ? new ObservableCollection<ServerConfig>(loaded) : [];

                if (Servers.Count > 0) SelectedServer = Servers[0];

                ConnectCommand = new RelayCommand(async () => await ConnectAsync());
            }
            catch (Exception ex)
            {
                // On logge mais on ne bloque pas l'UI
                LoggerService.Error("Erreur dans le constructeur MainViewModel", ex);
                Servers = [];
                ConnectCommand = new RelayCommand(() => { });
            }
        }

        /// <summary>
        /// Initialise la connexion au serveur sélectionné
        /// </summary>
        public async Task ConnectAsync()
        {
            if (SelectedServer == null) return;

            LogService.Write($"Tentative de connexion vers {SelectedServer.Ip}...");
            ConnectionStatus = "Connexion...";

            _currentClient = new BridgeClient(SelectedServer.ConnectionUri);

            // On lie les événements du client aux logs de debug
            _currentClient.OnConnected += () => {
                ConnectionStatus = "Connecté";
                LogService.Write("WebSocket ouvert avec succès.", "SUCCESS");
            };

            _currentClient.OnError += (ex) => {
                LogService.Write($"Erreur socket: {ex.Message}", "ERROR");
            };

            try
            {
                await _currentClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                LogService.Write($"Échec: {ex.Message}", "ERROR");
                ConnectionStatus = "Erreur";
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