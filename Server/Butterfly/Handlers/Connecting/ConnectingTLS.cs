using System.Net.Http;

namespace Butterfly
{
    public class ConnectingTLS : HandlerLocalBuffer<Buffer.Connecting, Buffer.NetPacket, Buffer.Empty>
    {
        private struct Data
        {
            public const int SOCKET_1 = 0;
            public const int SOCKET_2 = 1;
            public const int SOCKET_3 = 2;
        }

        private const int DESTINATION_PORT = 443;

        private readonly Hellper.VPN.PacketManager PacketManager = new Hellper.VPN.PacketManager();

        private bool IsSyn1, IsSyn2, IsSyn3;
        private int ServerLocalPort1, ServerLocalPort2, ServerLocalPort3;
        private int ClientPort1, ClientPort2, ClientPort3; // Порт который использует клиент на своем устройсве.

        private System.Net.IPAddress DestinationServerAddress;

        private Buffer.NetPacket FirstClientHello;

        private SocketTCP[] Sockets;

        protected override void Construction()
        {
           
            SetInputStream(Process);

            ServerLocalPort1 = Local.Buffer.TLSPorts.ServerLocalPort_1;
            ServerLocalPort2 = Local.Buffer.TLSPorts.ServerLocalPort_2;
            ServerLocalPort3 = Local.Buffer.TLSPorts.ServerLocalPort_3;

            Sockets = AddHandler<SocketTCP>(Data.SOCKET_1.ToString(), Data.SOCKET_2.ToString(), Data.SOCKET_3.ToString());
        }

        private void Process(Buffer.NetPacket pNetworkPackets) 
        {
            if (pNetworkPackets.IsSyn())
            {
                if (IsSyn1 == false)
                {
                    //Console(pNetworkPackets.GetString("ConnectingTLS:SYN 1"));
                    IsSyn1 = true;
                    DestinationServerAddress = pNetworkPackets.GetDestinationAddress();

                    ClientPort1 = pNetworkPackets.SourcePort;

                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_1,
                        Sequence = pNetworkPackets.SequenceNumber,
                        Acknowledgment = -1
                    });

                    SendToParentListener<ConnectingTLS, Hellper.VPN.ClientPorts.ClientTLSPorts>(new Hellper.VPN.ClientPorts.ClientTLSPorts()
                    {
                        Index = Local.Buffer.TLSPorts.Index,
                        NumberPort = Data.SOCKET_1,
                        Port = ClientPort1
                    });

                    Sockets[Data.SOCKET_1].Connecting(DestinationServerAddress, DESTINATION_PORT, ServerLocalPort1);
                }
                else if (IsSyn2 == false && pNetworkPackets.SourcePort != ClientPort1)
                {
                    IsSyn2 = true;

                    ClientPort2 = pNetworkPackets.SourcePort;

                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_2,
                        Sequence = pNetworkPackets.SequenceNumber,
                        Acknowledgment = -1
                    });

                    SendToParentListener<ConnectingTLS, Hellper.VPN.ClientPorts.ClientTLSPorts>(new Hellper.VPN.ClientPorts.ClientTLSPorts()
                    {
                        Index = Local.Buffer.TLSPorts.Index,
                        NumberPort = Data.SOCKET_2,
                        Port = ClientPort2
                    });

                    Sockets[Data.SOCKET_2].Connecting(DestinationServerAddress, DESTINATION_PORT, ServerLocalPort2);
                }
                else if (IsSyn3 == false && pNetworkPackets.SourcePort != ClientPort1 && pNetworkPackets.SourcePort != ClientPort2)
                {
                    IsSyn3 = true;

                    ClientPort3 = pNetworkPackets.SourcePort;

                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_3,
                        Sequence = pNetworkPackets.SequenceNumber,
                        Acknowledgment = -1
                    });

                    SendToParentListener<ConnectingTLS, Hellper.VPN.ClientPorts.ClientTLSPorts>(new Hellper.VPN.ClientPorts.ClientTLSPorts()
                    {
                        Index = Local.Buffer.TLSPorts.Index,
                        NumberPort = Data.SOCKET_3,
                        Port = ClientPort3
                    });

                    Sockets[Data.SOCKET_3].Connecting(DestinationServerAddress, DESTINATION_PORT, ServerLocalPort3);
                }
            }
            else if (pNetworkPackets.IsAck())
            {  
                /*
                if (pNetworkPackets.SourcePort == ClientPort1)
                {
                    //Console(pNetworkPackets.GetString("ConnectingTLS:ACK 1"));
                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_1,
                        Acknowledgment = pNetworkPackets.AcknowledgmentNumber,
                        Sequence = pNetworkPackets.SequenceNumber
                    });

                    //Console(pNetworkPackets.GetPacketString());
                }
                else if (pNetworkPackets.SourcePort == ClientPort2)
                {
                    //Console(pNetworkPackets.GetString("ConnectingTLS:ACK 2"));
                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_2,
                        Acknowledgment = pNetworkPackets.AcknowledgmentNumber,
                        Sequence = pNetworkPackets.SequenceNumber
                    });
                }
                */
            }
            else if (pNetworkPackets.IsAckPush())
            {
                if (pNetworkPackets.SourcePort == ClientPort1)
                {
                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_1,
                        Acknowledgment = pNetworkPackets.AcknowledgmentNumber,
                        Sequence = -1
                    });

                    Sockets[Data.SOCKET_1].Send(pNetworkPackets.Data, pNetworkPackets.StartIndexSegments, pNetworkPackets.EndIndexSegments);
                }
                else if (pNetworkPackets.SourcePort == ClientPort2)
                {
                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_2,
                        Acknowledgment = pNetworkPackets.AcknowledgmentNumber,
                        Sequence = -1
                    });

                    Sockets[Data.SOCKET_2].Send(pNetworkPackets.Data, pNetworkPackets.StartIndexSegments, pNetworkPackets.EndIndexSegments);
                }
                else if (pNetworkPackets.SourcePort == ClientPort3)
                {
                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_3,
                        Acknowledgment = pNetworkPackets.AcknowledgmentNumber,
                        Sequence = -1
                    });

                    Sockets[Data.SOCKET_3].Send(pNetworkPackets.Data, pNetworkPackets.StartIndexSegments, pNetworkPackets.EndIndexSegments);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
