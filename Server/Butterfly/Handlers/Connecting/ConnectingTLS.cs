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

        private void Process(Buffer.NetPacket pNetworkPacket) 
        {
            if (pNetworkPacket.IsSyn())
            {
                if (IsSyn1 == false)
                {
                    IsSyn1 = true;
                    DestinationServerAddress = pNetworkPacket.GetDestinationAddress();

                    ClientPort1 = pNetworkPacket.SourcePort;

                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_1,
                        Sequence = pNetworkPacket.SequenceNumber,
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
                else if (IsSyn2 == false && pNetworkPacket.SourcePort != ClientPort1)
                {
                    IsSyn2 = true;

                    ClientPort2 = pNetworkPacket.SourcePort;

                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_2,
                        Sequence = pNetworkPacket.SequenceNumber,
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
                else if (IsSyn3 == false && pNetworkPacket.SourcePort != ClientPort1 && pNetworkPacket.SourcePort != ClientPort2)
                {
                    IsSyn3 = true;

                    ClientPort3 = pNetworkPacket.SourcePort;

                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_3,
                        Sequence = pNetworkPacket.SequenceNumber,
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
            else if (pNetworkPacket.IsAck())
            {  
                //...
            }
            else if (pNetworkPacket.IsAckPush())
            {
                if (pNetworkPacket.SourcePort == ClientPort1)
                {
                    /*
                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_1,
                        Acknowledgment = pNetworkPacket.AcknowledgmentNumber,
                        Sequence = -1
                    });
                    */

                    Sockets[Data.SOCKET_1].Send(pNetworkPacket.Data, pNetworkPacket.StartIndexSegments, pNetworkPacket.EndIndexSegments);
                }
                else if (pNetworkPacket.SourcePort == ClientPort2)
                {
                    /*
                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_2,
                        Acknowledgment = pNetworkPacket.AcknowledgmentNumber,
                        Sequence = -1
                    });
                    */

                    Sockets[Data.SOCKET_2].Send(pNetworkPacket.Data, pNetworkPacket.StartIndexSegments, pNetworkPacket.EndIndexSegments);
                }
                else if (pNetworkPacket.SourcePort == ClientPort3)
                {
                    /*
                    SendToParentListener<ConnectingTLS, Hellper.VPN.SeqAckNumber>(new Hellper.VPN.SeqAckNumber()
                    {
                        IndexInArray = Local.Buffer.TLSPorts.Index,
                        SocketNumber = Data.SOCKET_3,
                        Acknowledgment = pNetworkPacket.AcknowledgmentNumber,
                        Sequence = -1
                    });
                    */

                    Sockets[Data.SOCKET_3].Send(pNetworkPacket.Data, pNetworkPacket.StartIndexSegments, pNetworkPacket.EndIndexSegments);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
