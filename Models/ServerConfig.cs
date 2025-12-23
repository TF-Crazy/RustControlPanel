using System;

namespace RustControlPanel.Models
{
    public class ServerConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Nouveau Serveur";
        public string Ip { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 25555;
        public string Password { get; set; }

        public string ConnectionUri => $"ws://{Ip}:{Port}/{Password}";
    }
}