namespace Butterfly
{
    public abstract class HellperHeader : system.IHellper
    {
        private System.Action<string> ParentConsole;
        private System.Func<string, bool> ParentException;

        protected system.IScopeHellper User;

        void system.IHellper.SetHeader(system.IScopeHellper pUserHellper, System.Action<string> pParentConsole, System.Func<string, bool> pParentException)
        {
            User = pUserHellper;

            ParentConsole = pParentConsole;
            ParentException = pParentException;
        }

        protected bool Exception(string pMessage)
        {
            if (ParentException != null)
            {
                return ParentException.Invoke($"{GetType()}|:{pMessage}");
            }
            else
            {
                system.Console.WriteLine($"Вы не можете вызвать Exception() при создании Hellper'а: {GetType()}:{pMessage}", System.ConsoleColor.Red);

                return false;
            }
        }
        protected void Console(string pMessage)
        {
            if (ParentConsole != null)
            {
                ParentConsole.Invoke($"{GetType()}:{pMessage}");
            }
            else
            {
                system.Console.WriteLine($"[Console]Creating: {GetType()}:{pMessage}", System.ConsoleColor.Red);
            }
        }
        public virtual void Dispose()
        {
            User = null;

            ParentConsole = null;
            ParentException = null;
        }
    }   
}
