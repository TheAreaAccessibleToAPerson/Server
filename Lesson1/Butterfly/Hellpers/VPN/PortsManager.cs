using System.Net;

namespace Butterfly.Hellper.VPN
{
    public struct ClientPorts
    {
        public struct Port
        {
            public string ConnectionKey;// Имя подключение(обьекта) которое использует данный порт.

            public readonly int Index; // Индекс в массиве CleintPorts.PortArray[]
            public bool Free; // Занят ли уже порт в ClientPorts.Port[Index]

            public readonly int ServerLocalPort; // Номер Port'a, который будет использоваться на сервере для создания подключения.
            public int ClientNumberPort; // Номер порта который использовал клиент на своем устройве.

            public long SequenceNumber;
            public long AcknowledgmentNumber;

            public System.DateTime DateTime;

            public Port(int pIndex, int pServerNumberPort)
            {
                ConnectionKey = "";
                Index = pIndex;
                Free = true;
                ServerLocalPort = pServerNumberPort;
                ClientNumberPort = -1;
                SequenceNumber = 0;
                AcknowledgmentNumber = 0;
                DateTime = System.DateTime.Now;
            }
        }

        /// <summary>
        /// Номер портов которые используются на устройве, от которого мы получаем пакеты.
        /// </summary>
        public struct ClientTLSPorts 
        {
            public int Index; // Индекс TLSPorts в который нужно записать номера портов которые использует клиент на своем устройве.

            public int NumberPort;

            public int Port;
        }

        /// <summary>
        /// Номер порта которые используются на устройве, от которого мы получаем пакеты.
        /// </summary>
        public struct ClientPort
        {
            public int Index;

            public int Port;
        }

        public struct TLSPorts
        {
            public string ConnectionKey; // Имя подключение(обьекта) которое использует данный порт.

            public readonly int Index; // Индекс в массиве ClientPorts.TLSPortsArray[]
            public bool Free; // Заняты ли уже TLS порты в ClientPorts.TLSPortsArray[].

            public readonly int ServerLocalPort_1; // Номер первого TLS Port'a, который будет использоваться на сервере для создания подключения.
            public readonly int ServerLocalPort_2; // Номер второго TLS Port'a, который будет использоваться на сервере для создания подключения.
            public readonly int ServerLocalPort_3;

            public int ClientNumberPort_1; // Номер первого порта который использовал клиент на своем устройве.
            public int ClientNumberPort_2; // Номер первого порта который использовал клиент на своем устройве.
            public int ClientNumberPort_3;

            public System.DateTime DateTime;

            public long SequenceNumber1;
            public long AcknowledgmentNumber1;

            public long SequenceNumber2;
            public long AcknowledgmentNumber2;

            public long SequenceNumber3;
            public long AcknowledgmentNumber3;

            public TLSPorts(int pIndex, int pServerNumberPort_1, int pServerNumberPort_2, int pServerNumberPort_3)
            {
                ConnectionKey = "";
                Index = pIndex;
                Free = true;
                ServerLocalPort_1 = pServerNumberPort_1;
                ServerLocalPort_2 = pServerNumberPort_2;
                ServerLocalPort_3 = pServerNumberPort_3;
                ClientNumberPort_1 = -1;
                ClientNumberPort_2 = -1;
                ClientNumberPort_3 = -1;
                DateTime = System.DateTime.Now;

                SequenceNumber1 = 0;
                AcknowledgmentNumber1 = 0;

                SequenceNumber2 = 0;
                AcknowledgmentNumber2 = 0;

                SequenceNumber3 = 0;
                AcknowledgmentNumber3 = 0;
            }
        }

        public string Name; // Имя клиента который получит данную струкруту.
        public string Address;// Аресс клиента.
        public byte[] AddressBytes;

        public readonly int Index; // Позиция в массиве в классе PortManager.
        public bool Free; // Свободна ли струкрура для использования клиентом.

        // ДИОПОЗОН ВСЕХ ПОРТОВ.
        public readonly int BeginningOfTheRange; // Начало диапозона номеров всех портов.
        public readonly int EndOfRange;  // Конец диопозона номеров всех потов.


        public readonly TLSPorts[] TLSPortsArray; // Массив TLS потов.
        public ulong TLSPortsOrederOfUse; // Используется для выставление в поле TLSPorts.OrderOfUse.
        public int CurrentIndexTLSPorts; // Сдесь хранится номер последнего полученого TLSPorts.
        public readonly int MaximumNumberOfConnectionTLSPorts; // Достигнув середины от общего количесва доступных портов, начнем отключать
                                                      // первые подключения.
        public readonly Port[] PortArray; // Массив портов.
        public ulong PortOrederOfUse; // Используется для выставление в поле Port.OrderOfUse.
        public int CurrentIndexPort; // Сдесь хранится номер последнего полученого Port.
        public readonly int MaximumNumberOfConnectionPort; // Достигнув середины от общего количесва доступных портов, начнем отключать
                                                           // первые подключения.

        public System.DateTime DateTime;

