using System;
using System.IO;
using System.Text;

namespace RustControlPanel.Core.Network
{
    public class BridgeReader : BinaryReader
    {
        public BridgeReader(Stream input) : base(input, Encoding.UTF8) { }

        public override string ReadString()
        {
            // Lecture explicite de la longueur en UInt32
            uint length = ReadUInt32();
            byte[] bytes = ReadBytes((int)length);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}