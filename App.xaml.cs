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
            LogService.Write("Démarrage de l'application...");

            try
            {
                var mainWindow = new RustControlPanel.Views.MainWindow();
                var viewModel = new MainViewModel();

                mainWindow.DataContext = viewModel;
                mainWindow.Show();
                LogService.Write("Interface et ViewModel chargés.");
            }
            catch (Exception)
            {
                LogService.Write("Erreur fatale au chargement de MainWindow");
            }
        }

        public App()
        {
            // On capture les crashs de l'UI
            this.DispatcherUnhandledException += (s, e) =>
            {
                Services.LogService.HandleCrash(e.Exception);
                MessageBox.Show("L'application a rencontré un problème. Un rapport de crash a été généré.", "Crash", MessageBoxButton.OK, MessageBoxImage.Error);
                // On ne met pas e.Handled = true si on veut que l'app se ferme proprement après le crash
            };
        }
    }
}