// ════════════════════════════════════════════════════════════════════
// MainWindow.xaml.cs - Main window code-behind
// ════════════════════════════════════════════════════════════════════

using System;
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

        #endregion
    }
}
