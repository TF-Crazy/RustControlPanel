// ════════════════════════════════════════════════════════════════════
// MapEntitiesHandler.cs - Handles RequestMapEntities RPC response
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.ObjectModel;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.Core.Rpc.Handlers
{
    /// <summary>
    /// Handles the RequestMapEntities RPC response containing all map entities.
    /// </summary>
    public class MapEntitiesHandler : IRpcHandler
    {
        /// <summary>
        /// Event fired when map entities are received.
        /// </summary>
        public event EventHandler<ObservableCollection<MapEntity>>? EntitiesReceived;

        /// <summary>
        /// RPC ID for RequestMapEntities.
        /// </summary>
        public uint RpcId => RpcHelper.GetRpcId(RpcNames.REQUEST_MAP_ENTITIES);

        /// <summary>
        /// Handles the RequestMapEntities RPC response.
        /// Format: [int32 count][[uint64 entityId][uint64 steamId][byte type][string label][float x][float y][float rotation][bool isOnline][bool isDead]]...
        /// </summary>
        public void Handle(BridgeReader reader)
        {
            try
            {
                var entities = new ObservableCollection<MapEntity>();

                // Read entity count
                var count = reader.ReadInt32();
                Logger.Instance.Debug($"MapEntities: Receiving {count} entities");

                // Read each entity
                for (int i = 0; i < count; i++)
                {
                    var entity = new MapEntity
                    {
                        EntityId = reader.ReadUInt64(),
                        SteamId = reader.ReadUInt64(),
                        Type = (MapEntityType)reader.ReadByte(),
                        Label = reader.ReadString(),
                        X = reader.ReadFloat(),
                        Y = reader.ReadFloat(),
                        Rotation = reader.ReadFloat(),
                        IsOnline = reader.ReadBoolean(),
                        IsDead = reader.ReadBoolean()
                    };

                    entities.Add(entity);
                }

                Logger.Instance.Debug($"MapEntities: Parsed {entities.Count} entities successfully");

                // Fire event
                EntitiesReceived?.Invoke(this, entities);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Failed to parse MapEntities", ex);
            }
        }
    }
}
