// ════════════════════════════════════════════════════════════════════
// MainWindow.xaml.cs - Main window code-behind
// ════════════════════════════════════════════════════════════════════

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RustControlPanel.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Allows dragging the window by the titlebar.
        /// </summary>
        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        #region Theme Switcher (DEV ONLY - Remove after choice)

        private void SwitchToGlass(object sender, RoutedEventArgs e)
        {
            SwitchTheme("GlassMorphism");
        }

        private void SwitchToNeuro(object sender, RoutedEventArgs e)
        {
            SwitchTheme("Neumorphism");
        }

        private void SwitchToFluent(object sender, RoutedEventArgs e)
        {
            SwitchTheme("Fluent");
        }

        private void SwitchTheme(string themeName)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Switching to theme: {themeName}");
                
                var dict = new ResourceDictionary
                {
                    Source = new System.Uri($"pack://application:,,,/Styles/Themes/Theme.{themeName}.xaml", System.UriKind.Absolute)
                };

                System.Diagnostics.Debug.WriteLine($"Theme dictionary loaded from: {dict.Source}");

                // Remove old theme
                var oldTheme = Application.Current.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source?.OriginalString.Contains("/Themes/Theme.") == true);
                
                if (oldTheme != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Removing old theme: {oldTheme.Source}");
                    Application.Current.Resources.MergedDictionaries.Remove(oldTheme);
                }

                // Add new theme
                Application.Current.Resources.MergedDictionaries.Add(dict);
                System.Diagnostics.Debug.WriteLine($"Theme switched successfully to {themeName}");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR switching theme: {ex.Message}");
                MessageBox.Show($"Erreur de changement de thème: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #endregion
    }
}
