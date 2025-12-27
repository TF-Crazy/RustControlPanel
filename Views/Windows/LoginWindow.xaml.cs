// ════════════════════════════════════════════════════════════════════
// LoginWindow.xaml.cs - Login window code-behind
// ════════════════════════════════════════════════════════════════════

using System.Windows;
using System.Windows.Controls;
using RustControlPanel.ViewModels;

namespace RustControlPanel.Views.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Constructor

        public LoginWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Allows dragging the window by the titlebar.
        /// </summary>
        private void OnTitleBarMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Handles PasswordBox changes to update the ViewModel.
        /// </summary>
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }

        /// <summary>
        /// Handles double-click on saved server to quick connect.
        /// </summary>
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

        #endregion
    }
}
