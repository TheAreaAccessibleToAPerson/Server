namespace Butterfly.Buffer
{
    public struct CreatingTLSConnect : IBuffer
    {
        public NetPackets NetworkPacket;

        public string GetName()
        {
            return typeof(CreatingTLSConnect).Name;
        }
    }
}
