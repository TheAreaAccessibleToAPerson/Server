namespace Butterfly
{
    public interface IStateObject : system.IHeader
    {
        void Construction();
        void Dependency();

        void Start();
        void Stop();
        void Pause();
        void Resume();
        void Reboot();
    }
}
