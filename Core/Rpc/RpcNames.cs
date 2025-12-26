// ════════════════════════════════════════════════════════════════════
// RpcNames.cs - RPC name constants
// ════════════════════════════════════════════════════════════════════

namespace RustControlPanel.Core.Rpc
{
    /// <summary>
    /// Constants for all available RPC names in Carbon WebControlPanel Bridge.
    /// </summary>
    public static class RpcNames
    {
        #region Server Info

        /// <summary>
        /// Request server information (hostname, FPS, player count, time, etc.)
        /// </summary>
        public const string SERVER_INFO = "ServerInfo";

        #endregion

        #region Map

        /// <summary>
        /// Load map information (image, monuments, size, seed)
        /// </summary>
        public const string LOAD_MAP_INFO = "LoadMapInfo";

        #endregion

        #region Players

        /// <summary>
        /// Request player list with teams
        /// </summary>
        public const string PLAYERS = "Players";

        #endregion

        #region Entities

        /// <summary>
        /// Request map entities (positions, types)
        /// </summary>
        public const string REQUEST_MAP_ENTITIES = "RequestMapEntities";

        /// <summary>
        /// Search for specific entities
        /// </summary>
        public const string SEARCH_ENTITIES = "SearchEntities";

        #endregion

        #region Console

        /// <summary>
        /// Console tail push (server → client)
        /// </summary>
        public const string CONSOLE_TAIL = "ConsoleTail";

        /// <summary>
        /// Send console command (client → server)
        /// </summary>
        public const string CONSOLE_INPUT = "ConsoleInput";

        #endregion

        #region Chat

        /// <summary>
        /// Chat tail push (server → client)
        /// </summary>
        public const string CHAT_TAIL = "ChatTail";

        /// <summary>
        /// Send chat message (client → server)
        /// </summary>
        public const string CHAT_INPUT = "ChatInput";

        #endregion

        #region Plugins

        /// <summary>
        /// Request plugins list
        /// </summary>
        public const string PLUGINS = "Plugins";

        /// <summary>
        /// Request details for a specific plugin
        /// </summary>
        public const string PLUGIN_DETAILS = "PluginDetails";

        #endregion

        #region Inventory

        /// <summary>
        /// Request player inventory
        /// </summary>
        public const string SEND_PLAYER_INVENTORY = "SendPlayerInventory";

        #endregion
    }
}
