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

        private void OpenDebug_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var debugWin = new DebugWindow();
            debugWin.Show();
        }
    }
}