namespace Butterfly
{
    public class SafeArray<ValueType> : SafeStorage.ISafeStorage<ValueType> 
    {
        private const int ARRAY_MAX_COUNT = 1024;
        
        private ValueType[] Array = new ValueType[ARRAY_MAX_COUNT];
        private int ArrayCount = 0;

        private readonly object Locker = new object();

        public void Clear()
        {
            lock (Locker)
            {
                Array = null;
            }
        }

        public void Add(ValueType pValue)
        {
            lock (Locker)
            {
                Array[ArrayCount++] = pValue;
            }
        }

        public bool ExtractAll(out ValueType[] oResult)
        {
            oResult = default;

            bool result = true;
            {
                if (ArrayCount == 0)
                {
                    result = false;
                }
                else
                {
                    lock (Locker)
                    {
                        if (ArrayCount == 0)
                        {
                            result = false;
                        }
                        else
                        {
                            ValueType[] array = new ValueType[ArrayCount];
                            {
                                for (int i = 0; i < ArrayCount; i++)
                                {
                                    array[i] = Array[i];
                                }
                            }

                            ArrayCount = 0;

                            oResult = array;
                        }
                    }
                }
            }
            return result;
        }
    }
}
