// ════════════════════════════════════════════════════════════════════
// BridgeReader.cs - Binary reader for RPC responses
// ════════════════════════════════════════════════════════════════════

using System;
using System.IO;
using System.Text;

namespace RustControlPanel.Core.Bridge
{
    /// <summary>
    /// Binary reader for Carbon Bridge RPC protocol responses.
    /// Reads message format: [int32 channel][uint32 rpcId][...args]
    /// </summary>
    public class BridgeReader : IDisposable
    {
        #region Fields

        private readonly MemoryStream _stream;
        private readonly BinaryReader _reader;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new BridgeReader from a byte array.
        /// </summary>
        /// <param name="data">Binary data received from server</param>
        public BridgeReader(byte[] data)
        {
            _stream = new MemoryStream(data);
            _reader = new BinaryReader(_stream, Encoding.UTF8);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether there is more data to read.
        /// </summary>
        public bool HasData => _stream.Position < _stream.Length;

        /// <summary>
        /// Gets the current position in the stream.
        /// </summary>
        public long Position => _stream.Position;

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads the RPC header (channel and rpcId).
        /// </summary>
        /// <param name="channel">Channel number (output)</param>
        /// <param name="rpcId">RPC ID (output)</param>
        public void ReadRpcHeader(out int channel, out uint rpcId)
        {
            channel = _reader.ReadInt32();
            rpcId = _reader.ReadUInt32();
        }

        /// <summary>
        /// Reads a string value.
        /// </summary>
        /// <returns>String value</returns>
        public string ReadString()
        {
            return _reader.ReadString();
        }

        /// <summary>
        /// Reads an int32 value.
        /// </summary>
        /// <returns>Integer value</returns>
        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        /// <summary>
        /// Reads a uint32 value.
        /// </summary>
        /// <returns>Unsigned integer value</returns>
        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        /// <summary>
        /// Reads a uint64 value (for Steam IDs).
        /// </summary>
        /// <returns>Unsigned long value</returns>
        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }

        /// <summary>
        /// Reads a float value.
        /// </summary>
        /// <returns>Float value</returns>
        public float ReadFloat()
        {
            return _reader.ReadSingle();
        }

        /// <summary>
        /// Reads a boolean value.
        /// </summary>
        /// <returns>Boolean value</returns>
        public bool ReadBoolean()
        {
            return _reader.ReadBoolean();
        }

        /// <summary>
        /// Reads a byte value.
        /// </summary>
        /// <returns>Byte value</returns>
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        /// <summary>
        /// Reads a byte array of specified length.
        /// </summary>
        /// <param name="count">Number of bytes to read</param>
        /// <returns>Byte array</returns>
        public byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

        /// <summary>
        /// Skips a specified number of bytes.
        /// </summary>
        /// <param name="count">Number of bytes to skip</param>
        public void Skip(int count)
        {
            _stream.Seek(count, SeekOrigin.Current);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the reader and underlying stream.
        /// </summary>
        public void Dispose()
        {
            _reader?.Dispose();
            _stream?.Dispose();
        }

        #endregion
    }
}
