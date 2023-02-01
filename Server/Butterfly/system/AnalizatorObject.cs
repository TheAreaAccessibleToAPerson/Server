namespace Butterfly.system
{
    public struct Analizator
    {
        public const string Type = "Analizator";
    }

    public abstract class AnalizatorObject<InputBufferType, OutputBufferType> : IHeader, IStream, IStream.InOutput<OutputBufferType>, System.IDisposable
        where InputBufferType : struct where OutputBufferType : struct
    {
        #region Header

        private readonly string TypeObject;
        private readonly string NameObject;
        private readonly string Namespace;
        private readonly string Explorer;

        public string GetNameObject() => NameObject;
        public string GetTypeObject() => TypeObject;
        public string GetExplorerObject() => Explorer;
        public bool IsTypeObject(string pType) => TypeObject == pType;

        public AnalizatorObject()
        {
            TypeObject = Analizator.Type;

            Namespace = GetType().ToString();

            string[] namespaceSplit = Namespace.Split('.', 2);
            if (namespaceSplit[0] == Global.NAMESPACE && namespaceSplit.Length == 2)
            {
                NameObject = namespaceSplit[1];

                Explorer = $"[{TypeObject}]{NameObject}";
            }
            else
                Exception(Ex.MOBJ.x100029);

            InputStream = Process;
        }

        #endregion

        private System.Action<string> ParentConsole = null;
        private System.Func<string, bool> ParentException = null;

        private System.Action<InputBufferType> InputStream;
        private System.Action<OutputBufferType>[] OutputStreamArray = new System.Action<OutputBufferType>[32]; private int OutputStreamCount = 0;

        protected abstract void Process(InputBufferType pValue);

        protected void Add(OutputBufferType pValue)
        {
            for (int i = 0; i < OutputStreamCount; i++)
            {
                OutputStreamArray[i].Invoke(pValue);
            }
        }

        System.Action<InputStreamType> IStream.GetInputStream<InputStreamType>()
        {
            if (InputStream is System.Action<InputStreamType> inputStreamReduse)
            {
                return inputStreamReduse;
            }
            else Exception(Ex.AN.x100001);

            return default;
        }

        public virtual System.Action<InputStreamType> GetInputStream<InputStreamType>
            (System.Action<string> pParentConsole, System.Func<string, bool> pParentException)
        {
            ParentConsole = pParentConsole;
            ParentException = pParentException;

            return ((IStream)this).GetInputStream<InputStreamType>();
        }
        void IStream.AddOutputStream<OutputStreamValue>(System.Action<OutputStreamValue> pOutputStream)
        {
            if (pOutputStream is System.Action<OutputBufferType> outputStreamReduse)
            {
                OutputStreamArray[OutputStreamCount++] = outputStreamReduse;
            }
            else Exception(Ex.STRM.x100000);
        }
        void IStream.AddOutputStream<OutputStreamValue>(System.Action<OutputStreamValue> pOutputStream,
            System.Action<string> pParentConsole, System.Func<string, bool> pParentException)
        {
            ParentConsole = pParentConsole;
            ParentException = pParentException;

            if (pOutputStream is System.Action<OutputBufferType> outputStreamReduse)
            {
                OutputStreamArray[OutputStreamCount++] = outputStreamReduse;
            }
            else Exception(Ex.STRM.x100000);
        }
        void IStream.SetStream(IStream pStream)
        {
            if (pStream == null)
            {
                Exception(Ex.STRM.x100000);
            }
            else
            {
                if (pStream.GetInputStream<OutputBufferType>(ParentConsole, ParentException) is System.Action<OutputBufferType> outputStreamReduse)
                {
                    OutputStreamArray[OutputStreamCount++] = outputStreamReduse;
                }
                else Exception(Ex.STRM.x100000);
            } 
        }

        void IStream.SetStreamToParent(IDOM pStream)
        {
            OutputStreamArray[OutputStreamCount++] = pStream.SendToParent;
        }

        protected  bool Exception(string pMessage)
        {
            return ParentException(GetNameObject() + " : " + pMessage);
        }
            
        protected  void Console(string pMessage)
        {
            //ParentConsole(GetNameObject() + " : " + pMessage);
            System.Console.WriteLine(GetNameObject() + " : " + pMessage);
        }

        public void Dispose()
        {
            ParentConsole = null;
            ParentException = null;

            InputStream = null;

            for (int i = 0; i < OutputStreamCount; i++) OutputStreamArray[i] = null;
        }
    }
}
