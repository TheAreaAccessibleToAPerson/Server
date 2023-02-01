namespace Butterfly.Hellper.VPN
{
    class PortController
    {
        private ConvertingFields ConvertingField = new ConvertingFields();

        private ClientPorts ClientPorts;

        private readonly object Locker = new object();

        public void Creating(ClientPorts pClientPorts)
        {
            ClientPorts = pClientPorts;
        }

        public bool TryGetPort(string pClientKey, out ClientPorts.Port oPort, out string oKeyObjectRemove)
        {
            oPort = default;
            oKeyObjectRemove = "";

            lock (Locker)
            {
                bool result = false;
                {
                    System.DateTime currentDateTime = System.DateTime.Now;

                    // Получим число задейсвованых портов.
                    int countNotFreePort = 0;
                    foreach (var port in ClientPorts.PortArray)
                        if (!port.Free) 
                            countNotFreePort++;

                    // Если число задейсвованых портов превышает допустимое значение, то отключим самое старое.
                    if (countNotFreePort > ClientPorts.MaximumNumberOfConnectionPort)
                    {
                        result = true; // Укажим что нужно произвести отключение по ключю.

                        // Получаем индекс самого старого подключения.
                        int oldPortsConnectionIndex = -1;
                        System.DateTime oldConnectionDateTime = currentDateTime;
                        foreach (var port in ClientPorts.PortArray)
                        {
                            if (!port.Free)
                            {
                                if (port.DateTime < oldConnectionDateTime)
                                {
                                    oldConnectionDateTime = port.DateTime;
                                    oldPortsConnectionIndex = port.Index;
                                }
                            }
                        }

                        // Запишим ключ соединения которое нужно рассоединить.
                        oKeyObjectRemove = ClientPorts.PortArray[oldPortsConnectionIndex].ConnectionKey;
                        // Освободим TLSPorts.
                        FreePort(ClientPorts.PortArray[oldPortsConnectionIndex]);
                    }

                    // Получим свободный TLSPorts, который был отлючон раньше всех.
                    int indexPort = -1;
                    System.DateTime portDateTime = currentDateTime;
                    for (int i = 0; i < ClientPorts.PortArray.Length; i++)
                    {
                        var port = ClientPorts.PortArray[i];

                        if (port.Free)
                        {
                            if (port.DateTime < portDateTime)
                            {
                                portDateTime = port.DateTime;
                                indexPort = port.Index;
                            }
                        }
                    }

                    ClientPorts.PortArray[indexPort].Free = false; // Установим что эта структура занята.
                    ClientPorts.PortArray[indexPort].DateTime = currentDateTime; // Запишим время подлкючения.
                    ClientPorts.PortArray[indexPort].ConnectionKey = pClientKey;

                    oPort = ClientPorts.PortArray[indexPort];
                }
                return result;
            }
        }

        /// <summary>
        /// Если вернется true, то нужно уничтожить обьект по ключу.
        /// </summary>
        /// <param name="oTLSPorts"></param>
        /// <param name="oKeyObjectRemove"></param>
        /// <returns></returns>
        public bool TryGetTLSPorts(string pClientKey, out ClientPorts.TLSPorts oTLSPorts, out string oKeyObjectRemove)
        {
            lock(Locker)
            {
                oTLSPorts = default;
                oKeyObjectRemove = "";

                bool result = false;
                {
                    System.DateTime currentDateTime = System.DateTime.Now;

                    // Получим число задейсвованых TLSPorts
                    int countNotFreeTlsPorts = 0;
                    foreach (var tlsPorts in ClientPorts.TLSPortsArray)
                        if (!tlsPorts.Free)
                            countNotFreeTlsPorts++;

                    // Если число задейсвованых TLSPorts превышает допустимое значение, то отключим самое старое.
                    if (countNotFreeTlsPorts > ClientPorts.MaximumNumberOfConnectionTLSPorts)
                    {
                        result = true; // Укажим что нужно произвести отключение по ключю.

                        // Получаем индекс самого старого подключения.
                        int oldTlsPortsConnectionIndex = -1;
                        System.DateTime oldConnectionDateTime = currentDateTime;
                        foreach (var tlsPorts in ClientPorts.TLSPortsArray)
                        {
                            if (!tlsPorts.Free)
                            {
                                if (tlsPorts.DateTime < oldConnectionDateTime)
                                {
                                    oldConnectionDateTime = tlsPorts.DateTime;
                                    oldTlsPortsConnectionIndex = tlsPorts.Index;
                                }
                            }
                        }

                        // Запишим ключ соединения которое нужно рассоединить.
                        oKeyObjectRemove = ClientPorts.TLSPortsArray[oldTlsPortsConnectionIndex].ConnectionKey;
                        // Освободим TLSPorts.
                        FreeTLSPorts(ClientPorts.TLSPortsArray[oldTlsPortsConnectionIndex]);
                    }

                    // Получим свободный TLSPorts, который был отлючон раньше всех.
                    int indexTlsPorts = -1;
                    System.DateTime tlsPortsDateTime = currentDateTime;
                    for (int i = 0; i < ClientPorts.TLSPortsArray.Length; i++)
                    {
                        var tlsPorts = ClientPorts.TLSPortsArray[i];

                        if (tlsPorts.Free)
                        {
                            if (tlsPorts.DateTime < tlsPortsDateTime)
                            {
                                tlsPortsDateTime = tlsPorts.DateTime;
                                indexTlsPorts = tlsPorts.Index;
                            }
                        }
                    }

                    ClientPorts.TLSPortsArray[indexTlsPorts].Free = false; // Установим что эта структура занята.
                    ClientPorts.TLSPortsArray[indexTlsPorts].DateTime = currentDateTime; // Запишим время подлкючения.
                    ClientPorts.TLSPortsArray[indexTlsPorts].ConnectionKey = pClientKey;

                    oTLSPorts = ClientPorts.TLSPortsArray[indexTlsPorts];
                }
                return result;
            }
        }

        public void SetClientTLSPorts(Hellper.VPN.ClientPorts.ClientTLSPorts pClientTLSPortsBuffer)
        {
            lock(Locker)
            {
                if (pClientTLSPortsBuffer.NumberPort == 0)
                {
                    ClientPorts.TLSPortsArray[pClientTLSPortsBuffer.Index].ClientNumberPort_1 = pClientTLSPortsBuffer.Port;
                }
                else if (pClientTLSPortsBuffer.NumberPort == 1)
                {
                    ClientPorts.TLSPortsArray[pClientTLSPortsBuffer.Index].ClientNumberPort_2 = pClientTLSPortsBuffer.Port;
                }
                else if (pClientTLSPortsBuffer.NumberPort == 2)
                {
                    ClientPorts.TLSPortsArray[pClientTLSPortsBuffer.Index].ClientNumberPort_3 = pClientTLSPortsBuffer.Port;
                }
            }
        }

        public void SetClientPort(Hellper.VPN.ClientPorts.ClientPort pClientPortBuffer)
        {
            lock (Locker)
            {
                ClientPorts.PortArray[pClientPortBuffer.Index].ClientNumberPort = pClientPortBuffer.Port;
            }
        }

        public void SetSeqAckNumberTLSConnection(SeqAckNumber pSynAckNumber)
        {
            lock(Locker)
            {
                if (pSynAckNumber.SocketNumber == 0)
                {
                    ClientPorts.TLSPortsArray[pSynAckNumber.IndexInArray].SequenceNumber1 = pSynAckNumber.Sequence;
                    ClientPorts.TLSPortsArray[pSynAckNumber.IndexInArray].AcknowledgmentNumber1 = pSynAckNumber.Acknowledgment;
                }
                else if (pSynAckNumber.SocketNumber == 1)
                {
                    ClientPorts.TLSPortsArray[pSynAckNumber.IndexInArray].SequenceNumber2 = pSynAckNumber.Sequence;
                    ClientPorts.TLSPortsArray[pSynAckNumber.IndexInArray].AcknowledgmentNumber2 = pSynAckNumber.Acknowledgment;
                }
                else if (pSynAckNumber.SocketNumber == 2)
                {
                    ClientPorts.TLSPortsArray[pSynAckNumber.IndexInArray].SequenceNumber3 = pSynAckNumber.Sequence;
                    ClientPorts.TLSPortsArray[pSynAckNumber.IndexInArray].AcknowledgmentNumber3 = pSynAckNumber.Acknowledgment;
                }
            }
        }
        public void SetSeqAckNumberHTTPConnection(SeqAckNumber pSynAckNumber)
        {
            lock (Locker)
            {
                ClientPorts.PortArray[pSynAckNumber.IndexInArray].SequenceNumber = pSynAckNumber.Sequence;
                ClientPorts.PortArray[pSynAckNumber.IndexInArray].AcknowledgmentNumber = pSynAckNumber.Acknowledgment;
            }
        }

        public Buffer.IndexedByte SubstitutionOfFields(Buffer.NetPackets pNetworkPackets)
        {
            lock(Locker)
            {
                int clientPort = default; long ackNumber = default; long seqNumber = default;
                // 1 - TLSPorts[1Port], 2 - TLSPorts[2Port], 3 - TLSPorts[3Port], 4 - [Port]
                int typePort = -1; int index = -1;
                {
                    if (pNetworkPackets.SourcePort == 443)
                    {
                        foreach (var tlsPorts in ClientPorts.TLSPortsArray)
                            if (!tlsPorts.Free)
                                if (tlsPorts.ServerLocalPort_1 == pNetworkPackets.DestinationPort)
                                {
                                    typePort = 1; index = tlsPorts.Index;

                                    clientPort = tlsPorts.ClientNumberPort_1;
                                    seqNumber = tlsPorts.SequenceNumber1;
                                    ackNumber = tlsPorts.AcknowledgmentNumber1;
                                    break;
                                }
                                else if (tlsPorts.ServerLocalPort_2 == pNetworkPackets.DestinationPort)
                                {
                                    typePort = 2; index = tlsPorts.Index;

                                    clientPort = tlsPorts.ClientNumberPort_2;
                                    seqNumber = tlsPorts.SequenceNumber2;
                                    ackNumber = tlsPorts.AcknowledgmentNumber2;
                                    break;
                                }
                                else if (tlsPorts.ServerLocalPort_3 == pNetworkPackets.DestinationPort)
                                {
                                    typePort = 3; index = tlsPorts.Index;

                                    clientPort = tlsPorts.ClientNumberPort_3;
                                    seqNumber = tlsPorts.SequenceNumber3;
                                    ackNumber = tlsPorts.AcknowledgmentNumber3;
                                    break;
                                }
                    }
                    else if (pNetworkPackets.SourcePort == 80)
                    {
                        foreach (var port in ClientPorts.PortArray)
                            if (!port.Free)
                                if (port.ServerLocalPort == pNetworkPackets.DestinationPort)
                                {
                                    typePort = 4; index = port.Index;

                                    clientPort = port.ClientNumberPort;
                                    seqNumber = port.SequenceNumber;
                                    ackNumber = port.AcknowledgmentNumber;
                                    break;
                                }
                    }
                }

                int startIPPacket = pNetworkPackets.StartIndexIPPacket;
                int startTCPPacket = pNetworkPackets.StartIndexTCPPacket;

                if (pNetworkPackets.IsSynAck())
                {
                    byte[] ackNumberByte = ConvertingField.SplittingAn4Integer(seqNumber + 1);

                    pNetworkPackets.Data[startTCPPacket + 8] = ackNumberByte[0];
                    pNetworkPackets.Data[startTCPPacket + 9] = ackNumberByte[1];
                    pNetworkPackets.Data[startTCPPacket + 10] = ackNumberByte[2];
                    pNetworkPackets.Data[startTCPPacket + 11] = ackNumberByte[3];

                    {
                        long acknowledgment = ConvertingField.ToAnIntegerValue(
                        new byte[] { ackNumberByte[0], ackNumberByte[1], ackNumberByte[2], ackNumberByte[3] });

                        pNetworkPackets.AcknowledgmentNumber = acknowledgment;
                    }
                }
                else if (pNetworkPackets.IsAck())
                {
                    if (pNetworkPackets.SegmentLength == 0 && seqNumber == -1)
                    {
                        byte[] seqNumberByte = ConvertingField.SplittingAn4Integer(ackNumber);

                        pNetworkPackets.Data[startTCPPacket + 4] = seqNumberByte[0];
                        pNetworkPackets.Data[startTCPPacket + 5] = seqNumberByte[1];
                        pNetworkPackets.Data[startTCPPacket + 6] = seqNumberByte[2];
                        pNetworkPackets.Data[startTCPPacket + 7] = seqNumberByte[3];

                        if (typePort == 1)
                            ClientPorts.TLSPortsArray[index].AcknowledgmentNumber1 = pNetworkPackets.AcknowledgmentNumber;
                        else if (typePort == 2)
                            ClientPorts.TLSPortsArray[index].AcknowledgmentNumber2 = pNetworkPackets.AcknowledgmentNumber;
                        else if (typePort == 3)
                            ClientPorts.TLSPortsArray[index].AcknowledgmentNumber3 = pNetworkPackets.AcknowledgmentNumber;
                        else if (typePort == 4)
                            ClientPorts.PortArray[index].AcknowledgmentNumber = pNetworkPackets.AcknowledgmentNumber;

                        if (typePort == 1)
                            ClientPorts.TLSPortsArray[index].SequenceNumber1 = ackNumber;
                        else if (typePort == 2)
                            ClientPorts.TLSPortsArray[index].SequenceNumber2 = ackNumber;
                        else if (typePort == 3)
                            ClientPorts.TLSPortsArray[index].SequenceNumber3 = ackNumber;
                        else if (typePort == 4)
                            ClientPorts.PortArray[index].SequenceNumber = ackNumber;

                        {
                            long sequence = ConvertingField.ToAnIntegerValue(
                                new byte[] { seqNumberByte[0], seqNumberByte[1], seqNumberByte[2], seqNumberByte[3] });

                            pNetworkPackets.SequenceNumber = sequence;
                        }
                    }
                    else if (pNetworkPackets.SegmentLength > 0 && seqNumber == -1)
                    {
                        byte[] seqNumberByte = ConvertingField.SplittingAn4Integer(ackNumber);

                        pNetworkPackets.Data[startTCPPacket + 4] = seqNumberByte[0];
                        pNetworkPackets.Data[startTCPPacket + 5] = seqNumberByte[1];
                        pNetworkPackets.Data[startTCPPacket + 6] = seqNumberByte[2];
                        pNetworkPackets.Data[startTCPPacket + 7] = seqNumberByte[3];

                        if (typePort == 1)
                            ClientPorts.TLSPortsArray[index].AcknowledgmentNumber1 = pNetworkPackets.AcknowledgmentNumber;
                        else if (typePort == 2)
                            ClientPorts.TLSPortsArray[index].AcknowledgmentNumber2 = pNetworkPackets.AcknowledgmentNumber;
                        else if (typePort == 3)
                            ClientPorts.TLSPortsArray[index].AcknowledgmentNumber3 = pNetworkPackets.AcknowledgmentNumber;
                        else if (typePort == 4)
                            ClientPorts.PortArray[index].AcknowledgmentNumber = pNetworkPackets.AcknowledgmentNumber;

                        if (typePort == 1)
                            ClientPorts.TLSPortsArray[index].SequenceNumber1 = ackNumber;
                        else if (typePort == 2)
                            ClientPorts.TLSPortsArray[index].SequenceNumber2 = ackNumber;
                        else if (typePort == 3)
                            ClientPorts.TLSPortsArray[index].SequenceNumber3 = ackNumber;
                        else if (typePort == 4)
                            ClientPorts.PortArray[index].SequenceNumber = ackNumber;

                        {
                            long sequence = ConvertingField.ToAnIntegerValue(
                                new byte[] { seqNumberByte[0], seqNumberByte[1], seqNumberByte[2], seqNumberByte[3] });

                            pNetworkPackets.SequenceNumber = sequence;
                        }
                    }
                    else if (pNetworkPackets.SegmentLength > 0)
                    {
                        byte[] ackNumberByte = ConvertingField.SplittingAn4Integer(ackNumber);

                        pNetworkPackets.Data[startTCPPacket + 8] = ackNumberByte[0];
                        pNetworkPackets.Data[startTCPPacket + 9] = ackNumberByte[1];
                        pNetworkPackets.Data[startTCPPacket + 10] = ackNumberByte[2];
                        pNetworkPackets.Data[startTCPPacket + 11] = ackNumberByte[3];

                        byte[] seqNumberByte = ConvertingField.SplittingAn4Integer(seqNumber);

                        pNetworkPackets.Data[startTCPPacket + 4] = seqNumberByte[0];
                        pNetworkPackets.Data[startTCPPacket + 5] = seqNumberByte[1];
                        pNetworkPackets.Data[startTCPPacket + 6] = seqNumberByte[2];
                        pNetworkPackets.Data[startTCPPacket + 7] = seqNumberByte[3];

                        {
                            long sequence = ConvertingField.ToAnIntegerValue(
                                new byte[] { seqNumberByte[0], seqNumberByte[1], seqNumberByte[2], seqNumberByte[3] });

                            pNetworkPackets.SequenceNumber = sequence;
                        }

                        {
                            long acknowledgment = ConvertingField.ToAnIntegerValue(
                                new byte[] { ackNumberByte[0], ackNumberByte[1], ackNumberByte[2], ackNumberByte[3] });

                            pNetworkPackets.AcknowledgmentNumber = acknowledgment;
                        }
                    }
                    else
                    {
                        byte[] ackNumberByte = ConvertingField.SplittingAn4Integer(ackNumber);

                        pNetworkPackets.Data[startTCPPacket + 8] = ackNumberByte[0]; // 38684 4000250982
                        pNetworkPackets.Data[startTCPPacket + 9] = ackNumberByte[1];
                        pNetworkPackets.Data[startTCPPacket + 10] = ackNumberByte[2];
                        pNetworkPackets.Data[startTCPPacket + 11] = ackNumberByte[3];

                        byte[] seqNumberByte = ConvertingField.SplittingAn4Integer(seqNumber);

                        pNetworkPackets.Data[startTCPPacket + 4] = seqNumberByte[0];
                        pNetworkPackets.Data[startTCPPacket + 5] = seqNumberByte[1];
                        pNetworkPackets.Data[startTCPPacket + 6] = seqNumberByte[2];
                        pNetworkPackets.Data[startTCPPacket + 7] = seqNumberByte[3];

                        {
                            long sequence = ConvertingField.ToAnIntegerValue(
                                new byte[] { seqNumberByte[0], seqNumberByte[1], seqNumberByte[2], seqNumberByte[3] });

                            pNetworkPackets.SequenceNumber = sequence;
                        }

                        {
                            long acknowledgment = ConvertingField.ToAnIntegerValue(
                                new byte[] { ackNumberByte[0], ackNumberByte[1], ackNumberByte[2], ackNumberByte[3] });

                            pNetworkPackets.AcknowledgmentNumber = acknowledgment;
                        }
                    }
                }
                else if (pNetworkPackets.IsAckPush())
                {
                    byte[] ackNumberByte = ConvertingField.SplittingAn4Integer(ackNumber);

                    pNetworkPackets.Data[startTCPPacket + 8] = ackNumberByte[0];
                    pNetworkPackets.Data[startTCPPacket + 9] = ackNumberByte[1];
                    pNetworkPackets.Data[startTCPPacket + 10] = ackNumberByte[2];
                    pNetworkPackets.Data[startTCPPacket + 11] = ackNumberByte[3];

                    {

                        long acknowledgment = ConvertingField.ToAnIntegerValue(
                            new byte[] { ackNumberByte[0], ackNumberByte[1], ackNumberByte[2], ackNumberByte[3] });

                        pNetworkPackets.AcknowledgmentNumber = acknowledgment;
                    }
                }

                byte[] fieldPort = ConvertingField.SplittingAn2Integer1(clientPort);
                pNetworkPackets.Data[startTCPPacket + 2] = fieldPort[0];
                pNetworkPackets.Data[startTCPPacket + 3] = fieldPort[1];

                {
                    pNetworkPackets.DestinationPort = ConvertingField.ToAnIntegerValue(
                    pNetworkPackets.Data[startTCPPacket + 2],
                    pNetworkPackets.Data[startTCPPacket + 3]);
                }

                byte[] totalLengthTCP = ConvertingField.SplittingAn2Integer1(pNetworkPackets.HeaderLength_TCP + (pNetworkPackets.EndIndexSegments - pNetworkPackets.StartIndexSegments));

                byte[] pseudoTCPheader = new byte[]
                {
                pNetworkPackets.Data[startIPPacket + 12],
                pNetworkPackets.Data[startIPPacket + 13],
                pNetworkPackets.Data[startIPPacket + 14],
                pNetworkPackets.Data[startIPPacket + 15],

                pNetworkPackets.Data[startIPPacket + 16],
                pNetworkPackets.Data[startIPPacket + 17],
                pNetworkPackets.Data[startIPPacket + 18],
                pNetworkPackets.Data[startIPPacket + 19],

                0,
                (byte)pNetworkPackets.Protocol,

                totalLengthTCP[0], totalLengthTCP[1]
                };


                int tcpCheckSum = ConvertingField.ComputeHeaderTCPChecksum(pseudoTCPheader, pNetworkPackets.Data,
                    startTCPPacket, pNetworkPackets.EndIndexSegments - startTCPPacket);

                byte[] tcpCheckSumByte = ConvertingField.SplittingAn2Integer1(tcpCheckSum);
                pNetworkPackets.Data[startTCPPacket + 16] = tcpCheckSumByte[0];
                pNetworkPackets.Data[startTCPPacket + 17] = tcpCheckSumByte[1];
                {
                    pNetworkPackets.Checksum = tcpCheckSum;
                }

                int ipCheckSum = ConvertingField.ComputeHeaderIPChecksum(pNetworkPackets.Data,
                    startIPPacket, startIPPacket + pNetworkPackets.HeaderLength_IP);
                byte[] ipCheckSumByte = ConvertingField.SplittingAn2Integer1(ipCheckSum);
                pNetworkPackets.Data[startIPPacket + 10] = ipCheckSumByte[0];
                pNetworkPackets.Data[startIPPacket + 11] = ipCheckSumByte[1];
                {
                    pNetworkPackets.HeaderChecksum = ipCheckSum;
                }

                return new Buffer.IndexedByte(pNetworkPackets.Data, pNetworkPackets.StartIndexIPPacket, pNetworkPackets.EndIndexSegments);
            }
        }

        /// <summary>
        /// Передаем номер порта который использует наш сервер, и получаем номер порта
        /// который использует клиент на своем устройвтве.
        /// </summary>
        /// <param name="pPort"></param>
        /// <returns></returns>
        public int GetTLSPortClient(int pPort)
        {
            lock(Locker)
            {
                foreach(var tlsPorts in ClientPorts.TLSPortsArray)
                {
                    if (!tlsPorts.Free)
                    {
                        if (tlsPorts.ServerLocalPort_1 == pPort)
                            return tlsPorts.ClientNumberPort_1;

                        if (tlsPorts.ServerLocalPort_2 == pPort)
                            return tlsPorts.ClientNumberPort_2;

                        if (tlsPorts.ServerLocalPort_3 == pPort)
                            return tlsPorts.ClientNumberPort_3;
                    }
                }

                return -1;
            }
        }

        public void FreePort(ClientPorts.Port pPort)
        {
            lock(Locker)
            {

            }
        }

        public void FreeTLSPorts(ClientPorts.TLSPorts pTLSPorts)
        {
            lock(Locker)
            {
                int index = pTLSPorts.Index;
                {
                    ClientPorts.TLSPortsArray[index].Free = true;
                    ClientPorts.TLSPortsArray[index].ClientNumberPort_1 = -1;
                    ClientPorts.TLSPortsArray[index].ClientNumberPort_2 = -1;
                    ClientPorts.TLSPortsArray[index].ClientNumberPort_3 = -1;
                    ClientPorts.TLSPortsArray[index].ConnectionKey = "";
                    ClientPorts.TLSPortsArray[index].DateTime = System.DateTime.Now;
                }
            }
        }
    }
}

