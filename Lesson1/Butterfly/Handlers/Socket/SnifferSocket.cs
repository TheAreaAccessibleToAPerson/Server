using System;
using System.Net;
using System.Net.Sockets;

namespace Butterfly
{
    public sealed class SnifferSocket : HandlerLocalBuffer<Buffer.ServerConfig, Buffer.Empty, Buffer.Byte>
    {
        public struct Data
        {
            public const int BUFFER_SIZE = 256000;
        }

        private Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);

        protected override void Construction(){}

        protected override void Start()
        {
            AddThread(Receive, "Receive", 10, 1);
        }

        private void Receive()
        {
            if (Socket.Available > 0)
            {
                Buffer.Byte byteBuffer = new Buffer.Byte(Data.BUFFER_SIZE);

                try
                {
                    byteBuffer.Size = Socket.Receive(byteBuffer.Array);
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
                finally
                {
                    ToOutput(byteBuffer);
                }
            }
        }

        protected override void Configurate()
        {
            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Parse(Local.Buffer.SnifferAddress), Local.Buffer.SnifferPort));
                Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                Socket.IOControl(IOControlCode.ReceiveAll, new byte[4] { 1, 0, 0, 0 }, new byte[4]);
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
            catch (InvalidCastException ex)
            {
                Exception(ex);
            }
        }

        protected override void Stop()
        {
            Socket.Close();
            Socket = null;
        }
    }
}
