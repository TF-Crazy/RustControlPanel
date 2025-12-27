// ════════════════════════════════════════════════════════════════════
// MapInfoHandler.cs - Handles LoadMapInfo RPC response
// ════════════════════════════════════════════════════════════════════

using System;
using System.IO;
using System.Windows.Media.Imaging;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.Core.Rpc.Handlers
{
    /// <summary>
    /// Handles the LoadMapInfo RPC response containing map image and monuments.
    /// </summary>
    public class MapInfoHandler : IRpcHandler
    {
        /// <summary>
        /// Event fired when map info is received.
        /// </summary>
        public event EventHandler<MapInfo>? MapInfoReceived;

        /// <summary>
        /// RPC ID for LoadMapInfo.
        /// </summary>
        public uint RpcId => RpcHelper.GetRpcId(RpcNames.LOAD_MAP_INFO);

        /// <summary>
        /// Handles the LoadMapInfo RPC response.
        /// Format: [int32 width][int32 height][int32 imgLength][byte[] imgData][uint32 worldSize][int32 monumentCount][monuments...]
        /// Monument format: [string name][float x][float y]
        /// </summary>
        public void Handle(BridgeReader reader)
        {
            try
            {
                var mapInfo = new MapInfo();

                // Read image dimensions
                var imageWidth = reader.ReadInt32();
                var imageHeight = reader.ReadInt32();
                var imageDataLength = reader.ReadInt32();

                Logger.Instance.Debug($"MapInfo: Image {imageWidth}x{imageHeight}, {imageDataLength} bytes");

                // Read image data
                var imageData = reader.ReadBytes(imageDataLength);

                // Decode image as BitmapImage for WPF
                using var ms = new MemoryStream(imageData);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze(); // Make it thread-safe

                mapInfo.ImageSource = bitmap;

                // Read world size
                mapInfo.WorldSize = reader.ReadUInt32();

                Logger.Instance.Debug($"MapInfo: WorldSize = {mapInfo.WorldSize}m");

                // Read monuments
                var monumentCount = reader.ReadInt32();
                Logger.Instance.Debug($"MapInfo: {monumentCount} monuments");

                for (int i = 0; i < monumentCount; i++)
                {
                    var name = reader.ReadString();
                    var x = reader.ReadFloat();
                    var y = reader.ReadFloat();

                    mapInfo.Monuments.Add(new Monument(name, x, y));
                }

                Logger.Instance.Info($"Map loaded: {mapInfo.WorldSize}m, {monumentCount} monuments");

                // Fire event
                MapInfoReceived?.Invoke(this, mapInfo);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Failed to parse MapInfo", ex);
            }
        }
    }
}
