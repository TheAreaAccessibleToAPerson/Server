namespace Butterfly
{
    public struct ParallelProccesingConfig
    {
        public int TimeDelay;
        public int Count;
    }

    public abstract class ParallelProccesing<BufferType> : HandlerLocalBuffer<ParallelProccesingConfig, BufferType, BufferType> 
        where BufferType : struct
    {
        protected sealed override void Construction() { }

        private void UpdateThread()
        {
            if (TryGetInput(out BufferType[] pBuffers))
            {
                foreach (BufferType buffer in pBuffers)
                {
                    ToOutput(buffer);   
                }
            }
        }
        public sealed class config : ParallelProccesing<BufferType>
        {
            protected override void Start() => AddThread(UpdateThread, "UpdateThread", Local.Buffer.TimeDelay, Local.Buffer.Count);
        }
        public sealed class x10_8 : ParallelProccesing<BufferType>
        {
            protected override void Start() => AddThread(UpdateThread, "UpdateThread", 20, 1);
        }
        public sealed class x20_1 : ParallelProccesing<BufferType>
        {
            protected override void Start() => AddThread(UpdateThread, "UpdateThread", 20, 1);
        }
        public sealed class x20_2 : ParallelProccesing<BufferType>
        {
            protected override void Start() => AddThread(UpdateThread, "UpdateThread", 20, 2);
        }
        public sealed class x200_1 : ParallelProccesing<BufferType>
        {
            protected override void Start() => AddThread(UpdateThread, "UpdateThread", 200, 1);
        }
        public sealed class x100_1 : ParallelProccesing<BufferType>
        { 
            protected override void Start() => AddThread(UpdateThread, "UpdateThread", 100, 1); 
        }
        public sealed class x50_1 : ParallelProccesing<BufferType>
        {
            protected override void Start() => AddThread(UpdateThread, "UpdateThread", 50, 1);
        }
    }
}
