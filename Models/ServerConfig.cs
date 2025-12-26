// ════════════════════════════════════════════════════════════════════
// ServerConfig.cs - Server connection configuration
// ════════════════════════════════════════════════════════════════════

namespace RustControlPanel.Models
{
    /// <summary>
    /// Server connection configuration.
    /// </summary>
    public class ServerConfig
    {
        #region Properties

        /// <summary>
        /// Server hostname or IP address.
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        /// Server port (default: 3050).
        /// </summary>
        public int Port { get; set; } = 3050;

        /// <summary>
        /// Connection password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Display name for saved server.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Whether to save credentials.
        /// </summary>
        public bool SaveCredentials { get; set; } = false;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the WebSocket URI for this configuration.
        /// </summary>
        /// <returns>WebSocket URI string</returns>
        public string GetWebSocketUri()
        {
            return $"ws://{Host}:{Port}";
        }

        /// <summary>
        /// Creates a copy of this configuration.
        /// </summary>
        /// <returns>New ServerConfig instance</returns>
        public ServerConfig Clone()
        {
            return new ServerConfig
            {
                Host = Host,
                Port = Port,
                Password = Password,
                DisplayName = DisplayName,
                SaveCredentials = SaveCredentials
            };
        }

        #endregion
    }
}
