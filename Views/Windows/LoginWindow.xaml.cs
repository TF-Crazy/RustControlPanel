// ════════════════════════════════════════════════════════════════════
// LoginWindow.xaml.cs - V3 FIXED
// ════════════════════════════════════════════════════════════════════

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

        private void OnConnectClick(object sender, RoutedEventArgs e)
        {
            // Validation
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

            // Call ViewModel connect
            if (DataContext is LoginViewModel vm)
            {
                vm.ConnectCommand.Execute(PasswordBox);
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorPopup.Visibility = Visibility.Visible;
            
            // Animation fade in
            var fadeIn = new DoubleAnimation(0, 1, System.TimeSpan.FromMilliseconds(200));
            ErrorPopup.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void OnErrorOkClick(object sender, RoutedEventArgs e)
        {
            // Animation fade out
            var fadeOut = new DoubleAnimation(1, 0, System.TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (s, a) => ErrorPopup.Visibility = Visibility.Collapsed;
            ErrorPopup.BeginAnimation(OpacityProperty, fadeOut);
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

        private void OnPortPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$");
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
            if (HostPlaceholder != null && HostTextBox != null)
            {
                HostPlaceholder.Visibility = string.IsNullOrEmpty(HostTextBox.Text) 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
            
            if (PortPlaceholder != null && PortTextBox != null)
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
    }
}
