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
            _folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RustControlPanel");
            _filePath = Path.Combine(_folderPath, "servers.json");
        }

        public List<ServerConfig> LoadServers()
        {
            if (!File.Exists(_filePath))
            {
                LoggerService.Log("Aucun fichier servers.json trouvé, création d'une liste vide.");
                return [];
            }
            try
            {
                string json = File.ReadAllText(_filePath);
                var servers = JsonSerializer.Deserialize<List<ServerConfig>>(json, _jsonOptions);
                LoggerService.Log($"{servers?.Count ?? 0} serveurs chargés depuis la configuration.");
                return servers ?? [];
            }
            catch (Exception ex)
            {
                LoggerService.Error("Erreur lors de la lecture du fichier JSON", ex);
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