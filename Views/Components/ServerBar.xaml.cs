using System.Windows;
using System.Windows.Controls;

namespace RustControlPanel.Views.Components
{
    /// <summary>
    /// Logique d'interaction pour ServerBar.xaml
    /// </summary>
    public partial class ServerBar : UserControl
    {
        public ServerBar()
        {
            InitializeComponent();
        }

        private void OpenDebug_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                mainWin.DebugPanel.Visibility = Visibility.Visible;
            }
        }
    }
}