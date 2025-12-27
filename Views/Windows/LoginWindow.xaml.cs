// ════════════════════════════════════════════════════════════════════
// LoginWindow.xaml.cs - Login window code-behind with placeholders
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

            // Initialize placeholders
            UpdatePlaceholders();
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

        private void OnHostTextChanged(object sender, TextChangedEventArgs e)
        {
            if (HostPlaceholder != null)
            {
                HostPlaceholder.Visibility = string.IsNullOrEmpty(HostTextBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void OnPortTextChanged(object sender, TextChangedEventArgs e)
        {
            if (PortPlaceholder != null)
            {
                PortPlaceholder.Visibility = string.IsNullOrEmpty(PortTextBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                if (PasswordPlaceholder != null)
                {
                    PasswordPlaceholder.Visibility = string.IsNullOrEmpty(pb.Password)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }

                if (DataContext is LoginViewModel vm)
                {
                    vm.Password = pb.Password;
                }
            }
        }

        private void UpdatePlaceholders()
        {
            if (HostPlaceholder != null)
            {
                HostPlaceholder.Visibility = string.IsNullOrEmpty(HostTextBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            if (PortPlaceholder != null)
            {
                PortPlaceholder.Visibility = string.IsNullOrEmpty(PortTextBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            if (PasswordPlaceholder != null && PasswordBox != null)
            {
                PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
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