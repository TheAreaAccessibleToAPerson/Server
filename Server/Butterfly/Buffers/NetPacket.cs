namespace Butterfly.Buffer
{
    public struct NetPacket : IBuffer
    {


        //**************************************//
        //             IP_HEADER                //
        //**************************************//

        public int Version;
        public int HeaderLength_IP;
        public int TypeService;
        public int TotalLength; // HeaderLength_IP + HeaderLength_TCP + SegmentLength
        public int Identification;
        public bool DontFragmentFlag, MoreFragmentsFlag;
        public int FragmentOffset;
        public int TimeToLive;
        public int Protocol;
        public int HeaderChecksum;
        public byte[] SourceAddress;
        public byte[] DestinationAddress;

        //**************************************//

        //**************************************//
        //             TCP_HEADER               //
        //**************************************//

        public int SourcePort;
        public int DestinationPort;
        public long SequenceNumber;
        public long AcknowledgmentNumber;
        public int HeaderLength_TCP;
        public int SegmentLength;
        public bool URG, ACK, PUSH, RST, SYN, FIN;
        public int WindowSize;
        public int Checksum;
        public int UrgentPointer;

        //**************************************//
        //               DATA                   //
        //**************************************//

        public byte[] Data;

        // Индекс первого байта с которого начинается хранение заголовка IP пакета.
        public int StartIndexIPPacket;

        // Индекс первого байта с которого начинается хранение заголовка TCP пакета.
        public int StartIndexTCPPacket;

        // С этого места начинаются данные.
        public int StartIndexSegments;

        // В этом месте заканчиваются данные.
        public int EndIndexSegments;


        //**************************************//

        public bool IsSyn() => !URG && !ACK && !PUSH && !RST && SYN && !FIN;
        public bool IsSynAck() => !URG && ACK && !PUSH && !RST && SYN && !FIN;
        public bool IsAckPush() => !URG && ACK && PUSH && !RST && !SYN && !FIN;
        public bool IsAck() => !URG && ACK && !PUSH && !RST && !SYN && !FIN;
        public bool IsUrgen() => URG && !ACK && !PUSH && !RST && !SYN && !FIN;

        public System.Net.IPAddress GetSourceAddress() => new System.Net.IPAddress(SourceAddress);
        public System.Net.IPAddress GetDestinationAddress() => new System.Net.IPAddress(DestinationAddress);

        public string GetFlagString()
        {
            string result = "";
            {
                result += $"\n**************************************************************************";
                result += $"\n[URG]{URG}, [ACK]{ACK}, [PUSH]{PUSH}, [RST]{RST}, [SYN]{SYN}, [FIN]{FIN}";
                result += $"\n**************************************************************************";
            }
            return result;
        }

        public string GetPacketString(string pPointerGet = "")
        {
            string result = "";
            {
                result += "\n*********************************************************************************";
                result += "\n [IP]" + pPointerGet;
                result += "\n";
                result += $"\n[Version]{Version}";
                result += $"\n[HeaderLength]{HeaderLength_IP}";
                result += $"\n[TotalLength]{TotalLength}";
                result += $"\n[Identification]{Identification}";
                result += $"\n[DontFragmentFlag]{DontFragmentFlag}, [MoreFragmentsFlag]{MoreFragmentsFlag}";
                result += $"\n[FragmentOffset]{FragmentOffset}";
                result += $"\n[TimeToLive]{TimeToLive}";
                result += $"\n[Protocol]{Protocol}";
                result += $"\n[HeaderChecksum]{HeaderChecksum}";
                result += $"\n[SourceAddress]{GetSourceAddress()}";
                result += $"\n[DestinationAddress]{GetDestinationAddress()}";
                result += "\n*********************************************************************************";

                result += $"\n**************************************************************************";
                result += $"\n[TCP]" + pPointerGet;
                result += "\n";
                result += $"\n[SourcePort]{SourcePort}";
                result += $"\n[DestinationPort]{DestinationPort}";
                result += $"\n[SequenceNumber]{SequenceNumber}";
                result += $"\n[AcknowledgmentNumber]{AcknowledgmentNumber}";
                result += $"\n[HeaderLength]{HeaderLength_TCP}";
                result += $"\n[TotalLength]{TotalLength}";
                result += $"\n[URG]{URG}, [ACK]{ACK}, [PUSH]{PUSH}, [RST]{RST}, [SYN]{SYN}, [FIN]{FIN}";
                result += $"\n[WindowSize]{WindowSize}";
                result += $"\n[Checksum]{Checksum}";
                result += $"\n[UrgentPointer]{UrgentPointer}";
                result += $"\n**************************************************************************";
            }
            return result;
        }

        public string GetName()
        {
            return typeof(NetPacket).Name;
        }
    }
}
