using System;
using System.Diagnostics;
using System.IO;

namespace RustControlPanel.Services
{
    public static class LoggerService
    {
        private static readonly string LogFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RustControlPanel", "Logs");

        private static readonly string LogFile = Path.Combine(LogFolder, $"log_{DateTime.Now:yyyy-MM-dd}.txt");

        static LoggerService()
        {
            try
            {
                if (!Directory.Exists(LogFolder)) Directory.CreateDirectory(LogFolder);
            }
            catch { /* Impossible d'écrire sur le disque */ }
        }

        public static void Initialize()
        {
            try
            {
                if (!Directory.Exists(LogFolder))
                    Directory.CreateDirectory(LogFolder);

                Log("Logger initialisé manuellement.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERREUR INITIALISATION LOGGER: " + ex.Message);
            }
        }

        public static void Log(string message, string level = "INFO")
        {
            string logLine = $"[{DateTime.Now:HH:mm:ss}] [{level}] {message}";

            // Debug Output (Visual Studio)
            Debug.WriteLine(logLine);

            // File Output
            try
            {
                File.AppendAllText(LogFile, logLine + Environment.NewLine);
            }
            catch { /* On ne crash pas le logger si le fichier est verrouillé */ }
        }

        public static void Error(string message, Exception? ex = null)
        {
            string errorDetails = ex != null
                ? $"\n   [TYPE]: {ex.GetType().Name}\n   [MSG]: {ex.Message}\n   [STACK]: {ex.StackTrace}"
                : "";
            Log($"{message}{errorDetails}", "ERROR");
        }
    }
}