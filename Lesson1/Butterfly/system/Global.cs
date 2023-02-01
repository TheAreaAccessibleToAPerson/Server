namespace Butterfly
{
    public static class Global
    {
        public static void StartProgram<ProgramObjectType>() where ProgramObjectType : new()
        {
            if (new ProgramObjectType() is system.ProgramObject programReduse)
            {
                programReduse.StartProgram();
            }
        }

        public const string NAMESPACE = "Butterfly";

        public const bool TEST = true;

        public const int PROGRAM_LIFE_TIME_DELAY = 250;
    }
}
