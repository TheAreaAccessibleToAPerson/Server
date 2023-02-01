namespace Butterfly.Buffer
{
    public struct Client : IBuffer
    {
        public System.Net.Sockets.Socket Socket;
        public System.Net.IPAddress Address;
        public int Port;
        public int Size;

        public NetPacket FirstPacket;

        public const int BUFFER_SIZE = 32000;

        public Hellper.VPN.ClientPorts ClientPorts;

        public string GetName()
        {
            return typeof(Client).Name;
        }
    }
}
