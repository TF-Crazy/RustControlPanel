using System.Windows;
using System.Windows.Controls;

namespace RustControlPanel.Views.Components
{
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
            this.DataContext = new { Services.LogService.Entries };
        }
    }
}
