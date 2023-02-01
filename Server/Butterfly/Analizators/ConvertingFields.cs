using System;
using System.Text;

namespace Butterfly
{
    public class ConvertingFields
    {
        /// <summary>
        /// Переводим чисто в десятичной системе исчеление в двоичное.
        /// </summary>
        /// <param name="pNumber"></param>
        /// <returns></returns>
        public byte[] NumberToBinaryArray(int pNumber)
        {
            
            string binaryCode = Convert.ToString(pNumber, 2);

            byte[] r = new byte[binaryCode.Length];
            {
                for (int i = 0; i < binaryCode.Length; i++)
                {
                    if (binaryCode[i] == '0') r[i] = 0;
                    else r[i] = 1;
                }
            }

            return r;
        }

        /// <summary>
        /// Переводим чисто в десятичной системе исчеление в двоичное.
        /// </summary>
        /// <param name="pNumber"></param>
        /// <returns></returns>
        public string NumberToBinaryString(int pNumber)
        {
            return Convert.ToString(pNumber, 2);
        }

        /// <summary>
        /// Переводим массив байтов в строку.
        /// </summary>
        /// <param name="pByteArray"></param>
        /// <returns></returns>
        public string ByteArrayToString(byte[] pByteArray)
        {
            return Encoding.UTF8.GetString(pByteArray);
        }

       
        /// <summary>
        /// Приводим строку содержащюю бинарный код к целочисленому значению в десятичной системе исчесления.
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public int BinaryToNumberInt(string pString)
        {
            return Convert.ToInt32(pString, 2);
        }

        /// <summary>
        /// Увеличиваем размер массива байт с начала массива.
        /// </summary>
        /// <param name="pByteArray"></param>
        /// <param name="pNewSize"></param>
        /// <returns></returns>
        public byte[] FirstResizeArray(byte[] pByteArray, int pNewSize)
        {
            if (pByteArray.Length >= pNewSize) return pByteArray;

            // Узнаем сколько не хватает.
            int sizeByteArrayBuffer = pNewSize - pByteArray.Length;
            byte[] arrayBuffer = new byte[pNewSize];
            {
                for (int i = 0; i < sizeByteArrayBuffer; i++) arrayBuffer[i] = 0;
                for (int i = 0; i < pByteArray.Length; i++) arrayBuffer[i + sizeByteArrayBuffer] = pByteArray[i];
            }

            return arrayBuffer;
        }


        /// <summary>
        /// Преобразовать в целое значение.
        /// </summary>
        /// <param name="pFirstByte"></param>
        /// <param name="pLastByte"></param>
        /// <returns></returns>
        public int ToAnIntegerValue(byte pFirstByte, byte pLastByte)
        {
            int result = 0;

            result = pFirstByte * 256 + pLastByte;

            return result;
        }
        /// <summary>
        /// Преобразовать в целое значение. В параметр передается массив из 4 значений.
        /// </summary>
        /// <returns></returns>
        public long ToAnIntegerValue(byte[] pBytes)
        {
            long result = 0;

            if (pBytes.Length != 4) return result;

            long byte_1 = pBytes[0]; long byte_2 = pBytes[1]; long byte_3 = pBytes[2]; long byte_4 = pBytes[3];

            result = ((byte_1 * 256 + byte_2) * 256 + byte_3) * 256 + byte_4;

            return result;
        }

        /// <summary>
        /// Разбиение целого значения на 2
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        public byte[] SplittingAn2Integer(int pValue)
        {
            int i = 0; int r = 0;
            if (pValue > 256)
            {
                i = pValue / 256;
                r = pValue - (i * 256);
            }
            else
                r = pValue;

            byte[] bytes = new byte[2];
            bytes[0] = (byte)i;
            bytes[1] = (byte)r;

            return bytes;
        }

        public byte[] SplittingAn2Integer1(int pValue)
        {
            long i, r;

            i = pValue % 256;
            r = (pValue / 256) % 256;

            byte[] bytes = new byte[2];
            bytes[0] = (byte)r;
            bytes[1] = (byte)i;

            return bytes;
        }

        public byte[] SplittingAn4Integer(long pValue)
        {
            long i = 0; long r = 0; long t = 0; long v = 0;

            i = pValue % 256;
            r = (pValue / 256) % 256;
            t = (pValue / (256 * 256)) % 256;
            v = (pValue / (256 * 256 * 256));

            byte[] bytes = new byte[4];
            bytes[3] = (byte)i;
            bytes[2] = (byte)r;
            bytes[1] = (byte)t;
            bytes[0] = (byte)v;

            return bytes;
        }
        /// <summary>
        /// SourceAddress, DestinationAddress, 0, (TPC = 6, UDP = 17 number), TCP-HEADER + TCP-SEGMENT_LENGTH(полезная нагрузка),
        /// TCP - Header
        /// Все сегмент данные.
        /// </summary>
        public ushort ComputeHeaderTCPChecksum(byte[] pPseudoTCPheader, byte[] pData, 
            int pStartTCPHeader, int pTCPLength)
        {
            long pseudoHeader = Convert8To16(pPseudoTCPheader, 0, pPseudoTCPheader.Length);

            pData[pStartTCPHeader + 16] = 0; 
            pData[pStartTCPHeader + 17] = 0;
            
            long result = Convert8To16(pData, pStartTCPHeader, pTCPLength, pseudoHeader);

            while ((result >> 16) != 0)
                result = (result & 0xFFFF) + (result >> 16);

            return ((ushort)~result); // & 0xffff;

            //int i = ((int)~result) & 0xffff;

            //return i;

            //return (ushort)~result;
        }

        public ushort ComputeHeaderIPChecksum(byte[] pData, int pStartIPPacket, int pIPPacketLength)
        {
            long result = 0;

            pData[pStartIPPacket + 10] = 0; 
            pData[pStartIPPacket + 11] = 0;

            result += Convert8To16(pData, pStartIPPacket, pIPPacketLength);

            while ((result >> 16) != 0)
                result = (result & 0xFFFF) + (result >> 16);

            return (ushort)~result;
        }

        public long Convert8To16(byte[] pHeader, int pStart, int pLength, long pSum = 0)
        {
            long sum = pSum;
            
            if ((pStart + pLength) <= pHeader.Length)
            {
                if ((pLength % 2) == 0)
                {
                    for (int i = 0; i < pLength; i += 2)
                    {
                        ushort word16 = (ushort)((pHeader[pStart + i] << 8) + pHeader[pStart + i + 1]);
                        sum += word16;
                    }
                }
                else
                {
                    if ((pStart + pLength) < pHeader.Length)
                    {
                        pHeader[pStart + pLength] = 0;

                        for (int i = 0; i < (pLength + 1 - pStart); i += 2)
                        {
                            ushort word16 = (ushort)((pHeader[pStart + i] << 8) + pHeader[pStart + i + 1]);
                            sum += word16;
                        }
                    }
                    else
                    {
                        byte[] b = new byte[4096];
                        Array.Copy(pHeader, pStart, b, pStart, pLength);

                        b[pLength - pStart + 1] = 0;

                        for (int i = 0; i < (pLength - pStart + 1); i += 2)
                        {
                            ushort word16 = (ushort)((b[i] << 8) + b[i + 1]);
                            sum += word16;
                        }
                    }

                }
            }


            return sum;
        }

        public ushort ComputeHeaderChecksum(byte[] header, int start, int length)
        {
            long sum = Convert8To16(header, start, length);

            while ((sum >> 16) != 0)
            {
                sum = (sum & 0xFFFF) + (sum >> 16);
            }

            return (ushort)~sum;
        }
    }
}
