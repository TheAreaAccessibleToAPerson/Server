namespace Butterfly.system
{
    public class FuncObject<InputValueType, OutputValueType> : IStream
        where InputValueType : struct where OutputValueType : struct
    {
        private System.Func<InputValueType, OutputValueType> Func;

        private System.Func<string, bool> ExceptionParent;
        private string PathException;
        
        public FuncObject(System.Func<InputValueType, OutputValueType> pFunc, string pException, System.Func<string, bool> pExceptionParent)
        {
            Func = pFunc;

            PathException = pException;
            ExceptionParent = pExceptionParent;
            InputStream = Input;
        }

        private System.Action<InputValueType> InputStream;

        private readonly System.Action<OutputValueType>[] OutputStreamArray = new System.Action<OutputValueType>[32];
        private int OutputStreamCount = 0;

        public void Input(InputValueType pValue)
        {
            OutputValueType result = Func.Invoke(pValue);

            for (int i = 0; i < OutputStreamCount; i++)
            {
                OutputStreamArray[i].Invoke(result);
            }
        }

        System.Action<InputStreamType> IStream.GetInputStream<InputStreamType>()
        {
            if (InputStream is System.Action<InputStreamType> inputStreamReduse)
            {
                return inputStreamReduse;
            }
            else Exception(Ex.STRM.x100001);

            return default;
        }
        void IStream.SetStream(IStream pStream)
        {
            if (pStream.GetInputStream<OutputValueType>() is System.Action<OutputValueType> outputStreamReduse)
            {
                OutputStreamArray[OutputStreamCount++] = outputStreamReduse;
            }
            else Exception(Ex.STRM.x100000);
        }
        void IStream.AddOutputStream<OutputStreamValue>(System.Action<OutputStreamValue> pOutputStream)
        {
            if (pOutputStream is System.Action<OutputValueType> outputStreamReduse)
            {
                OutputStreamArray[OutputStreamCount++] = outputStreamReduse;
            }
            else Exception(Ex.STRM.x100000);
        }
        void IStream.SetStreamToParent(IDOM pStream)
        {
            OutputStreamArray[OutputStreamCount++] = pStream.SendToParent;
        }

        private bool Exception(string pMessage)
        {
            return ExceptionParent.Invoke("[FuncObject]" + Func.Method.Name + ":" + pMessage);
        }

        public string GetTypeObject()
        {
            return typeof(FuncObject<InputValueType, OutputValueType>).ToString();
        }
        public string GetNameObject()
        {
            return Func.Method.Name;
        }
        public string GetExplorerObject()
        {
            return PathException;
        }
        public string GetNamespaceObject()
        {
            return GetType().ToString();
        }

        public void Dispose()
        {
            Func = null;
            ExceptionParent = null;
            PathException = null;
            InputStream = null;

            for (int i = 0; i < OutputStreamCount; i++) OutputStreamArray[i] = null;
        }

        public bool IsTypeObject(string pType)
        {
            throw new System.NotImplementedException();
        }
    }
}
