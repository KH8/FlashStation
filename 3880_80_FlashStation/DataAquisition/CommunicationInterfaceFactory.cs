using System;
using System.Collections;

namespace _3880_80_FlashStation.DataAquisition
{
    static public class CommunicationInterfaceFactory
    {
        public static CommunicationInterfaceVariable CreateVariable(string name, int pos, string type)
        {
            switch (type)
            {
                case "BitArray":
                    return new CiBitArray(name, pos, type, new BitArray(2));
                case "Integer":
                    return new CiInteger(name, pos, type, 0);
                case "DoubleInteger":
                    return new CiDoubleInteger(name, pos, type, 0);
                case "Real":
                    return new CiReal(name, pos, type, 0.0f);
                case "String":
                    return new CiString(name, pos, type, "", 0);
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
