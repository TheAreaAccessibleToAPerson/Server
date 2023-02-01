namespace Butterfly.system
{
    public interface IHandler<InputValueType, OutputValueType> : IStream, IStream.InInput<InputValueType>, IStream.InOutput<OutputValueType>
        where InputValueType : struct where OutputValueType : struct
    { }
    public abstract class HandlerObject<InputBufferType, OutputBufferType> : ThreadObject , IHandler<InputBufferType, OutputBufferType>  
        where InputBufferType : struct where OutputBufferType : struct
    {
        public HandlerObject() : base(TypeObjectData.HANDLER)
        {
            LocalInputStream = ToInput;
            LocalOutputStream = ToOutput;

            ((IDependency.IStream)this).AddDependencyStream(DependencyStream);
        }

        private struct StreamData
        {
            public const string DEPENDENCY_CALL_OBJECT = "DependencyObject";

            public const string DEPENDENCY_HANDLER = "DependencyHandler";
            public const string DEPENDENCY_ANALIZATOR = "DependencyAnalizator";
            public const string DEPENDENCY_FUNC = "DependencyFunc";
        }

        private readonly SafeList<InputBufferType> InputValueBuffer = new SafeList<InputBufferType>();
        private readonly SafeList<OutputBufferType> OutputValueBuffer = new SafeList<OutputBufferType>();

        private System.Action<InputBufferType> LocalInputStream;
        private System.Action<OutputBufferType> LocalOutputStream;

        private readonly object[] LocalLockerArray = new object[128];
        private int LocalLockerCount = 0;

        private System.Action<InputBufferType> InputStream;
        private readonly System.Action<OutputBufferType>[] OutputStreamArray = new System.Action<OutputBufferType>[32];
        private int OutputStreamCount = 0;

        private readonly IStream[] DependencyAnalizators = new IStream[128];
        private readonly System.Func<IStream>[] DependencyHandlers = new System.Func<IStream>[128];
        private readonly IStream[] DependencyFuncs = new IStream[128];
        private readonly System.Action[] DependencyCallObjects = new System.Action[128];
        private int DependencyCount = 0;

        public void input_to(InputBufferType pValue)
        {
            ToInput(pValue);
        }

        private void ToInput(InputBufferType pValue)
        {
            if (__StartProcess)
            {
                if (InputStream == null)
                {
                    InputValueBuffer.Add(pValue);
                }
                else
                { 
                    InputStream.Invoke(pValue);
                }
            } 
        }
        private void ToInput(InputBufferType[] pValue)
        {
            for (int i = 0; i < pValue.Length; i++) ToInput(pValue[i]);
        }
        protected bool TryGetInput(out InputBufferType[] oInputValue)
        {
            bool result = InputValueBuffer.ExtractAll(out oInputValue);

            return result;
        }
        protected void ToOutput(OutputBufferType pValue)
        {
            if (OutputStreamCount > 0)
            {
                for (int i = 0; i < OutputStreamCount; i++)
                {
                    OutputStreamArray[i](pValue);
                }
            }
            else
            {
                OutputValueBuffer.Add(pValue);
            }
        }

        protected void SetInputStream(System.Action<InputBufferType> pInputStream)
        {
            if (__IsCreating)
            {
                if (InputStream == null)
                {
                    InputStream = pInputStream;
                }    
                else
                    Exception(Ex.HNDR.x100008);
            }
            else 
                Exception(Ex.HNDR.x100007);
        }

        #region Dependency

        #region Stream

        private System.Collections.Generic.List<string> DependencyTypeList 
            = new System.Collections.Generic.List<string>();

        private bool DependencyStream()
        {
            bool result = true;
            {
                IStream prevRedirectStream = this;
                IStream currentRedirectStream = prevRedirectStream;
                string prevDependency = default;

                int dependencyIndex = 0;
                foreach (string dependencyType in DependencyTypeList)
                {
                    if (dependencyType == StreamData.DEPENDENCY_CALL_OBJECT)
                    {
                        DependencyCallObjects[dependencyIndex++].Invoke();
                    }
                    else if (dependencyType == StreamData.DEPENDENCY_ANALIZATOR)
                    {
                        currentRedirectStream = DependencyAnalizators[dependencyIndex++];

                        if (prevDependency == default)
                        {
                            OutputStreamArray[OutputStreamCount++] = currentRedirectStream.GetInputStream<OutputBufferType>(Console, Exception);
                        }
                        else
                        {
                            prevRedirectStream.SetStream(currentRedirectStream);
                        }
                    }
                    else if (dependencyType == StreamData.DEPENDENCY_HANDLER)
                    {
                        currentRedirectStream = DependencyHandlers[dependencyIndex++].Invoke();

                        if (prevDependency == default)
                        {
                            OutputStreamArray[OutputStreamCount++] = currentRedirectStream.GetInputStream<OutputBufferType>();
                        }
                        else
                        {
                            prevRedirectStream.SetStream(currentRedirectStream);
                        }
                    }
                    else if (dependencyType == StreamData.DEPENDENCY_FUNC)
                    {
                        currentRedirectStream = DependencyFuncs[dependencyIndex++];

                        if (prevDependency == default)
                        {
                            OutputStreamArray[OutputStreamCount++] = currentRedirectStream.GetInputStream<OutputBufferType>();
                        }
                        else
                        {
                            prevRedirectStream.SetStream(currentRedirectStream);
                        }
                    }

                    prevDependency = dependencyType;
                    prevRedirectStream = currentRedirectStream;
                }
            }
            return result;
        }

        #endregion

        #endregion

        #region Stream

        void IStream.SetStream(IStream pStream)
        {
            if (pStream.GetInputStream<OutputBufferType>() is System.Action<OutputBufferType> outputStreamReduse)
            {
                OutputStreamArray[OutputStreamCount++] = outputStreamReduse;
            }
            else Exception(Ex.STRM.x100000);
        }
        System.Action<InputStreamType> IStream.GetInputStream<InputStreamType>()
        {
            if (LocalInputStream is System.Action<InputStreamType> inputStreamReduse)
            {
                return inputStreamReduse;
            }
            else Exception(Ex.STRM.x100001);

            return default;
        }
        void IStream.AddOutputStream<OutputStreamValue>(System.Action<OutputStreamValue> pOutputStream)
        {
            if (pOutputStream is System.Action<OutputBufferType> outputStreamReduse)
            {
                OutputStreamArray[OutputStreamCount++] = outputStreamReduse;
            }
            else Exception(Ex.STRM.x100000);
        }
        void IStream.SetStreamToParent(IDOM pStream)
        {
            OutputStreamArray[OutputStreamCount++] = pStream.SendToParent;
        }

        void IStream.ToInput<InputStreamType>(InputStreamType pInputValue)
        {
            if (pInputValue is InputBufferType inputValueReduse)
            {
                ToInput(inputValueReduse);
            }
            else
                Exception(Ex.STRM.x100002);
        }
        void IStream.ToInput<InputStreamType>(InputStreamType[] pInputValue)
        {
            if (pInputValue is InputBufferType[] inputValueReduse)
            {
                ToInput(inputValueReduse);
            }
            else
                Exception(Ex.STRM.x100002);
        }
        public virtual System.Action<InputStreamType> GetInputStream<InputStreamType>(System.Action<string, bool> pParentConsole, 
            System.Func<string, bool> pParentException)
        {
            return ((IStream)this).GetInputStream<InputStreamType>();
        }

        public virtual IStream.InOutput<OutputBufferType> redirect_to(IStream pValue)
        {
            DependencyAnalizators[DependencyCount++] = pValue;
            DependencyTypeList.Add(StreamData.DEPENDENCY_ANALIZATOR);

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> redirect_to<StreamType>() where StreamType : new()
        {
            StreamType stream = new StreamType();
            
            if (stream is IHeader streamHeader)
            {
                if (streamHeader.GetTypeObject() == Analizator.Type)
                {
                    if (stream is IStream streamReduse)
                    {
                        redirect_to(streamReduse);
                    }
                    else
                        Exception(Ex.HNDR.x100003);
                }
                else if (streamHeader.GetTypeObject() == TypeObjectData.HANDLER)
                {
                    System.Func<StreamType> handler = ((IDOM)this).AddPrivateHandler<StreamType>();

                    redirect_to(handler);
                }
                else
                    Exception(Ex.HNDR.x100016);
            }

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> redirect_to<StreamType>(object pLocalBuffer) where StreamType : new()
        {
            StreamType stream = new StreamType();
            if (stream is IHeader streamHeader)
            {
                if (streamHeader.GetTypeObject() == Analizator.Type)
                {
                    if (stream is ILocalBuffer analizatorLocalBufferReduse)
                    {
                        analizatorLocalBufferReduse.SetBuffer(pLocalBuffer);

                        if (stream is IStream streamReduse)
                        {
                            redirect_to(streamReduse);
                        }
                        else
                            Exception(Ex.HNDR.x100003);
                    }
                    else
                        Exception(Ex.HNDR.x100017, streamHeader.GetNameObject());
                }
                else if (streamHeader.GetTypeObject() == TypeObjectData.HANDLER)
                {
                    System.Func<StreamType> handler = ((IDOM)this).AddPrivateHandler<StreamType>();
                    redirect_to(handler);
                }
                else
                    Exception(Ex.HNDR.x100016);
            }


            return this;
        }

        public virtual IStream.InOutput<OutputBufferType> redirect_to<StreamType1, StreamType2>()
                where StreamType1 : new() where StreamType2 : new()
        {
            redirect_to<StreamType1>(); redirect_to<StreamType2>();

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> redirect_to<StreamType1, StreamType2, StreamType3>()
            where StreamType1 : new() where StreamType2 : new() where StreamType3 : new()
        {
            redirect_to<StreamType1>(); redirect_to<StreamType2>(); redirect_to<StreamType3>();

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> redirect_to<StreamType1, StreamType2, StreamType3, StreamType4>()
            where StreamType1 : new() where StreamType2 : new() where StreamType3 : new() where StreamType4 : new()
        {
            redirect_to<StreamType1>(); redirect_to<StreamType2>(); redirect_to<StreamType3>(); redirect_to<StreamType4>();

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> redirect_to<StreamType1, StreamType2, StreamType3, StreamType4, StreamType5>()
            where StreamType1 : new() where StreamType2 : new() where StreamType3 : new() where StreamType4 : new() where StreamType5 : new()
        {
            redirect_to<StreamType1>(); redirect_to<StreamType2>(); redirect_to<StreamType3>(); redirect_to<StreamType4>();
            redirect_to<StreamType5>();

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> redirect_to<InputStreamType, OutputStreamType>(System.Func<InputStreamType, OutputStreamType> pValue)
            where InputStreamType : struct where OutputStreamType : struct
        {
            DependencyFuncs[DependencyCount++] = new FuncObject<InputStreamType, OutputStreamType>(pValue, GetExplorerObject(), Exception);
            DependencyTypeList.Add(StreamData.DEPENDENCY_FUNC);

            return this;
        }

        public virtual IStream.InOutput<OutputBufferType> redirect_to<HandlerType>(System.Func<HandlerType> pValue)
        {
            if (pValue is System.Func<IStream> handlerReduse)
            {
                DependencyHandlers[DependencyCount++] = handlerReduse;
                DependencyTypeList.Add(StreamData.DEPENDENCY_HANDLER);
            }

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> redirect_to(System.Action<OutputBufferType> pValue)
        {
            if (DependencyCount == 0)
            {
                ((IStream)this).AddOutputStream(pValue);
            }
            else Exception(Ex.HNDR.x100009);

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> lock_redirect_to(System.Action<OutputBufferType> pValue)
        {
            if (DependencyCount == 0)
            {
                LocalLockerArray[LocalLockerCount++] = new object();

                System.Action<OutputBufferType> action = (outputValue) => { lock (LocalLockerArray[LocalLockerCount - 1]) pValue(outputValue); };

                ((IStream)this).AddOutputStream(action);
            }
            else Exception(Ex.HNDR.x100013);

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> redirect_to<Type>(System.Action<Type> pValue)
        {
            if (DependencyCount == 0)
            {
                Exception(Ex.HNDR.x100015);
            }
            else
            {
                for (int i = DependencyCount - 1; i >= 0; i--)
                {
                    string lastStreamDependency = DependencyTypeList[i];

                    if (lastStreamDependency == StreamData.DEPENDENCY_ANALIZATOR)
                    {
                        DependencyAnalizators[DependencyCount - 1].AddOutputStream(pValue, Console, Exception); break;
                    }
                    else if (lastStreamDependency == StreamData.DEPENDENCY_HANDLER)
                    {
                        DependencyTypeList.Add(StreamData.DEPENDENCY_CALL_OBJECT);

                        int r = DependencyCount - 1;
                        System.Action action = () => { DependencyHandlers[r].Invoke().AddOutputStream(pValue); };

                        DependencyCallObjects[DependencyCount++] = action;

                        break;
                    }
                    else if (lastStreamDependency == StreamData.DEPENDENCY_FUNC)
                    {
                        DependencyFuncs[DependencyCount - 1].AddOutputStream(pValue); break;
                    }
                }
            }

            return this;
        }
        public virtual IStream.InOutput<OutputBufferType> lock_redirect_to<Type>(System.Action<Type> pValue)
        {
            LocalLockerArray[LocalLockerCount++] = new object();
            System.Action<Type> action = (outputValue) => { lock (LocalLockerArray[LocalLockerCount - 1]) pValue(outputValue); };

            redirect_to(action);

            return this;
        }

        public virtual IStream.InOutput<OutputBufferType> send_to_parent()
        {
            int lastDependencyIndex = DependencyTypeList.Count;

            if (lastDependencyIndex == 0)
            {
                OutputStreamArray[OutputStreamCount++] = ((IDOM)this).SendToParent;
            }
            else
            {
                string dependency = DependencyTypeList[lastDependencyIndex - 1];

                if (dependency == StreamData.DEPENDENCY_CALL_OBJECT)
                {
                    Exception(Ex.HNDR.x100004);
                }
                else if (dependency == StreamData.DEPENDENCY_ANALIZATOR)
                {
                    DependencyAnalizators[DependencyCount - 1].SetStreamToParent(this);
                }
                else if (dependency == StreamData.DEPENDENCY_FUNC)
                {
                    DependencyFuncs[DependencyCount - 1].SetStreamToParent(this);
                }
                else if (dependency == StreamData.DEPENDENCY_HANDLER)
                {
                    DependencyHandlers[DependencyCount - 1].Invoke().SetStreamToParent(this);
                }
            }

            return this;
        }

        #endregion

        #region System

        protected override void StartObj()
        {
            base.StartObj();
        }

        #endregion
    }
}

