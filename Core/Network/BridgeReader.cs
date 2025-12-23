using System;
using System.IO;
using System.Text;

namespace RustControlPanel.Core.Network
{
    public class BridgeReader(Stream input) : BinaryReader(input, Encoding.UTF8)
    {
        public override string ReadString()
        {
            uint length = ReadUInt32();
            return Encoding.UTF8.GetString(ReadBytes((int)length));
        }
    }
}