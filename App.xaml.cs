// ════════════════════════════════════════════════════════════════════
// App.xaml.cs - Application entry point and global error handling
// ════════════════════════════════════════════════════════════════════

using System;
using System.Windows;
using RustControlPanel.Core.Rpc;
using RustControlPanel.Core.Utils;
using RustControlPanel.Services;

namespace RustControlPanel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Lifecycle

        /// <summary>
        /// Called when the application starts.
        /// Sets up global exception handlers and initializes services.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Setup global exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            Logger.Instance.Info("========================================");
            Logger.Instance.Info("Rust Control Panel - Application Started");
            Logger.Instance.Info("========================================");

            // Initialize critical singletons early
            _ = RpcRouter.Instance;
            _ = ServerStatsService.Instance;
        }

        /// <summary>
        /// Called when the application exits.
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Instance.Info("========================================");
            Logger.Instance.Info("Rust Control Panel - Application Exited");
            Logger.Instance.Info("========================================");

            base.OnExit(e);
        }

        #endregion

        #region Exception Handlers

        /// <summary>
        /// Handles unhandled exceptions from non-UI threads.
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logger.Instance.Critical("Unhandled exception occurred", ex);

                MessageBox.Show(
                    $"Une erreur critique s'est produite :\n\n{ex.Message}\n\nL'application va se fermer.\n\nVoir RustControlPanel.log pour plus de détails.",
                    "Erreur Critique",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles unhandled exceptions from the UI thread.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Instance.Error("Dispatcher unhandled exception occurred", e.Exception);

            MessageBox.Show(
                $"Une erreur s'est produite :\n\n{e.Exception.Message}\n\nVoir RustControlPanel.log pour plus de détails.",
                "Erreur Application",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Mark as handled to prevent app crash
            e.Handled = true;
        }

        #endregion
    }
}
