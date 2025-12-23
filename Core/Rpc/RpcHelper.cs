using System.IO;
using RustControlPanel.Core.Network;

namespace RustControlPanel.Core.Rpc
{
    public static class RpcHelper
    {
        public static byte[] Encode(RpcPacket packet)
        {
            using var ms = new MemoryStream();
            using var writer = new BridgeWriter(ms);

            writer.Write(packet.Id);
            writer.Write(packet.Method);

            if (packet.Parameters != null)
            {
                foreach (var param in packet.Parameters)
                {
                    switch (param)
                    {
                        case string s: writer.Write(s); break;
                        case uint u: writer.Write(u); break;
                        case int i: writer.Write(i); break;
                        case bool b: writer.Write(b); break;
                        case float f: writer.Write(f); break;
                    }
                }
            }
            return ms.ToArray();
        }
    }
}