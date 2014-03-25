using System;
using System.Collections;

namespace _3880_80_FlashStation.DataAquisition
{
    static public class CommunicationInterfaceFactory
    {
        public static CommunicationInterfaceVariable CreateCommunicationInterfaceVariable(int id, int pos, string type)
        {
            switch (type)
            {
                case "BitArray":
                    return new CiBitArray(id, pos, type, new BitArray(2));
                case "Integer":
                    return new CiInteger(id, pos, type, 0);
                case "DoubleInteger":
                    return new CiDoubleInteger(id, pos, type, 0);
                case "Real":
                    return new CiReal(id, pos, type, 0.0f);
                case "String":
                    return new CiString(id, pos, type, "");
                default:
                    throw new FactoryException("Cannot remove from a single variable");
            }
        }

        #region Auxiliaries

        public class FactoryException : ApplicationException
        {
            public FactoryException(string info) : base(info) { }
        }

        #endregion
    }
}
