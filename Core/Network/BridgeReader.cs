using System.IO;
using System.Text;

namespace RustControlPanel.Core.Network
{
    /// <summary>
    /// Lecteur binaire personnalisé pour le protocole Carbon.
    /// Utilise un constructeur principal (Primary Constructor).
    /// </summary>
    public class BridgeReader(Stream input) : BinaryReader(input, Encoding.UTF8)
    {
        public override string ReadString()
        {
            // Carbon utilise un UInt32 pour la longueur des chaînes
            uint length = ReadUInt32();
            byte[] bytes = ReadBytes((int)length);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}