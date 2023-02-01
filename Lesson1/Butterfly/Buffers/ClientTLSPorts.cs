namespace Butterfly.Buffer
{
    public struct ClientTLSPorts : IBuffer
    {
        public int Index; // Индекс TLSPorts в который нужно записать номера портов которые использует клиент на своем устройве.

        public int Port_1;
        public int Port_2;

        public string GetName()
        {
            return typeof(ClientTLSPorts).ToString();
        }
    }
}