        public ClientPorts(int pIndex, int pBegginningOfTheRange, int pEndOfRange, TLSPorts[] pTLSPortsArray, Port[] pPortArray)
        {
            Name = "";
            Address = "";
            AddressBytes = new byte[4];

            Index = pIndex;
            Free = true;

            BeginningOfTheRange = pBegginningOfTheRange;
            EndOfRange = pEndOfRange;


            TLSPortsArray = pTLSPortsArray;
            TLSPortsOrederOfUse = 0;
            CurrentIndexTLSPorts = 0;
            MaximumNumberOfConnectionTLSPorts = pTLSPortsArray.Length / 2;

            PortArray = pPortArray;
            PortOrederOfUse = 0;
            CurrentIndexPort = 0;
            MaximumNumberOfConnectionPort = pPortArray.Length / 2;

            DateTime = System.DateTime.Now;
        }
    }

    class PortsManager
    {
        private ClientPorts[] ClientsPorts;

        private const int FIRST_PORT = 40000;

        private readonly object Locker = new object();

        public PortsManager Creating(int pClientPortCount, int pPortCount, int pTLSPortsCount)
        {
            ClientsPorts = new ClientPorts[pClientPortCount];

            int indexPort = FIRST_PORT;

            for (int i = 0; i < pClientPortCount; i++)
            {
                int beginningOfTheRange = indexPort;

                ClientPorts.TLSPorts[] tlsPortsArray = new ClientPorts.TLSPorts[pTLSPortsCount];
                for (int u = 0; u < tlsPortsArray.Length; u++)
                {
                    tlsPortsArray[u] = new ClientPorts.TLSPorts(u, indexPort++, indexPort++, indexPort++)
                    {
                        Free = true
                    };
                }

                ClientPorts.Port[] portArray = new ClientPorts.Port[pPortCount];
                for (int u = 0; u < portArray.Length; u++)
                {
                    portArray[u] = new ClientPorts.Port(u, indexPort++)
                    {
                        Free = true
                    };
                }

                int endOfRange = indexPort - 1;

                ClientsPorts[i] = new ClientPorts(i, beginningOfTheRange, endOfRange, tlsPortsArray, portArray)
                {
                    Free = true,

                    TLSPortsOrederOfUse = 0,
                    CurrentIndexTLSPorts = 0,

                    PortOrederOfUse = 0,
                    CurrentIndexPort = 0,
                };
            }

            return this;
        }

        public bool TryGetClientPorts(string pClientKey, IPAddress pClientAddress, out ClientPorts oClientPorts)
        {
            lock(Locker)
            {
                oClientPorts = default;

                bool result = false;
                {
                    for (int i = 0; i < ClientsPorts.Length; i++)
                    {
                        if (ClientsPorts[i].Free)
                        {
                            result = true;

                            ClientsPorts[i].Free = false;
                            ClientsPorts[i].Name = pClientKey;
                            ClientsPorts[i].Address = pClientAddress.ToString();
                            ClientsPorts[i].AddressBytes = pClientAddress.GetAddressBytes();

                            oClientPorts = ClientsPorts[i];

                            break;
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Получить пакет для отправки клиенту.
        /// </summary>
        /// <param name="pPort"></param>
        /// <returns></returns>
        public bool TryGetPacketToSendClient(int pPort, out string oClientKey)
        {
            oClientKey = default;

            lock(Locker)
            {
                bool result = true;
                {
                    for (int i = 0; i < ClientsPorts.Length; i++)
                    {
                        if (!ClientsPorts[i].Free)
                        {
                            if (ClientsPorts[i].BeginningOfTheRange <= pPort && pPort <= ClientsPorts[i].EndOfRange)
                            {
                                oClientKey = ClientsPorts[i].Name;

                                return result;
                            }
                        }
                    }
                }
                return result = false;
            }
        }

        public void FreeClientPorts(ClientPorts pClientPorts)
        {
            lock(Locker)
            {
                ClientsPorts[pClientPorts.Index].Free = true;
                ClientsPorts[pClientPorts.Index].Address = "";
                ClientsPorts[pClientPorts.Index].AddressBytes = new byte[4];
                ClientsPorts[pClientPorts.Index].Name = "";

                for (int r = 0; r < ClientsPorts[pClientPorts.Index].TLSPortsArray.Length; r++)
                {
                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].ConnectionKey = "";
                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].ClientNumberPort_1 = -1;
                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].ClientNumberPort_2 = -1;
                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].ClientNumberPort_3 = -1;

                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].SequenceNumber1 = 0;
                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].SequenceNumber2 = 0;
                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].SequenceNumber3 = 0;

                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].AcknowledgmentNumber1 = 0;
                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].AcknowledgmentNumber2 = 0;
                    ClientsPorts[pClientPorts.Index].TLSPortsArray[r].AcknowledgmentNumber3 = 0;
                }

                for (int r = 0; r < ClientsPorts[pClientPorts.Index].PortArray.Length; r++)
                {
                    ClientsPorts[pClientPorts.Index].PortArray[r].ConnectionKey = "";
                    ClientsPorts[pClientPorts.Index].PortArray[r].ClientNumberPort = -1;
                    ClientsPorts[pClientPorts.Index].PortArray[r].SequenceNumber = 0;
                    ClientsPorts[pClientPorts.Index].PortArray[r].AcknowledgmentNumber = 0;
                }
            }
        }
    }
}
