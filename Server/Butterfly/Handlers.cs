namespace Butterfly
{
    public abstract class Handler<InputBufferType, OutputBufferType> : system.HandlerObject<InputBufferType, OutputBufferType>
        where InputBufferType : struct where OutputBufferType : struct
    { }
    public abstract class HandlerLocalBuffer<LocalBufferType, InputBufferType, OutputBufferType> 
        : system.LocalValue.LocalBuffer<LocalBufferType>.Handler<InputBufferType, OutputBufferType>
            where InputBufferType : struct where OutputBufferType : struct
    { }
    public abstract class HandlerLocalBuffers<LocalBufferType, InputBufferType, OutputBufferType>
        : system.LocalValue.LocalBuffers<LocalBufferType>.Handler<InputBufferType, OutputBufferType>
            where InputBufferType : struct where OutputBufferType : struct
    { }

    public abstract class Echo<BufferType> : Handler<BufferType, BufferType> where BufferType : struct
    {   
        protected sealed override void Construction() => SetInputStream(ToOutput);

        public sealed class Parent : Echo<BufferType> { }
        public sealed class Parent<ParentType> : Echo<BufferType> where ParentType : class { }
        public sealed class Children : Echo<BufferType> { }
        public sealed class Children<ChildrenType> : Echo<BufferType> where ChildrenType : class { }
    }
}
