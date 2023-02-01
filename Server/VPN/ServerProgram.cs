namespace Butterfly
{
    public sealed class ServerProgram : ProgramController
    {
        protected override void Start()
        {
            AddObject<Server>(new Buffer.ServerConfig()
            {
                AddressServer = "192.168.31.158",
                PortServer = 11,

                SnifferAddress = "192.168.31.158",
                SnifferPort = 0
            });
        }
    } 

    public sealed class Server : ControllerLocalBuffer<Buffer.ServerConfig>
    {
        private readonly Hellper.VPN.ConnectionManager ConnectionManager = new Hellper.VPN.ConnectionManager();

        protected override void Construction()
        {
            AddHellper(ConnectionManager.Creating(150, 25, 25));

            AddHandler<ServerSocketTCP>(Local.Buffer)
                .redirect_to<ParallelProccesing<Buffer.Client>.x100_1>()
                .redirect_to<Buffer.Client>(ConnectionManager.AddClient);
           
            /*
            AddHandler<AsyncSnifferSocket>(Local.Buffer)
                .redirect_to<ParallelProccesing<Buffer.Byte>.x20_1>()
                .redirect_to<Analiz.NetPackets>()
                .redirect_to<Buffer.NetPackets>(ConnectionManager.SendPacketToClient);
            */

            AddHandler<Echo<Buffer.Client>.Children>()
                .redirect_to(ConnectionManager.FreePorts);
        }
    }   

    public sealed class Client : IndependentControllerLocalBuffer<Buffer.Client>
    {
        private readonly Hellper.VPN.ConnectionController ConnectionController = new Hellper.VPN.ConnectionController();

        protected override void Construction()
        {
            AddHellper(ConnectionController.Creating(Local.Buffer));
           
            AddHandler<ClientSocketTCP.x10>(Local.Buffer)
                .redirect_to<ParallelProccesing<Buffer.Byte>.x20_1>()
                .redirect_to<Analiz.NetPackets>()
                .redirect_to<Buffer.NetPackets>(ConnectionController.ReceiveNetworkPackets);

            AddHandler<Echo<Buffer.NetPackets>.Parent>()
                .redirect_to<Buffer.NetPackets, Buffer.IndexedByte>(ConnectionController.SubstitutionOfFields)
                .redirect_to(GetHandler<ClientSocketTCP.x10>);

            AddHandler<Echo<Hellper.VPN.ClientPorts.ClientTLSPorts>.Children<ConnectingTLS>>()
                .redirect_to(ConnectionController.SetClientTLSPorts);

            AddHandler<Echo<Hellper.VPN.ClientPorts.ClientPort>.Children<ConnectingHTTP>>()
                .redirect_to(ConnectionController.SetClientPort);

            AddHandler<Echo<Hellper.VPN.SeqAckNumber>.Children<ConnectingTLS>>()
                .redirect_to(ConnectionController.SetSeqAckNumberTLSConnection);

            AddHandler<Echo<Hellper.VPN.SeqAckNumber>.Children<ConnectingHTTP>>()
                .redirect_to(ConnectionController.SetSeqAckNumberHTTPConnection);

            AddHandler<Echo<Hellper.VPN.ClientPorts.Port>.Children>()
                .redirect_to(ConnectionController.FreePort);

            AddHandler<Echo<Hellper.VPN.ClientPorts.TLSPorts>.Children>()
                .redirect_to(ConnectionController.FreeTLSPorts);
        }

        protected override void Stop() => SendToParent(Local.Buffer);
    }

    public sealed class Connecting : IndependentControllerLocalBuffer<Buffer.Connecting>
    {
        protected override void Construction()
        {
            AddHandler<ConnectingTLS, ConnectingHTTP>(Local.Buffer);
           
            AddHandler<Echo<Buffer.NetPackets>.Parent>()
                .redirect_to((networkPackets) => 
                {
                    if (networkPackets.DestinationPort == 443)
                    {
                        GetHandler<ConnectingTLS>().input_to(networkPackets);
                    }
                    else if (networkPackets.DestinationPort == 80)
                    {
                        GetHandler<ConnectingHTTP>().input_to(networkPackets);
                    }
                });
        }

        protected override void Stop() => SendToParent(Local.Buffer.TLSPorts);
    }
}
