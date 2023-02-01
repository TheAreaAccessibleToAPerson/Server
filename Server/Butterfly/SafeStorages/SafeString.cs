using System;
using System.Collections.Generic;
using System.Text;

namespace Butterfly
{
    class SafeString
    {
        private const string NONE = "None";

        private object Locker = new object();
        public string Value { private set; get; }

        public SafeString()
        {
            Value = NONE;
        }

        public SafeString(string pValue)
        {
            Value = pValue;
        }

        public string Get()
        {
            return Value;
        }

        public string __Get()
        {
            lock (Locker)
            {
                return Value;
            }
        }

        public string Set(string pValue)
        {
            lock (Locker)
            {
                return Value = pValue;
            }
        }

        public string Define(string pValue)
        {
            lock (Locker)
            {
                if (Value == NONE) Value = pValue;
                return Value;
            }
        }

        public bool Comparison(string pValue)
        {
            lock (Locker)
            {
                return Value == pValue;
            }
        }

        public bool Comparison(string pValue1, string pValue2)
        {
            lock (Locker)
            {
                return Value == pValue1 || Value == pValue2;
            }
        }
        public bool Comparison(string pValue1, string pValue2, string pValue3)
        {
            lock (Locker)
            {
                return Value == pValue1 || Value == pValue2 || Value == pValue3;
            }
        }

        public bool Comparison(string pValue1, string pValue2, string pValue3, string pValue4)
        {
            lock (Locker)
            {
                return Value == pValue1 || Value == pValue2 || Value == pValue3 || Value == pValue4;
            }
        }

        public bool Replace(string pValue, string pReplaceString)
        {
            bool result = true;
            {
                lock (Locker)
                {
                    result = Value == pValue;

                    if (result) Value = pReplaceString;
                }
            }
            return result;  
        }

        public bool Replace(string pValue1, string pValue2, string pReplaceString)
        {
            bool result = true;
            {
                lock (Locker)
                {
                    result = Value == pValue1 || Value == pValue2;

                    if (result) Value = pReplaceString;
                }
            }
            return result;
        }
        public bool Replace(string pValue1, string pValue2,  string  pValue3, string pReplaceString)
        {
            bool result = true;
            {
                lock (Locker)
                {
                    result = Value == pValue1 || Value == pValue2 || Value == pValue3;

                    if (result) Value = pReplaceString;
                }
            }
            return result;
        }

        public bool Replace(string pValue1, string pValue2, string pValue3, string pValue4, string pReplaceString)
        {
            bool result = true;
            {
                lock (Locker)
                {
                    result = Value == pValue1 || Value == pValue2 || Value == pValue3 || Value == pValue4;

                    if (result) Value = pReplaceString;
                }
            }
            return result;
        }
        public bool Replace(string pValue1, string pValue2, string pValue3, string pValue4, string pValue5, string pReplaceString)
        {
            bool result = true;
            {
                lock (Locker)
                {
                    result = Value == pValue1 || Value == pValue2 || Value == pValue3 || Value == pValue4 || Value == pValue5;
                    
                    if (result) Value = pReplaceString;
                }
            }
            return result;
        }
    }
}
