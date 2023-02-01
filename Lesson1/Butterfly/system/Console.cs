namespace Butterfly.system
{
    public static class Console
    {
        private static object Locker = new object();

        public static void WriteLine(string pMessage, System.ConsoleColor pConsoleColor = System.ConsoleColor.Green)
        {
            lock(Locker)
            {
                System.Console.ForegroundColor = pConsoleColor;
                System.Console.WriteLine(pMessage);
            }
        }

        public static void Write(string pMessage, System.ConsoleColor pConsoleColor = System.ConsoleColor.Green)
        {
            lock (Locker)
            {
                System.Console.ForegroundColor = pConsoleColor;
                System.Console.Write(pMessage);
            }
        }
    }
}
