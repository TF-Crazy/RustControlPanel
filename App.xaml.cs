using System.Windows;
using RustControlPanel.Services;
using RustControlPanel.ViewModels;
using RustControlPanel.Views;
namespace RustControlPanel
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LoggerService.Initialize();
            LoggerService.Log("Démarrage de l'application...");
            base.OnStartup(e);
            try
            {
                var mainWindow = new MainWindow();
                var viewModel = new MainViewModel();
                mainWindow.DataContext = viewModel;

                LoggerService.Log("Interface et ViewModel chargés.");
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                LoggerService.Error("Crash critique au démarrage !", ex);
                MessageBox.Show($"Erreur fatale au démarrage :\n{ex.Message}", "Crash", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        public App()
        {
            // Capture les erreurs XAML / Rendu
            this.DispatcherUnhandledException += (s, e) =>
            {
                LoggerService.Error("ERREUR NON GÉRÉE (UI)", e.Exception);
                MessageBox.Show($"Crash UI : {e.Exception.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true; // Empêche la fermeture immédiate pour voir le message
            };
        }
    }
}