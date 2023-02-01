namespace Butterfly
{
    public sealed class ConnectingHTTP : HandlerLocalBuffer<Buffer.Connecting, Buffer.NetPacket, Buffer.Empty>
    {
        public struct Data
        {
            public const int SOCKET_1 = 0;
            public const int SOCKET_2 = 1;
        }

        private const int DESTINATION_SERVER_PORT = 80;

        private SocketTCP[] Sockets;
        private int ServerLocalPort1, ServerLocalPort2;

        private int ClientPort1, ClientPort2; // Порт который использует клиент на своем устройвсте.

        private bool IsSyn1, IsSyn2;

        private System.Net.IPAddress DestinationServerAddress;

        protected override void Construction()
        {
            SetInputStream(Process);

            ServerLocalPort1 = Local.Buffer.Port1.ServerLocalPort;
            ServerLocalPort2 = Local.Buffer.Port2.ServerLocalPort;

            Sockets = AddHandler<SocketTCP>(Data.SOCKET_1.ToString(), Data.SOCKET_2.ToString());
        }

        private void Process(Buffer.NetPacket pNetworkPackets)
        {
            if (pNetworkPackets.IsSyn())
            {
                if (IsSyn1 == false)
                {
                    IsSyn1 = true;

                    DestinationServerAddress = pNetworkPackets.GetDestinationAddress();

                    SendToParentListener<ConnectingHTTP, Hellper.VPN.ClientPorts.ClientPort>(new Hellper.VPN.ClientPorts.ClientPort()
                    {
                        Index = Local.Buffer.Port1.Index,
                        Port = ClientPort1 = pNetworkPackets.SourcePort
                    });

                    SendToParentListener<ConnectingHTTP, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.Port1.Index,
                        SocketNumber = Data.SOCKET_1,
                        Acknowledgment = pNetworkPackets.SequenceNumber + 1,
                    });

                    Sockets[Data.SOCKET_1].Connecting(DestinationServerAddress, DESTINATION_SERVER_PORT, ServerLocalPort1);
                }
                else if (IsSyn2 == false && ClientPort1 != pNetworkPackets.SourcePort)
                {
                    IsSyn2 = true;

                    SendToParentListener<ConnectingHTTP, Hellper.VPN.ClientPorts.ClientPort>(new Hellper.VPN.ClientPorts.ClientPort()
                    {
                        Index = Local.Buffer.Port2.Index,
                        Port = ClientPort2 = pNetworkPackets.SourcePort
                    });

                    SendToParentListener<ConnectingHTTP, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.Port2.Index,
                        SocketNumber = Data.SOCKET_2,
                        Acknowledgment = pNetworkPackets.SequenceNumber + 1,
                    });

                    Sockets[Data.SOCKET_2].Connecting(DestinationServerAddress, DESTINATION_SERVER_PORT, ServerLocalPort2);
                }
            }
            else if (pNetworkPackets.IsAckPush())
            {
                if (pNetworkPackets.SourcePort == ClientPort1)
                {
                    SendToParentListener<ConnectingHTTP, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.Port1.Index,
                        SocketNumber = Data.SOCKET_1,
                        Sequence = pNetworkPackets.AcknowledgmentNumber
                    });
                   
                    Sockets[Data.SOCKET_1].Send(pNetworkPackets.Data, pNetworkPackets.StartIndexSegments, pNetworkPackets.EndIndexSegments);
                }
                else if (pNetworkPackets.SourcePort == ClientPort2)
                {
                    SendToParentListener<ConnectingHTTP, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.Port2.Index,
                        SocketNumber = Data.SOCKET_2,
                        Sequence = pNetworkPackets.AcknowledgmentNumber
                    });

                    Sockets[Data.SOCKET_2].Send(pNetworkPackets.Data, pNetworkPackets.StartIndexSegments, pNetworkPackets.EndIndexSegments);
                }
            }
        }
    }
}
