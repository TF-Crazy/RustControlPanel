// ════════════════════════════════════════════════════════════════════
// RpcHelper.cs - RPC ID calculation using MD5
// ════════════════════════════════════════════════════════════════════

using System;
using System.Security.Cryptography;
using System.Text;

namespace RustControlPanel.Core.Utils
{
    /// <summary>
    /// Helper class for RPC operations.
    /// Calculates RPC IDs using MD5 hash of the RPC name.
    /// </summary>
    public static class RpcHelper
    {
        /// <summary>
        /// Calculates the RPC ID for a given RPC name.
        /// Formula: MD5("RPC_{rpcName}") -> first 4 bytes as uint32
        /// </summary>
        /// <param name="rpcName">Name of the RPC (e.g. "ServerInfo")</param>
        /// <returns>32-bit unsigned integer RPC ID</returns>
        public static uint GetRpcId(string rpcName)
        {
            var input = $"RPC_{rpcName}";
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToUInt32(hash, 0);
        }
    }
}
