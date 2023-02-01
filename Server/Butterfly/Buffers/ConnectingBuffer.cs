namespace Butterfly.Buffer
{
    public struct Connecting : IBuffer
    {
        public Hellper.VPN.ClientPorts.TLSPorts TLSPorts;

        public Hellper.VPN.ClientPorts.Port Port1;
        public Hellper.VPN.ClientPorts.Port Port2;

        public string GetName()
        {
            return typeof(Connecting).Name;
        }
    }

    public struct ConnectingSocket : IBuffer
    {
        public string GetName()
        {
            return typeof(ConnectingSocket).Name;
        }
    }
}
