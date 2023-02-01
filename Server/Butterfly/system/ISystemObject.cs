using System;

namespace Butterfly.system
{
    public interface IHeader
    {
        abstract string GetNameObject();
        abstract string GetTypeObject();
        abstract string GetExplorerObject();
        abstract bool IsTypeObject(string pType);
    }
    public interface ILocalBuffer
    {
        void SetBuffer(Object pLocalBuffer);
    }

    public interface ILocalBuffers
    {
        void SetBuffers(Object[] pLocalBuffers);
    }
}


