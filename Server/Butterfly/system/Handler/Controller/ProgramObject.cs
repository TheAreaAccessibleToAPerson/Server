namespace Butterfly.system
{
    public abstract class ProgramObject : ThreadObject
    {
        protected sealed override void Construction() { }

        public ProgramObject() : base(TypeObjectData.PROGRAM_CONTROLLER) { }

        public static bool IsStartProgram = false;

        private readonly SafeList<System.Action> NodesToDestroy = new SafeList<System.Action>();

        private void NodeDestroyProcess()
        {
            if (NodesToDestroy.ExtractAll(out System.Action[] oActions))
            {
                foreach(System.Action action in oActions)
                {
                    action.Invoke();
                }
            }
        }

        public void AddNodeToDestroy(System.Action pAction)
        {
            NodesToDestroy.Add(pAction);
        }

        public void StartProgram()
        {
            if (!IsStartProgram)
            {
                int i = System.Console.WindowWidth; // 56
                int u = i - 56;
                int r = u / 2 - 1;
                string g = "";
                string f = "";
                for (int h = 0; h < r; h++)
                {
                    g += "█";
                    f += " ";
                }

                system.Console.WriteLine(g + "████████████████████████████████████████████████████████" + g + "\n" +
                                         g + "████████████████████████████████████████████████████████" + g + "\n" +
                                         g + "█▄─▄─▀█▄─██─▄█─▄─▄─█─▄─▄─█▄─▄▄─█▄─▄▄▀█▄─▄▄─█▄─▄███▄─█─▄█" + g + "\n" +
                                         g + "██─▄─▀██─██─████─█████─████─▄█▀██─▄─▄██─▄████─██▀██▄─▄██" + g + "\n" +
                                         f + "▀▄▄▄▄▀▀▀▄▄▄▄▀▀▀▄▄▄▀▀▀▄▄▄▀▀▄▄▄▄▄▀▄▄▀▄▄▀▄▄▄▀▀▀▄▄▄▄▄▀▀▄▄▄▀▀", 
                                         System.ConsoleColor.Green);

                IsStartProgram = true;

                ((IDOM)this).SetDOM(this, null, GetNameObject(), 0);
                ((IDOM)this).CreatingNode();

                ((IThreadSystem)this).AddThread(NodeDestroyProcess, "NodeDestroyProcess", 100);

                UpdateThread();
            }
            else
                Exception(Ex.PRG.x1000000);
        }

        private void UpdateThread()
        {
            while (true)
            {
                if (__IsDestroy) return;

                System.Threading.Thread.Sleep(Global.PROGRAM_LIFE_TIME_DELAY);
            }
        }
    }
}


