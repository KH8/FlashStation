using System;
using System.Collections;

namespace _ttAgent.DataAquisition
{
    static class DataMapper
    {
        #region Methods

        public static Byte Read8Bits(byte[] valByte, int pos)
        {
            return valByte[pos + 0];
        }

        public static BitArray Read16Bits(byte[] valByte, int pos)
        {
            var valByte1 = new byte[2];
            valByte1[0] = valByte[pos + 0];
            valByte1[1] = valByte[pos + 1];
            return new BitArray(new int[] { BitConverter.ToUInt16(valByte1, 0) });
        }

        public static BitArray Read32Bits(byte[] valByte, int pos)
        {
            var valByte1 = new byte[4];
            valByte1[0] = valByte[pos + 0];
            valByte1[1] = valByte[pos + 1];
            valByte1[2] = valByte[pos + 2];
            valByte1[3] = valByte[pos + 3];
            return new BitArray(new int[] { BitConverter.ToUInt16(valByte1, 0) });
        }

        public static Boolean ReadSingleBit(byte[] valByte, int pos, int bitpos)
        {
            var bitArray = Read16Bits(valByte, pos);
            return bitArray[bitpos];
        }

        public static Int16 ReadInteger(byte[] valByte, int pos)
        {
            var valByte1 = new byte[2];
            valByte1[1] = valByte[pos + 0];
            valByte1[0] = valByte[pos + 1];
            return BitConverter.ToInt16(valByte1, 0);
        }

        public static Int32 ReadDInteger(byte[] valByte, int pos)
        {
            var valByte1 = new byte[4];
            valByte1[3] = valByte[pos + 0];
            valByte1[2] = valByte[pos + 1];
            valByte1[1] = valByte[pos + 2];
            valByte1[0] = valByte[pos + 3];
            return BitConverter.ToInt32(valByte1, 0);
        }

        public static float ReadReal(byte[] valByte, int pos)
        {
            var valByte1 = new byte[4];
            valByte1[3] = valByte[pos + 0];
            valByte1[2] = valByte[pos + 1];
            valByte1[1] = valByte[pos + 2];
            valByte1[0] = valByte[pos + 3];
            return BitConverter.ToSingle(valByte1, 0);
        }

        public static string ReadString(byte[] valByte, int pos, int stringLength)
        {
            var valChar = new char[stringLength];
            for (int i = 0; i < stringLength; i++)
            {
                valChar[i] = Convert.ToChar(valByte[pos + i]);
                if (valByte[pos + i] == 0x00) valChar[i] = ' ';
            }
            return new string(valChar);
        }

        public static void Write8Bits(byte[] valByte, int pos, Byte valByteSource)
        {
            valByte[pos + 0] = valByteSource;
        }

        public static void Write16Bits(byte[] valByte, int pos, BitArray valBitArray)
        {
            var valByte1 = new byte[2];
            valBitArray.CopyTo(valByte1, 0);
            valByte[pos + 0] = valByte1[0];
            valByte[pos + 1] = valByte1[1];
        }

        public static void Write32Bits(byte[] valByte, int pos, BitArray valBitArray)
        {
            var valByte1 = new byte[4];
            valBitArray.CopyTo(valByte1, 0);
            valByte[pos + 0] = valByte1[0];
            valByte[pos + 1] = valByte1[1];
            valByte[pos + 2] = valByte1[2];
            valByte[pos + 3] = valByte1[3];
        }

        public static void WriteSingleBit(byte[] valByte, int pos, int bitpos, Boolean value)
        {
            var bitArray = Read16Bits(valByte, pos);
            bitArray[bitpos] = value;
            Write16Bits(valByte, pos, bitArray);
        }

        public static void WriteInteger(byte[] valByte, int pos, Int16 valInteger)
        {
            byte[] valByte1 = BitConverter.GetBytes(valInteger);
            valByte[pos + 0] = valByte1[1];
            valByte[pos + 1] = valByte1[0];
        }

        public static void WriteDInteger(byte[] valByte, int pos, Int32 valDInteger)
        {
            byte[] valByte1 = BitConverter.GetBytes(valDInteger);
            valByte[pos + 0] = valByte1[3];
            valByte[pos + 1] = valByte1[2];
            valByte[pos + 2] = valByte1[1];
            valByte[pos + 3] = valByte1[0];
        }

        public static void WriteReal(byte[] valByte, int pos, float valReal)
        {
            byte[] valByte1 = BitConverter.GetBytes(valReal);
            valByte[pos + 0] = valByte1[3];
            valByte[pos + 1] = valByte1[2];
            valByte[pos + 2] = valByte1[1];
            valByte[pos + 3] = valByte1[0];
        }

        public static void WriteString(byte[] valByte, int pos, string valString)
        {
            int sizeString = valString.Length;
            char[] valChar = valString.ToCharArray();
            for (int i = 0; i < sizeString; i++)
                valByte[pos + i] = Convert.ToByte(valChar[i]);
        }

        #endregion
    }
}
