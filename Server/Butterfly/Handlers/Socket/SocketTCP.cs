using System;
using System.Net;
using System.Net.Sockets;

namespace Butterfly
{
        public class SocketTCP : Handler<Buffer.Byte, Buffer.Byte>
        {
            public struct Data
            {
                public const int BUFFER_SIZE = 32000;
            }

            private Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            protected override void Construction()
            {
            }

            public void Connecting(IPAddress pDestinationAddress, int pDestinationPort, int pLocalPort)
            {
                try
                {
                    Socket.Bind(new IPEndPoint(IPAddress.Any, pLocalPort));
                    Socket.Connect(new IPEndPoint(pDestinationAddress, pDestinationPort));
                }
                catch (ArgumentNullException ex)
                {
                    Exception(ex);
                }
                catch (SocketException ex)
                {
                    Exception(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    Exception(ex);
                }
            }

            public void Send(byte[] pByteArray, int pStart, int pEnd)
            { 
                try
                {
                    SocketError socketError = default;
                    Socket.Send(pByteArray, pStart, pEnd - pStart, SocketFlags.None, out socketError);
                }
                catch (ArgumentNullException ex)
                {
                    Exception(ex);
                }
                catch (SocketException ex)
                {
                    Exception(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    Exception(ex);
                }
            }

            protected override void Stop()
            {
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                    Socket = null;
                }
                catch (SocketException ex)
                {
                    Exception(ex);
                }
                catch (ObjectDisposedException ex)
                {
                    Exception(ex);
                }
            }
        }
}
