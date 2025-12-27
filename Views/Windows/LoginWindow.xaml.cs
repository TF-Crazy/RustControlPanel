using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
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
                this.DragMove();
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnConnectClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HostTextBox.Text))
            {
                ShowError("Host is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(PortTextBox.Text))
            {
                ShowError("Port is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError("Password is required");
                return;
            }

            if (DataContext is LoginViewModel vm)
            {
                vm.ConnectCommand.Execute(PasswordBox);
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorPopup.Visibility = Visibility.Visible;
            
            var fadeIn = new DoubleAnimation(0, 1, System.TimeSpan.FromMilliseconds(200));
            ErrorPopup.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void OnErrorOkClick(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, System.TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (s, a) => ErrorPopup.Visibility = Visibility.Collapsed;
            ErrorPopup.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void OnPortPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb && DataContext is LoginViewModel vm)
            {
                vm.Password = pb.Password;
            }
        }
    }
}
