namespace Butterfly.system
{
    public interface IHellper : System.IDisposable
    {
        void SetHeader(system.IScopeHellper pUserHellper, System.Action<string> pParentConsole, System.Func<string, bool> pParentException);
    }
}
