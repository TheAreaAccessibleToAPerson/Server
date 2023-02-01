namespace Butterfly.system
{
    public class ThreadBuffer : System.IDisposable
    {
        public string Name;
        public System.Threading.Tasks.Task Thread;
        private System.Action BreakTheBlockAction;

        public bool Stop = false;

        public ThreadBuffer(string pName, System.Threading.Tasks.Task pThread, System.Action pBreakTheBlockAction)
        {
            Name = pName;
            Thread = pThread;
            BreakTheBlockAction = pBreakTheBlockAction;
        }

        public void BreakTheBlock()
        {
            if (BreakTheBlockAction != null)
            {
                BreakTheBlockAction.Invoke();
            }
        }

        public void Dispose()
        {
            Thread = null;
            BreakTheBlockAction = null;
        }
    }

}

