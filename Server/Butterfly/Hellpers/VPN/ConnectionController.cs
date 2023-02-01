namespace Butterfly.Hellper.VPN
{
    public struct SeqAckNumber : IBuffer
    {
        public int IndexInArray; // Индекс в массиве.
        public int SocketNumber; // Для какого сокета преднозначек.
        public long Sequence; // Значение.
        public long Acknowledgment;

        public string GetName() => typeof(SeqAckNumber).ToString();
    }

    class ConnectionController : HellperHeader
    {
        private VPN.PortController PortController = new VPN.PortController();

        private ConvertingFields ConvertingField = new ConvertingFields();

        public ConnectionController Creating(Buffer.Client pClientBuffer)
        {
            PortController.Creating(pClientBuffer.ClientPorts);

            return this;
        }

        public void SetSeqAckNumberTLSConnection(SeqAckNumber pSeqAckNumber)
        {
            PortController.SetSeqAckNumberTLSConnection(pSeqAckNumber);
        }

        public void SetSeqAckNumberHTTPConnection(SeqAckNumber pSeqAckNumber)
        {
            PortController.SetSeqAckNumberHTTPConnection(pSeqAckNumber);
        }

        public void ReceiveNetworkPackets(Buffer.NetPackets pNetworkPackets)
        {
            string key = pNetworkPackets.GetDestinationAddress().ToString();

            if (key == "185.215.4.10" || key == "62.217.160.2")
            {
                if (User.ContainsKeyObject(key))
                {
                    User.SendToChildren(key, pNetworkPackets);
                }
                else
                {
                    if (pNetworkPackets.DestinationPort == 443 || 
                        pNetworkPackets.DestinationPort == 80)
                    {
                        User.AddObject<Connecting>(key, new Buffer.Connecting()
                        {
                            TLSPorts = GetTLSPorts(key),

                            Port1 = GetPort(key),
                            Port2 = GetPort(key)
                        });

                        User.SendToChildren(key, pNetworkPackets);
                    }
                }
            } 
        }

        public Buffer.IndexedByte SubstitutionOfFields(Buffer.NetPackets pNetworkPacketsBuffer)
        {
            return PortController.SubstitutionOfFields(pNetworkPacketsBuffer);
        }



        /// <summary>
        /// Запишем какие номера портов использует клиент на своем устройве, что бы в дальнейшем подменить на них.
        /// </summary>
        public void SetClientTLSPorts(Hellper.VPN.ClientPorts.ClientTLSPorts pClientTLSPortsBuffer)
        {
            PortController.SetClientTLSPorts(pClientTLSPortsBuffer);
        }

        public void SetClientPort(Hellper.VPN.ClientPorts.ClientPort pClientPortBuffer)
        {
            PortController.SetClientPort(pClientPortBuffer);
        }

        private VPN.ClientPorts.TLSPorts GetTLSPorts(string pClientKey)
        {
            if (PortController.TryGetTLSPorts(pClientKey, out VPN.ClientPorts.TLSPorts oTLSPorts, out string oKeyObjectRemove))
            {
                User.RemoveObject(oKeyObjectRemove);
            }

            return oTLSPorts;
        }

        private VPN.ClientPorts.Port GetPort(string pClientKey)
        {
            if (PortController.TryGetPort(pClientKey, out ClientPorts.Port oPort, out string oKeyObjectRemove))
            {
                User.RemoveObject(oKeyObjectRemove);
            }

            return oPort;
        }

        public void FreePort(VPN.ClientPorts.Port pPort)
        {
            PortController.FreePort(pPort);
        }

        public void FreeTLSPorts(ClientPorts.TLSPorts pTLSPorts)
        {
            PortController.FreeTLSPorts(pTLSPorts);
        }
    }
}
