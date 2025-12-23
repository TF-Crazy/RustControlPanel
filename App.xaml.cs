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
            base.OnStartup(e);
            LoggerService.Log("Démarrage de l'application...");

            try
            {
                var mainWindow = new RustControlPanel.Views.MainWindow();
                var viewModel = new MainViewModel();

                mainWindow.DataContext = viewModel;
                mainWindow.Show();
                LoggerService.Log("Interface et ViewModel chargés.");
            }
            catch (Exception ex)
            {
                LoggerService.Error("Erreur fatale au chargement de MainWindow", ex);
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