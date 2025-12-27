// ════════════════════════════════════════════════════════════════════
// LoginWindow.xaml.cs - Login window code-behind
// ════════════════════════════════════════════════════════════════════

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RustControlPanel.Models;
using RustControlPanel.ViewModels;

namespace RustControlPanel.Views.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }

        private void OnServerDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is FrameworkElement element)
            {
                if (element.DataContext is ServerConfig config)
                {
                    vm.QuickConnectCommand.Execute(config);
                }
            }
        }
    }
}
