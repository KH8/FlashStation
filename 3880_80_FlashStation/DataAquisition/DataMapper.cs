using System;
using System.Collections;

namespace _3880_80_FlashStation.DataAquisition
{
    static class DataMapper
    {
        #region Methods

        public static BitArray Read16Bits(byte[] valByte, int pos)
        {
            var valByte1 = new byte[2];
            valByte1[0] = valByte[pos + 0];
            valByte1[1] = valByte[pos + 1];
            return new BitArray(new int[] { BitConverter.ToUInt16(valByte1, pos) });
        }

        public static int ReadInteger(byte[] valByte, int pos)
        {
            var valByte1 = new byte[2];
            valByte1[1] = valByte[pos + 0];
            valByte1[0] = valByte[pos + 1];
            return BitConverter.ToInt16(valByte1, 0);
        }

        public static int ReadDInteger(byte[] valByte, int pos)
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
                valChar[i] = Convert.ToChar(valByte[pos + i]);
            return new string(valChar);
        }

        public static void Write16Bits(byte[] valByte, int pos, BitArray valBitArray)
        {
            var valByte1 = new byte[2];
            valBitArray.CopyTo(valByte1, 0);
            valByte[pos + 0] = valByte1[0];
            valByte[pos + 1] = valByte1[1];
        }

        public static void WriteInteger(byte[] valByte, int pos, short valInteger)
        {
            byte[] valByte1 = BitConverter.GetBytes(valInteger);
            valByte[pos + 0] = valByte1[1];
            valByte[pos + 1] = valByte1[0];
        }

        public static void WriteDInteger(byte[] valByte, int pos, int valDInteger)
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
