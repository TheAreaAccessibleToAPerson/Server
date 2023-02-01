namespace Butterfly
{
    public sealed class ServerSocketTCP : HandlerLocalBuffer<Buffer.ServerConfig, Buffer.Client, Buffer.Client>
    {
        private System.Net.Sockets.Socket Socket;
        private  System.Net.IPEndPoint LocalIPEndPoint;

        protected override void Construction() 
        {
            Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, 
                System.Net.Sockets.ProtocolType.Tcp);

            LocalIPEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(Local.Buffer.AddressServer), Local.Buffer.PortServer);
        }

        protected override void Start()
        {
            AddThread(Accept, "Accept", 0, 1, BreakTheBlockAccept);
        }

        private void Accept()
        {
            Buffer.Client clientBuffer = new Buffer.Client();

            try
            {
                clientBuffer.Socket = Socket.Accept();
                clientBuffer.Address = ((System.Net.IPEndPoint)clientBuffer.Socket.RemoteEndPoint).Address;
                clientBuffer.Port = ((System.Net.IPEndPoint)clientBuffer.Socket.RemoteEndPoint).Port;
            }
            catch (System.ArgumentNullException ex)
            {
                Exception(ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Exception(ex);
            }
            catch (System.ObjectDisposedException ex)
            {
                Exception(ex);
            }
            finally
            {
                ToOutput(clientBuffer);
            }
        }

        private void BreakTheBlockAccept()
        {
            try
            {
                System.Net.Sockets.Socket s = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, 
                    System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                s.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(Local.Buffer.AddressServer), Local.Buffer.PortServer));

                s.Close();
                s = null;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Exception(ex);
            }
            catch (System.ObjectDisposedException ex)
            {
                Exception(ex);
            }
        }

        protected override void Stop()
        {
            try
            {
                Socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                Socket.Close();
                Socket = null;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Exception(ex);
            }
            catch (System.ObjectDisposedException ex)
            {
                Exception(ex);
            }
        }

        protected override void Configurate()
        {
            try
            {
                Socket.Bind(LocalIPEndPoint);
                Socket.Listen(10);
            }
            catch (System.ArgumentNullException ex)
            {
                Exception(ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Exception(ex);
            }
            catch (System.ObjectDisposedException ex)
            {
                Exception(ex);
            }
        }
    }
}
