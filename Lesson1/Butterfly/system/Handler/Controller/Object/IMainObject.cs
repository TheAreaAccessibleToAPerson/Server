namespace Butterfly.system
{
    public interface IMainObject : IStateObject, IDOM, IHeader, IDependency.IStream, IDependency.IObject, IScopeHellper, System.IDisposable
    {
        //...
    }

    public interface IDOM
    {
        void CreatingNode();
        void DestroyNode();

        void RemoveChildren(string pKey);

        void SetDOM(ProgramObject pProgramObject, MainObject pParentObject, string pKeyObject, int pNodeNumber);

        System.Func<PrivateHandlerType> AddPrivateHandler<PrivateHandlerType>() where PrivateHandlerType : new();
        System.Func<PrivateHandlerType> AddPrivateHandler<PrivateHandlerType>(object pLocalBuffer) where PrivateHandlerType : new();
        System.Func<PrivateHandlerType> AddPrivateHandler<PrivateHandlerType>(object[] pLocalBuffers) where PrivateHandlerType : new();
        void SendToParent<BufferType>(BufferType pMessage) where BufferType : struct;
        bool SendToHandler<HandlerType, BufferType>(BufferType pBuffer) where HandlerType : new() where BufferType : struct;
        bool SendToChildrenListener<HandlerType, BufferType>(BufferType pBuffer) where HandlerType : new() where BufferType : struct;
        bool SendToParentListener<HandlerType, BufferType>(BufferType pBuffer) where HandlerType : new() where BufferType : struct;
    }

    public interface IScopeHellper
    {
        void AddObject<ObjectType>() where ObjectType : new();
        void AddObject<ObjectType>(string pKey, object pLocalBuffer) where ObjectType : new();
        void RemoveObject(string pKey);

        void SendToChildren<BufferType>(string pChildrenKey, BufferType pBuffer) where BufferType : struct ;
        bool ContainsKeyObject(string pKey);
    }

    public interface IDependency
    {
        public interface IStream
        {
            void AddDependencyStream(System.Func<bool> pDependencyStream);
        }
        
        public interface IObject
        {
            System.Func<PrivateHandlerType> AddPrivateHandler<PrivateHandlerType>() where PrivateHandlerType : new();
            System.Func<PrivateHandlerType> AddPrivateHandler<PrivateHandlerType>(object pLocalValue) where PrivateHandlerType : new();
            System.Func<PrivateHandlerType> AddPrivateHandler<PrivateHandlerType>(object[] pLocalValues) where PrivateHandlerType : new();
        }
    }
}
