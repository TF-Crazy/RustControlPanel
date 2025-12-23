namespace RustControlPanel.Core.Rpc
{
    public struct RpcPacket
    {
        public uint Id;
        public string Method;
        public object[] Parameters;
    }
}