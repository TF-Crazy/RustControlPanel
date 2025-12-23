using System.Windows.Controls;
using System.Windows;


namespace RustControlPanel.Views.Components
{
    public partial class DebugPanelView : UserControl
    {
        public DebugPanelView()
        {
            InitializeComponent();
            this.DataContext = new { Entries = Services.LogService.Entries };
        }

        private void CloseDebug_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}