namespace Butterfly.Hellper.VPN
{
    class ConnectionManager : HellperHeader
    {
        private readonly VPN.PortsManager PortsManager = new VPN.PortsManager();

        public ConnectionManager Creating(int pClientPortCount, int pPortCount, int pTLSPortsCount)
        {
            PortsManager.Creating(pClientPortCount, pPortCount, pTLSPortsCount);

            return this;
        }

        public void AddClient(Buffer.Client pClientBuffer)
        {
            string key = pClientBuffer.Address.ToString();
            
            if (PortsManager.TryGetClientPorts(key, pClientBuffer.Address, out ClientPorts oClientPorts))
            {
                pClientBuffer.ClientPorts = oClientPorts;
     
                User.AddObject<Client>(key, pClientBuffer);
            }
            else
            {
                Exception("Не удалось получить порты для нового клиента.");
            }
        }
        public void FreePorts(Buffer.Client pClientBuffer)
        {
        }
        public void SendPacketToClient(Buffer.NetPacket pNetworkPackets)
        {
            if (pNetworkPackets.GetSourceAddress().ToString() == "185.215.4.10")
            {
                if (PortsManager.TryGetPacketToSendClient(pNetworkPackets.DestinationPort, out string oClientKey))
                {
                    User.SendToChildren(oClientKey, pNetworkPackets);
                }
            }
        }
    }
}