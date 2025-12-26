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
        /// Handles PasswordBox changes to update the ViewModel.
        /// </summary>
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }

        #endregion
    }
}
