using System;
using System.Collections.Generic;
using System.Text;

namespace Butterfly
{
    public interface IBuffer
    {
        public struct Data
        {
            public const string NAME = "IBuffer";
            public const string GET_METHOD_NAME = "GetName()";
        }

        public abstract string GetName();
    }
}
