// ════════════════════════════════════════════════════════════════════
// RawRpcLogger.cs - Debug utility to log raw RPC data
// ════════════════════════════════════════════════════════════════════

using System;
using System.Text;
using RustControlPanel.Core.Bridge;
using RustControlPanel.Core.Utils;

namespace RustControlPanel.Core.Rpc.Handlers
{
    /// <summary>
    /// Debug handler that logs all raw RPC data.
    /// Helps identify RPC structure and parsing issues.
    /// </summary>
    public class RawRpcLogger
    {
        /// <summary>
        /// Logs raw RPC data in hexadecimal and attempts to parse as values.
        /// </summary>
        public static void LogRawData(uint rpcId, byte[] data, int offset)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"════════════════════════════════════════");
                sb.AppendLine($"RPC ID: {rpcId} (0x{rpcId:X8})");
                sb.AppendLine($"Total bytes: {data.Length}");
                sb.AppendLine($"Data after header (offset {offset}):");
                
                // Hex dump
                sb.Append("HEX: ");
                for (int i = offset; i < data.Length && i < offset + 64; i++)
                {
                    sb.Append($"{data[i]:X2} ");
                }
                sb.AppendLine();

                // Try to read as various types
                using var reader = new BridgeReader(data);
                reader.ReadRpcHeader(out _, out _); // Skip header

                sb.AppendLine("Attempting to parse:");
                
                try
                {
                    int pos = 0;
                    while (reader.HasData && pos < 10)
                    {
                        var startPos = reader.Position;
                        
                        // Try string
                        try
                        {
                            var str = reader.ReadString();
                            sb.AppendLine($"  [{startPos}] String: \"{str}\" (len: {str.Length})");
                            pos++;
                            continue;
                        }
                        catch
                        {
                            // Reset and try int32
                        }

                        // Can't continue, break
                        break;
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"  Parsing stopped: {ex.Message}");
                }

                sb.AppendLine($"════════════════════════════════════════");
                Logger.Instance.Debug(sb.ToString());
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"RawRpcLogger error: {ex.Message}");
            }
        }
    }
}
