namespace Butterfly.system
{
    public sealed class LocalValue 
    {
        public sealed class LocalBuffer<LocalBufferType>
        {
            public LocalBufferType Buffer { private set; get; }

            private void SetBuffer(System.Object pLocalBuffer, System.Func<string, bool> pException)
            {
                if (pLocalBuffer is LocalBufferType localBufferReduse)
                {
                    Buffer = localBufferReduse;
                }
                else
                    pException(Ex.LB.x100000);
            }

            public abstract class Handler<InputBufferType, OutputBufferType> : HandlerObject<InputBufferType, OutputBufferType>, ILocalBuffer
                where InputBufferType : struct where OutputBufferType : struct
            {
                protected readonly LocalBuffer<LocalBufferType> Local = new LocalBuffer<LocalBufferType>();

                void ILocalBuffer.SetBuffer(System.Object pLocalBuffer) => Local.SetBuffer(pLocalBuffer, Exception);
            }
            public abstract class Controller : ThreadObject, ILocalBuffer
            {
                public Controller(string pTypeObject) : base(pTypeObject) { }

                protected readonly LocalBuffer<LocalBufferType> Local = new LocalBuffer<LocalBufferType>();

                void ILocalBuffer.SetBuffer(System.Object pLocalBuffer) => Local.SetBuffer(pLocalBuffer, Exception);
            }
            public abstract class Analizator<InputBufferType, OutputBufferType> : AnalizatorObject<InputBufferType, OutputBufferType>, ILocalBuffer
                where InputBufferType : struct where OutputBufferType : struct
            {
                protected readonly LocalBuffer<LocalBufferType> Local = new LocalBuffer<LocalBufferType>();

                void ILocalBuffer.SetBuffer(System.Object pLocalBuffer) => Local.SetBuffer(pLocalBuffer, Exception);
            }
        }


        public sealed class LocalBuffers<LocalBufferType>
        {
            public LocalBufferType[] Buffers { private set; get; }

            private void SetBuffers(object[] pLocalBuffers, System.Func<string, bool> pException)
            {
                if (pLocalBuffers is LocalBufferType[] localBuffersReduse)
                {
                    Buffers = localBuffersReduse;
                }
                else
                    pException(Ex.LB.x100001);
            }

            public abstract class Handler<InputBufferType, OutputBufferType> : HandlerObject<InputBufferType, OutputBufferType>, ILocalBuffers
            where InputBufferType : struct where OutputBufferType : struct
            {
                protected readonly LocalBuffers<LocalBufferType> Local = new LocalBuffers<LocalBufferType>();

                void ILocalBuffers.SetBuffers(System.Object[] pLocalBuffers) => Local.SetBuffers(pLocalBuffers, Exception);
            }
            public abstract class Controller : ThreadObject, ILocalBuffers
            {
                public Controller(string pTypeObject) : base(pTypeObject) { }

                protected readonly LocalBuffers<LocalBufferType> Local = new LocalBuffers<LocalBufferType>();

                void ILocalBuffers.SetBuffers(System.Object[] pLocalBuffers) => Local.SetBuffers(pLocalBuffers, Exception);
            }
            public abstract class Analizator<InputBufferType, OutputBufferType> : AnalizatorObject<InputBufferType, OutputBufferType>, ILocalBuffers
                where InputBufferType : struct where OutputBufferType : struct
            {
                protected readonly LocalBuffers<LocalBufferType> Local = new LocalBuffers<LocalBufferType>();

                void ILocalBuffers.SetBuffers(System.Object[] pLocalBuffers) => Local.SetBuffers(pLocalBuffers, Exception);
            }
        }
    }
}
