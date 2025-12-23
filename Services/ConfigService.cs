using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RustControlPanel.Models;

namespace RustControlPanel.Services
{
    public class ConfigService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        // Cache des options pour les performances (CA1869)
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public ConfigService()
        {
            // Chemin 1 : Dans le dossier de l'application (pour le dev/git)
            string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Servers", "servers.json");

            // Chemin 2 : Dans AppData (pour l'installation finale)
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RustControlPanel", "servers.json");

            // On choisit le local s'il existe, sinon AppData
            _filePath = File.Exists(localPath) ? localPath : appDataPath;
            _folderPath = Path.GetDirectoryName(_filePath) ?? "";

            LogService.Write($"Utilisation du fichier de config : {_filePath}");
        }

        public List<ServerConfig> LoadServers()
        {
            if (!File.Exists(_filePath))
            {
                LogService.Write("Aucun fichier servers.json trouvé, création d'une liste vide.");
                return [];
            }
            try
            {
                string json = File.ReadAllText(_filePath);
                var servers = JsonSerializer.Deserialize<List<ServerConfig>>(json, _jsonOptions);
                LogService.Write($"{servers?.Count ?? 0} serveurs chargés depuis la configuration.");
                return servers ?? [];
            }
            catch (Exception)
            {
                LogService.Write("Erreur lors de la lecture du fichier JSON");
                return [];
            }
        }

        public void SaveServers(List<ServerConfig> servers)
        {
            if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);
            string json = JsonSerializer.Serialize(servers, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
    }
}