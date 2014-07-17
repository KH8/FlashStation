using System;
using System.Collections;

namespace _PlcAgent.DataAquisition
{
    static public class CommunicationInterfaceFactory
    {
        public static CommunicationInterfaceVariable CreateVariable(string name, int pos, int posBit, CommunicationInterfaceComponent.VariableType type, int length)
        {
            switch (type)
            {
                case CommunicationInterfaceComponent.VariableType.Bit:
                    return new CiBit(name, pos, posBit, type, false);
                case CommunicationInterfaceComponent.VariableType.Byte:
                    return new CiByte(name, pos, type, 0);
                case CommunicationInterfaceComponent.VariableType.Char:
                    return new CiChar(name, pos, type, ' ');
                case CommunicationInterfaceComponent.VariableType.Word:
                    return new CiWord(name, pos, type, new BitArray(2));
                case CommunicationInterfaceComponent.VariableType.DoubleWord:
                    var bitArrays = new BitArray[2];
                    bitArrays[0] = new BitArray(2);
                    bitArrays[1] = new BitArray(2);
                    return new CiDoubleWord(name, pos, type, bitArrays);
                case CommunicationInterfaceComponent.VariableType.Integer:
                    return new CiInteger(name, pos, type, 0);
                case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                    return new CiDoubleInteger(name, pos, type, 0);
                case CommunicationInterfaceComponent.VariableType.Real:
                    return new CiReal(name, pos, type, 0.0f);
                case CommunicationInterfaceComponent.VariableType.String:
                    return new CiString(name, pos, type, "", length);
                default:
                    throw new FactoryException("Cannot create a variable");
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
