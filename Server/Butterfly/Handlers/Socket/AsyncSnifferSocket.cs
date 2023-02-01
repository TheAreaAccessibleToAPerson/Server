using System;
using System.Net;
using System.Net.Sockets;

namespace Butterfly
{
    public sealed class AsyncSnifferSocket : HandlerLocalBuffer<Buffer.ServerConfig, Buffer.Empty, Buffer.Byte>
    {
        public struct Data
        {
            public const int BUFFER_SIZE = 256000;
        }

        private Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);

        protected override void Construction(){}

        protected override void Start()
        {
            StartAsyncReceive();
        }

        private void StartAsyncReceive()
        {
            try
            {
                Buffer.Byte byteBuffer = new Buffer.Byte(Data.BUFFER_SIZE);
                Socket.BeginReceive(byteBuffer.Array, 0, byteBuffer.Array.Length, SocketFlags.None, new AsyncCallback(AsyncReceive), byteBuffer);
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

        private void AsyncReceive(IAsyncResult aResult)
        {
            Buffer.Byte byteBufferResult = (Buffer.Byte)aResult.AsyncState;
            byteBufferResult.Size = Socket.EndReceive(aResult);

            ToOutput(byteBufferResult);

            try
            {
                Buffer.Byte byteBuffer = new Buffer.Byte(Data.BUFFER_SIZE);
                Socket.BeginReceive(byteBuffer.Array, 0, byteBuffer.Array.Length, SocketFlags.None, new AsyncCallback(AsyncReceive), byteBuffer);
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


        protected override void Configurate()
        {
            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Parse(Local.Buffer.SnifferAddress), Local.Buffer.SnifferPort));
                Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                Socket.IOControl(IOControlCode.ReceiveAll, new byte[4] { 1, 0, 0, 0 }, new byte[4] { 0, 0, 0, 0 });
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
