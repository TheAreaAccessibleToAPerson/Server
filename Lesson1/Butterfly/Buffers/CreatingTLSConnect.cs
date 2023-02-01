namespace Butterfly.Buffer
{
    public struct CreatingTLSConnect : IBuffer
    {
        public NetPacket NetworkPacket;

        public string GetName()
        {
            return typeof(CreatingTLSConnect).Name;
        }
    }
}
