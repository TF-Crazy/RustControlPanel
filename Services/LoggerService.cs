using System;
using System.Diagnostics;

namespace RustControlPanel.Services
{
    public static class LoggerService
    {
        public static void Log(string message, string level = "INFO")
        {
            string logLine = $"[{DateTime.Now:HH:mm:ss}] [{level}] {message}";
            Debug.WriteLine(logLine);
            // On pourra ajouter ici une écriture vers un fichier plus tard
        }

        public static void Error(string message, Exception? ex = null)
        {
            string errorDetails = ex != null ? $"\nDétails: {ex.Message}\n{ex.StackTrace}" : "";
            Log($"{message}{errorDetails}", "ERROR");
        }
    }
}