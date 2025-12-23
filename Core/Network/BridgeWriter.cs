using System;
using System.IO;
using System.Text;

namespace RustControlPanel.Core.Network
{
    public class BridgeWriter(MemoryStream ms) : BinaryWriter(ms, Encoding.UTF8)
    {
        public override void Write(string value)
        {
            byte[] bytes = string.IsNullOrEmpty(value) ? [] : Encoding.UTF8.GetBytes(value);
            base.Write((uint)bytes.Length);
            base.Write(bytes);
        }
    }    
}