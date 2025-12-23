using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace RustControlPanel.Services
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = "INFO";
        public string Message { get; set; } = "";
    }

    public static class LogService
    {
        public static ObservableCollection<LogEntry> Entries { get; } = [];

        public static void Write(string message, string level = "INFO")
        {
            // On s'assure d'être sur le thread UI pour l'ObservableCollection
            Application.Current.Dispatcher.Invoke(() =>
            {
                Entries.Add(new LogEntry { Timestamp = DateTime.Now, Level = level, Message = message });
                if (Entries.Count > 500) Entries.RemoveAt(0); // Limite de mémoire
            });
            System.Diagnostics.Debug.WriteLine($"[{level}] {message}");
        }

        public static void HandleCrash(Exception ex)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"CRASH_{DateTime.Now:yyyyMMdd_HHmm}.txt");
            string content = $"CRASH LOG\nDate: {DateTime.Now}\nMessage: {ex.Message}\nStack:\n{ex.StackTrace}";
            System.IO.File.WriteAllText(path, content);
        }
    }
}