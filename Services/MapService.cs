// ════════════════════════════════════════════════════════════════════
// MapService.cs - Manages map data and entity updates (Singleton)
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Rpc;
using RustControlPanel.Core.Rpc.Handlers;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.Services
{
    /// <summary>
    /// Service for managing map data and entity updates (Singleton).
    /// </summary>
    public sealed class MapService
    {
        #region Singleton

        private static readonly Lazy<MapService> _instance = new(() => new MapService());

        /// <summary>
        /// Gets the singleton instance of MapService.
        /// </summary>
        public static MapService Instance => _instance.Value;

        #endregion

        #region Fields

        private readonly MapInfoHandler _mapInfoHandler;
        private readonly MapEntitiesHandler _entitiesHandler;
        private readonly DispatcherTimer _entityPollTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Current map information.
        /// </summary>
        public MapInfo MapInfo { get; private set; } = new();

        /// <summary>
        /// Map entities (players, heli, cargo, etc.).
        /// </summary>
        public ObservableCollection<MapEntity> Entities { get; } = new();

        /// <summary>
        /// Player details.
        /// </summary>
        public ObservableCollection<PlayerDetails> Players { get; } = new();

        #endregion

        #region Events

        /// <summary>
        /// Fired when map info is received.
        /// </summary>
        public event EventHandler<MapInfo>? MapInfoReceived;

        /// <summary>
        /// Fired when entities are updated.
        /// </summary>
        public event EventHandler? EntitiesUpdated;

        #endregion

        #region Constructor

        private MapService()
        {
            // Create handlers
            _mapInfoHandler = new MapInfoHandler();
            _entitiesHandler = new MapEntitiesHandler();

            // Register handlers
            RpcRouter.Instance.RegisterHandler(_mapInfoHandler);
            RpcRouter.Instance.RegisterHandler(_entitiesHandler);

            // Subscribe to handler events
            _mapInfoHandler.MapInfoReceived += OnMapInfoReceived;
            _entitiesHandler.EntitiesReceived += OnEntitiesReceived;

            // Setup entity polling timer (every 2 seconds)
            _entityPollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _entityPollTimer.Tick += OnEntityPollTick;

            // Subscribe to connection state
            ConnectionService.Instance.ConnectionStateChanged += OnConnectionStateChanged;

            Logger.Instance.Debug("MapService instance created");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Requests map info from the server.
        /// </summary>
        public void RequestMapInfo()
        {
            if (!ConnectionService.Instance.IsConnected)
            {
                Logger.Instance.Warning("Cannot request map info: Not connected");
                return;
            }

            Logger.Instance.Debug("Requesting map info...");

            var writer = new BridgeWriter();
            writer.WriteRpcHeader(RpcNames.LOAD_MAP_INFO);

            BridgeClient.Instance.SendAsync(writer.ToArray());
        }

        /// <summary>
        /// Requests map entities from the server.
        /// </summary>
        public void RequestEntities()
        {
            if (!ConnectionService.Instance.IsConnected)
            {
                Logger.Instance.Warning("Cannot request entities: Not connected");
                return;
            }

            Logger.Instance.Debug("Requesting map entities...");

            var writer = new BridgeWriter();
            writer.WriteRpcHeader(RpcNames.REQUEST_MAP_ENTITIES);

            BridgeClient.Instance.SendAsync(writer.ToArray());
        }

        #endregion

        #region Event Handlers

        private void OnConnectionStateChanged(object? sender, bool isConnected)
        {
            if (isConnected)
            {
                Logger.Instance.Info("Connected - Loading map...");

                // Request map info
                RequestMapInfo();

                // Start entity polling
                _entityPollTimer.Start();
            }
            else
            {
                Logger.Instance.Info("Disconnected - Stopping map updates");

                // Stop polling
                _entityPollTimer.Stop();

                // Clear data
                Entities.Clear();
                Players.Clear();
            }
        }

        private void OnMapInfoReceived(object? sender, MapInfo mapInfo)
        {
            MapInfo = mapInfo;
            MapInfoReceived?.Invoke(this, mapInfo);
            Logger.Instance.Info($"Map loaded: {mapInfo.WorldSize}m, {mapInfo.Monuments.Count} monuments");

            // Request initial entities
            RequestEntities();
        }

        private void OnEntitiesReceived(object? sender, ObservableCollection<MapEntity> entities)
        {
            // Update entities collection
            Entities.Clear();
            foreach (var entity in entities)
            {
                Entities.Add(entity);
            }

            // Extract players from entities
            UpdatePlayersFromEntities(entities);

            EntitiesUpdated?.Invoke(this, EventArgs.Empty);
            Logger.Instance.Debug($"Entities updated: {entities.Count} total");
        }

        private void OnEntityPollTick(object? sender, EventArgs e)
        {
            // Poll entities every 2 seconds
            RequestEntities();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the Players collection from entities.
        /// </summary>
        private void UpdatePlayersFromEntities(ObservableCollection<MapEntity> entities)
        {
            // Get player entities
            var playerEntities = entities.Where(e =>
                e.Type == MapEntityType.ActivePlayer ||
                e.Type == MapEntityType.SleepingPlayer).ToList();

            // Update or add players
            foreach (var entity in playerEntities)
            {
                var existingPlayer = Players.FirstOrDefault(p => p.SteamId == entity.SteamId);

                if (existingPlayer != null)
                {
                    // Update existing player
                    existingPlayer.IsOnline = entity.IsOnline;
                    existingPlayer.IsDead = entity.IsDead;
                    existingPlayer.Position = entity.Position;
                    existingPlayer.Rotation = entity.Rotation;
                    existingPlayer.EntityId = entity.EntityId;
                }
                else
                {
                    // Add new player
                    var newPlayer = new PlayerDetails
                    {
                        SteamId = entity.SteamId,
                        DisplayName = entity.Label,
                        IsOnline = entity.IsOnline,
                        IsDead = entity.IsDead,
                        Health = 100f, // Default (will be updated if we get player details)
                        Ping = 0,
                        Position = entity.Position,
                        Rotation = entity.Rotation,
                        EntityId = entity.EntityId
                    };

                    Players.Add(newPlayer);
                }
            }

            // Remove players that are no longer in entities
            var steamIdsToKeep = playerEntities.Select(e => e.SteamId).ToHashSet();
            for (int i = Players.Count - 1; i >= 0; i--)
            {
                if (!steamIdsToKeep.Contains(Players[i].SteamId))
                {
                    Players.RemoveAt(i);
                }
            }
        }

        #endregion
    }
}
