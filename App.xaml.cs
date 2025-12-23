using System.Windows;
using RustControlPanel.ViewModels;

namespace RustControlPanel
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            // On injecte le ViewModel principal comme contexte de données
            mainWindow.DataContext = new MainViewModel();
            mainWindow.Show();
        }
    }
}