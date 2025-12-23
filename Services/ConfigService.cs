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

        public ConfigService()
        {
            _folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RustControlPanel");
            _filePath = Path.Combine(_folderPath, "servers.json");
        }

        public List<ServerConfig> LoadServers()
        {
            try
            {
                if (!File.Exists(_filePath)) return new List<ServerConfig>();
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<ServerConfig>>(json) ?? new List<ServerConfig>();
            }
            catch
            {
                return new List<ServerConfig>();
            }
        }

        public void SaveServers(List<ServerConfig> servers)
        {
            if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);
            string json = JsonSerializer.Serialize(servers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}