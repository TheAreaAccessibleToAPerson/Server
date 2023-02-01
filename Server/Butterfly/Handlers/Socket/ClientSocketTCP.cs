using System.Net.Sockets;
using System.Threading;

namespace Butterfly
{ 
    public abstract class ClientSocketTCP : HandlerLocalBuffer<Buffer.Client, Buffer.IndexedByte, Buffer.Byte>
    {
        private Socket Socket;

        protected sealed override void Construction()
        {
            SetInputStream(Send);
        }

        protected override void Start()
        {
            Socket = Local.Buffer.Socket;
        
            AddThread(EchoSend, "EchoSend", 5000, 1);
        }

        public sealed class x10 : ClientSocketTCP
        {
            protected override void Start()
            {
                AddThread(Receive, "Receive", 10);
                base.Start();
            }
        }
        public sealed class x15 : ClientSocketTCP
        {
            protected override void Start()
            {
                AddThread(Receive, "Receive", 15);
                base.Start();
            }
        }
        public sealed class x20 : ClientSocketTCP
        {
            protected override void Start()
            {
                AddThread(Receive, "Receive", 20);
                base.Start();
            }
        }
        public sealed class x25 : ClientSocketTCP
        {
            protected override void Start()
            {
                AddThread(Receive, "Receive", 25);
                base.Start();
            }
        }

        protected void Receive()
        {
            if (Socket.Available > 0)
            {
                Buffer.Byte byteBuffer = new Buffer.Byte(Buffer.Client.BUFFER_SIZE);

                try
                {
                    byteBuffer.Size = Socket.Receive(byteBuffer.Array);
                }
                catch (System.ArgumentNullException ex)
                {
                    Exception(ex);
                }
                catch (SocketException ex)
                {
                    Exception(ex);
                }
                catch (System.ObjectDisposedException ex)
                {
                    Exception(ex);
                }
                finally
                {
                    ToOutput(byteBuffer);
                }
            }
        }

        private void Send(Buffer.IndexedByte pByteBuffer)
        {
            if (pByteBuffer.Empty) return;

            try
            {
                SocketError socketError = default;
                Socket.Send(pByteBuffer.ByteArray, pByteBuffer.Start, pByteBuffer.End - pByteBuffer.Start, SocketFlags.None, out socketError);
            }
            catch (System.ArgumentNullException ex)
            {
                Exception(ex);
            }
            catch (SocketException ex)
            {
                Exception(ex);
            }
            catch (System.ObjectDisposedException ex)
            {
                Exception(ex);
            }
        }

        private void EchoSend()
        {
            try
            {
                //Socket.Send(new byte[1] { 0 });
            }
            catch (System.ArgumentNullException ex)
            {
                Exception(ex);
            }
            catch (SocketException ex)
            {
                Exception(ex);
            }
            catch (System.ObjectDisposedException ex)
            {
                Exception(ex);
            }
        }

        protected sealed override void Stop()
        {
            Socket.Close();
            Socket = null;
        }
    }
}
