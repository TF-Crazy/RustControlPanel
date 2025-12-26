// ════════════════════════════════════════════════════════════════════
// SettingsService.cs - Application settings persistence (Singleton)
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.Services
{
    /// <summary>
    /// Manages application settings persistence (Singleton).
    /// Saves and loads server configurations and app preferences.
    /// </summary>
    public sealed class SettingsService
    {
        #region Singleton

        private static readonly Lazy<SettingsService> _instance = new(() => new SettingsService());

        /// <summary>
        /// Gets the singleton instance of SettingsService.
        /// </summary>
        public static SettingsService Instance => _instance.Value;

        #endregion

        #region Fields

        private readonly string _settingsFilePath;
        private AppSettings _settings;

        #endregion

        #region Constructor

        private SettingsService()
        {
            var configDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            Directory.CreateDirectory(configDir);
            _settingsFilePath = Path.Combine(configDir, "appsettings.json");

            _settings = LoadSettings();
            Logger.Instance.Debug("SettingsService instance created");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of saved server configurations.
        /// </summary>
        public List<ServerConfig> SavedServers => _settings.SavedServers;

        /// <summary>
        /// Gets or sets the last connected server host.
        /// </summary>
        public string? LastConnectedHost
        {
            get => _settings.LastConnectedHost;
            set
            {
                _settings.LastConnectedHost = value;
                SaveSettings();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds or updates a server configuration.
        /// </summary>
        /// <param name="config">Server configuration to save</param>
        public void AddOrUpdateServer(ServerConfig config)
        {
            // Remove existing config with same host:port
            _settings.SavedServers.RemoveAll(s => s.Host == config.Host && s.Port == config.Port);

            // Add new config
            _settings.SavedServers.Add(config.Clone());

            SaveSettings();
            Logger.Instance.Info($"Saved server: {config.Host}:{config.Port}");
        }

        /// <summary>
        /// Removes a server configuration.
        /// </summary>
        /// <param name="host">Server host</param>
        /// <param name="port">Server port</param>
        public void RemoveServer(string host, int port)
        {
            _settings.SavedServers.RemoveAll(s => s.Host == host && s.Port == port);
            SaveSettings();
            Logger.Instance.Info($"Removed server: {host}:{port}");
        }

        /// <summary>
        /// Gets a server configuration by host and port.
        /// </summary>
        /// <param name="host">Server host</param>
        /// <param name="port">Server port</param>
        /// <returns>Server configuration or null</returns>
        public ServerConfig? GetServer(string host, int port)
        {
            return _settings.SavedServers.Find(s => s.Host == host && s.Port == port);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads settings from file.
        /// </summary>
        private AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                    if (settings != null)
                    {
                        Logger.Instance.Info("Settings loaded successfully");
                        return settings;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Failed to load settings", ex);
            }

            Logger.Instance.Info("Using default settings");
            return new AppSettings();
        }

        /// <summary>
        /// Saves settings to file.
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
                File.WriteAllText(_settingsFilePath, json);
                Logger.Instance.Debug("Settings saved");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Failed to save settings", ex);
            }
        }

        #endregion

        #region Nested Class

        /// <summary>
        /// Application settings data structure.
        /// </summary>
        private class AppSettings
        {
            public List<ServerConfig> SavedServers { get; set; } = new();
            public string? LastConnectedHost { get; set; }
        }

        #endregion
    }
}
