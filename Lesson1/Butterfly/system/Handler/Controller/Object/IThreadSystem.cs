namespace Butterfly.system
{
    public interface IThreadSystem
    {
        void AddThread(System.Action action, string pName, int pThreadTimeDelay, int pCountThread = 1);
    }
}

