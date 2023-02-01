using Butterfly.SafeStorage;
using System.Collections.Generic;

namespace Butterfly
{
    public class SafeList<ValueType> : ISafeStorage<ValueType>
    {
        private readonly List<ValueType> List = new List<ValueType>();

        private readonly object Locker = new object();

        private bool IsClose = false;

        public void Close()
        {
            lock(Locker)
            {
                IsClose = true;
            }
        }

        public void Clear()
        {
            lock (Locker)
            {
                List.Clear();
            }
        }

        public void Add(ValueType pValue)
        {
            lock (Locker)
            {
                if (!IsClose)
                {
                    List.Add(pValue);
                }    
            }
        }
        
        public bool ExtractAll(out ValueType[] oResult)
        {
            oResult = default;
            bool result = true;
            {
                if (List.Count == 0)
                {
                    result = false;
                }
                else
                {
                    lock (Locker)
                    {
                        if (List.Count == 0)
                        {
                            result = false;
                        }
                        else
                        {

                            ValueType[] array = new ValueType[List.Count];
                            {
                                for (int i = 0; i < List.Count; i++)
                                {
                                    array[i] = List[i];
                                }
                            }

                            oResult = array;

                            List.Clear();
                        }
                    }
                }
            }
            return result;
        }  
    }
}

