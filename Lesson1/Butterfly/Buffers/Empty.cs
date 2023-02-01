namespace Butterfly.Buffer
{
    public struct Empty : IBuffer
    {
        public string GetName()
        {
            return typeof(Empty).Name;
        }
    }
}
