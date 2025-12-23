using RustControlPanel.Core.Network;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RustControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TestConnection();
        }

        private async void TestConnection()
        {
            var testServerUri = "ws://51.75.118.32:28000/xZDKMJI36tYfmRkPvPwi";
            var client = new BridgeClient(testServerUri);

            client.OnConnected += () => Debug.WriteLine("✅ Connecté à Carbon !");
            client.OnDisconnected += (reason) => Debug.WriteLine($"❌ Déconnecté : {reason}");
            client.OnMessageReceived += (data) => Debug.WriteLine($"📩 Message reçu : {data.Length} octets");
            client.OnError += (ex) => Debug.WriteLine($"⚠️ Erreur : {ex.Message}");

            try
            {
                await client.ConnectAsync();

                // Test d'envoi : Demander la version ou une commande simple
                // ID 1, Methode "GetServerInfo" (Exemple Carbon)
                await client.SendRpcAsync(1, "GetPlayers");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Échec de connexion : {ex.Message}");
            }
        }
    }
}