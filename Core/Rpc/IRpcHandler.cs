// ════════════════════════════════════════════════════════════════════
// IRpcHandler.cs - Interface for RPC message handlers
// ════════════════════════════════════════════════════════════════════

using RustControlPanel.Core.Bridge;

namespace RustControlPanel.Core.Rpc
{
    /// <summary>
    /// Interface for RPC message handlers.
    /// Each handler processes messages for specific RPC IDs.
    /// </summary>
    public interface IRpcHandler
    {
        /// <summary>
        /// Gets the RPC ID this handler processes.
        /// </summary>
        uint RpcId { get; }

        /// <summary>
        /// Handles an incoming RPC message.
        /// </summary>
        /// <param name="reader">Binary reader containing the message data</param>
        void Handle(BridgeReader reader);
    }
}
