namespace Butterfly.Analiz
{
    class NetPackets : Analizator<Buffer.Byte, Buffer.NetPackets>
    {
        protected override void Process(Buffer.Byte pValue)
        {
            // Ищем начало IPHeader. Оно должно быть не 0.
            int index = 0; // Индекс текущего читаемого байта.
            const int STEP_COUNT = 50; // Количесво шагов, после которого мы перестаем искать первый не нулевой байт.
            int StepNumber = 0; // Номер текущего шага.

            while (true)
            {
                for (int i = index; i < pValue.Size; i++)
                {
                    if (pValue.Array[i] != 0)
                    {
                        index = i;
                        break;
                    }
                    
                    if (StepNumber++ >= STEP_COUNT) return;
                }

                Buffer.NetPackets netPackets = new Buffer.NetPackets();

                netPackets.StartIndexIPPacket = index; // С данного индекса начинается чтение заголовка IP пакета.
                // Читаем IP пакет.
                netPackets.Version = pValue.Array[index] >> 4;
                netPackets.HeaderLength_IP = (pValue.Array[index] & 7) * 4;
                
                if (netPackets.HeaderLength_IP >= 20 && netPackets.StartIndexIPPacket + netPackets.HeaderLength_IP < pValue.Size)
                {
                    netPackets.TypeService = pValue.Array[++index];

                    netPackets.TotalLength = (pValue.Array[++index] << 8) | pValue.Array[++index];
                    
                    if (netPackets.TotalLength <= 1500 && netPackets.StartIndexIPPacket + netPackets.TotalLength <= pValue.Size)
                    {

                        netPackets.Identification = pValue.Array[++index] << 8 | pValue.Array[++index];
                            
                        netPackets.DontFragmentFlag = ((pValue.Array[++index] >> 6) & 1) == 1;
                        netPackets.MoreFragmentsFlag = ((pValue.Array[index] >> 5) & 1) == 1;

                        netPackets.FragmentOffset = (int)(pValue.Array[index] & ~(1UL << 7) & ~(1UL << 6) & ~(1UL << 5)) 
                                                        + pValue.Array[++index];

                        netPackets.TimeToLive = pValue.Array[++index];

                        netPackets.Protocol = pValue.Array[++index];

                        netPackets.HeaderChecksum = pValue.Array[++index] << 8 | pValue.Array[++index];

                        netPackets.SourceAddress = new byte[4] { pValue.Array[++index], pValue.Array[++index],
                                                                    pValue.Array[++index], pValue.Array[++index] };

                        netPackets.DestinationAddress = new byte[4] { pValue.Array[++index], pValue.Array[++index],
                                                                    pValue.Array[++index], pValue.Array[++index] };
                        // Выставляем индекс в конец IPHeader, и начинаем обрабатывать TCP пакет.
                        netPackets.StartIndexTCPPacket = index = netPackets.StartIndexIPPacket + netPackets.HeaderLength_IP;

                        netPackets.SourcePort = (pValue.Array[index] << 8) + pValue.Array[++index];

                        netPackets.DestinationPort = (pValue.Array[++index] << 8) + pValue.Array[++index];

                        netPackets.SequenceNumber = (long)pValue.Array[++index] << 24 | (long)pValue.Array[++index] << 16 |
                                                    (long)pValue.Array[++index] << 8 | pValue.Array[++index];

                        netPackets.AcknowledgmentNumber = (long)pValue.Array[++index] << 24 | (long)pValue.Array[++index] << 16 |
                                                    (long)pValue.Array[++index] << 8 | pValue.Array[++index];

                        netPackets.HeaderLength_TCP = (pValue.Array[++index] >> 4) * 4;

                        ulong flags = (pValue.Array[index] & ~(1UL << 7) & ~(1UL << 6) & ~(1UL << 5) & ~(1UL << 4)) 
                                        + pValue.Array[++index];

                        netPackets.URG = ((flags >> 5) & 1) == 1;
                        netPackets.ACK = ((flags >> 4) & 1) == 1;
                        netPackets.PUSH = ((flags >> 3) & 1) == 1;
                        netPackets.RST = ((flags >> 2) & 1) == 1;
                        netPackets.SYN = ((flags >> 1) & 1) == 1;
                        netPackets.FIN = ((flags >> 0) & 1) == 1;

                        netPackets.WindowSize = pValue.Array[++index] << 8 | pValue.Array[++index];

                        netPackets.Checksum = pValue.Array[++index] << 8 | pValue.Array[++index];

                        netPackets.UrgentPointer = pValue.Array[++index] << 8 | pValue.Array[++index];

                        netPackets.SegmentLength = netPackets.TotalLength - netPackets.HeaderLength_IP - netPackets.HeaderLength_TCP;
                        netPackets.StartIndexSegments = netPackets.StartIndexTCPPacket + netPackets.HeaderLength_TCP;
                        netPackets.EndIndexSegments = netPackets.StartIndexIPPacket + netPackets.TotalLength;

                        netPackets.Data = pValue.Array;

                        index = netPackets.StartIndexIPPacket + netPackets.TotalLength;

                        Add(netPackets);
                    }
                    else
                        return;
                }
                else
                    return;
                

                // Сбросим счетчик первого не нулевого байта.
                StepNumber = 0;
            }
        }
    }
}
