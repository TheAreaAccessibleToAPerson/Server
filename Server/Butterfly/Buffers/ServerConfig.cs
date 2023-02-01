namespace Butterfly.Buffer
{
    public struct ServerConfig : IBuffer
    {
        public string AddressServer;
        public int PortServer;

        public string SnifferAddress;
        public int SnifferPort;

        public string GetName()
        {
            return typeof(ServerConfig).ToString();
        }
    }
}
