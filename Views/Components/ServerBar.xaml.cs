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
            var mainWin = Application.Current.MainWindow as MainWindow;
            if (mainWin != null)
            {
                mainWin.DebugPanel.Visibility = Visibility.Visible;
            }
        }
    }
}