namespace Butterfly.system
{
    public interface IStream : IHeader, System.IDisposable
    {
        abstract System.Action<InputStreamType> GetInputStream<InputStreamType>();
        public virtual System.Action<InputStreamType> GetInputStream<InputStreamType>
            (System.Action<string> pParentConsole, System.Func<string, bool> pParentException)
        { throw new System.NotImplementedException(); }
        
        abstract void AddOutputStream<OutputStreamValue>(System.Action<OutputStreamValue> pOutputStream);
        public virtual void AddOutputStream<OutputStreamValue>(System.Action<OutputStreamValue> pOutputStream, 
            System.Action<string> pParentConsole, System.Func<string, bool> pParentException)
        { throw new System.NotImplementedException(); }

        abstract void SetStream(IStream pStream);
        abstract void SetStreamToParent(IDOM pStream);

        public virtual void ToInput<InputStreamType>(InputStreamType pInputValue)
        { throw new System.NotImplementedException(); }
        public virtual void ToInput<InputStreamType>(InputStreamType[] pInputValue)
        { throw new System.NotImplementedException(); }

        public virtual string GetStreamType()
        { throw new System.NotImplementedException(); }

        public interface InOutput<OutputValueType> : IStream
            where OutputValueType : struct
        {
            public virtual InOutput<OutputValueType> redirect_to(IStream pValue)
            { throw new System.NotImplementedException(); }

            public virtual InOutput<OutputValueType> redirect_to<StreamType>() where StreamType : new()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<StreamType>(object pLocalBuffer) where StreamType : new()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<StreamType>(object[] pLocalBuffers) where StreamType : new()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<StreamType1, StreamType2>() 
                where StreamType1 : new() where StreamType2 : new()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<StreamType1, StreamType2, StreamType3>()
                where StreamType1 : new() where StreamType2 : new() where StreamType3 : new()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<StreamType1, StreamType2, StreamType3, StreamType4>()
                where StreamType1 : new() where StreamType2 : new() where StreamType3 : new() where StreamType4 : new()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<StreamType1, StreamType2, StreamType3, StreamType4, StreamType5>()
                where StreamType1 : new() where StreamType2 : new() where StreamType3 : new() where StreamType4 : new() where StreamType5 : new()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<HandlerType>(System.Func<HandlerType> pValue)
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<InputStreamType, OutputStreamType>(System.Func<InputStreamType, OutputStreamType> pValue)
                where InputStreamType : struct where OutputStreamType : struct
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to(System.Action<OutputValueType> pValue)
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> lock_redirect_to(System.Action<OutputValueType> pValue)
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> redirect_to<Type>(System.Action<Type> pValue)
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> lock_redirect_to<Type>(System.Action<Type> pValue)
            { throw new System.NotImplementedException(); }

            public virtual InOutput<OutputValueType> send_to_parent<OutputStreamType>()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> send_to_parent()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> send_to_children()
            { throw new System.NotImplementedException(); }
            public virtual InOutput<OutputValueType> send_to_childrens()
            { throw new System.NotImplementedException(); }
        }
        public interface InInput<InputValueType> : IStream
            where InputValueType : struct
        {
        }

    }
}
