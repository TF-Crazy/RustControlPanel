// ════════════════════════════════════════════════════════════════════
// BridgeWriter.cs - Binary writer for RPC messages
// ════════════════════════════════════════════════════════════════════

using System;
using System.IO;
using System.Text;
using RustControlPanel.Core.Utils;

namespace RustControlPanel.Core.Bridge
{
    /// <summary>
    /// Binary writer for Carbon Bridge RPC protocol.
    /// Message format: [int32 channel=0][uint32 rpcId][...args]
    /// </summary>
    public class BridgeWriter : IDisposable
    {
        #region Fields

        private readonly MemoryStream _stream;
        private readonly BinaryWriter _writer;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new BridgeWriter.
        /// </summary>
        public BridgeWriter()
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream, Encoding.UTF8);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts a new RPC message with the given name.
        /// Writes: [int32 channel=0][uint32 rpcId]
        /// </summary>
        /// <param name="rpcName">Name of the RPC</param>
        public void WriteRpcHeader(string rpcName)
        {
            // Channel (always 0)
            _writer.Write((int)0);

            // RPC ID (MD5 hash)
            var rpcId = RpcHelper.GetRpcId(rpcName);
            _writer.Write(rpcId);
        }

        /// <summary>
        /// Writes a string argument.
        /// </summary>
        /// <param name="value">String value</param>
        public void WriteString(string value)
        {
            _writer.Write(value ?? string.Empty);
        }

        /// <summary>
        /// Writes an int32 argument.
        /// </summary>
        /// <param name="value">Integer value</param>
        public void WriteInt32(int value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a uint32 argument.
        /// </summary>
        /// <param name="value">Unsigned integer value</param>
        public void WriteUInt32(uint value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a uint64 argument (for Steam IDs).
        /// </summary>
        /// <param name="value">Unsigned long value</param>
        public void WriteUInt64(ulong value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a float argument.
        /// </summary>
        /// <param name="value">Float value</param>
        public void WriteFloat(float value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a boolean argument.
        /// </summary>
        /// <param name="value">Boolean value</param>
        public void WriteBoolean(bool value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Gets the message as a byte array.
        /// </summary>
        /// <returns>Byte array containing the complete message</returns>
        public byte[] ToArray()
        {
            _writer.Flush();
            return _stream.ToArray();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the writer and underlying stream.
        /// </summary>
        public void Dispose()
        {
            _writer?.Dispose();
            _stream?.Dispose();
        }

        #endregion
    }
}
