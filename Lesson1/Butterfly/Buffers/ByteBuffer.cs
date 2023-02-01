namespace Butterfly.Buffer
{
    public struct Byte : IBuffer
    {
        public byte[] Array;
        public int Size;

        public Byte(int pByteArraySizeBuffer)
        {
            Array = new byte[pByteArraySizeBuffer];
            Size = -1;
        }

        public Byte(byte[] pArray)
        {
            Array = pArray;
            Size = pArray.Length;
        }

        public string GetName()
        {
            return typeof(Byte).Name;
        }
    }

    public struct IndexedByte : IBuffer
    {
        public readonly byte[] ByteArray;
        public readonly int Start;
        public readonly int End;

        public bool Empty;

        public IndexedByte(byte[] pByteBuffer, int pStart, int pEnd)
        {
            Empty = false;

            Start = pStart;
            End = pEnd;
            ByteArray = pByteBuffer;
        }

        public string GetName()
        {
            return typeof(IndexedByte).Name;
        }
    }
}