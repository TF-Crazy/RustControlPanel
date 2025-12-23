using System;
using System.IO;
using System.Text;

namespace RustControlPanel.Core.Network
{
    public class BridgeWriter : BinaryWriter
    {
        public BridgeWriter(MemoryStream ms) : base(ms, Encoding.UTF8) { }

        public override void Write(string value)
        {
            // Carbon attend un UInt32 pour la longueur, pas le 7-bit de .NET
            byte[] bytes = string.IsNullOrEmpty(value)
                ? Array.Empty<byte>()
                : Encoding.UTF8.GetBytes(value);

            base.Write((uint)bytes.Length);
            base.Write(bytes);
        }
    }
}