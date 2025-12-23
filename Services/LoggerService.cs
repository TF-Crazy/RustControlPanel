using System;
using System.Diagnostics;
using System.IO;

namespace RustControlPanel.Services
{
    public static class LoggerService
    {
        // On écrit dans le dossier de l'application pour être SUR de le trouver
        private static readonly string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug_log.txt");

        public static void Initialize()
        {
            Log("=== INITIALISATION DU SYSTÈME ===");
        }

        public static void Log(string message, string level = "INFO")
        {
            string logLine = $"[{DateTime.Now:HH:mm:ss}] [{level}] {message}";
            Debug.WriteLine(logLine);

            try
            {
                // File.AppendAllText crée le fichier s'il n'existe pas
                File.AppendAllText(LogFile, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERREUR ECRITURE LOG: " + ex.Message);
            }
        }

        public static void Error(string message, Exception? ex = null)
        {
            Log($"{message} | EX: {ex?.Message} | STACK: {ex?.StackTrace}", "ERROR");
        }
    }
}