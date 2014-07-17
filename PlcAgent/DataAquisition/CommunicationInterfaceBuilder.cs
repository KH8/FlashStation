using System;
using System.IO;

namespace _PlcAgent.DataAquisition
{
    static class CommunicationInterfaceBuilder
    {
        public struct Address
        {
            public int ByteAddress;
            public int BitAddress;
        }

        public static CommunicationInterfaceComposite InitializeInterface(uint id, CommunicationInterfaceComponent.InterfaceType type, CommunicationInterfacePath pathFile)
        {
            var readAreaFound = false;
            var writeAreaFound = false;

            var readAddress = new Address { ByteAddress = 0, BitAddress = 0, };
            var writeAddress = new Address { ByteAddress = 0, BitAddress = 0, };

            var readByteOverloaded = false;
            var writeByteOverloaded = false;

            var interfaceComposite = new CommunicationInterfaceComposite(type.ToString());
            var reader = new StreamReader(pathFile.Path[id]);

            string previousReadType = "";
            string previousWriteType = "";

            string line;
            string[] words;

            switch (type)
            {
                case CommunicationInterfaceComponent.InterfaceType.ReadInterface:
                    while (true)
                    {
                        //todo gotowiec?
                        line = reader.ReadLine();
                        if (line == null) break;
                        words = line.Split(';');
                        if (readAreaFound && words[0] == "#END") break;
                        if (readAreaFound)
                        {
                            var readByteOverloadedAux = readByteOverloaded;
                            readAddress = CheckRules(previousReadType, words[1], readByteOverloadedAux, readAddress, out readByteOverloaded);
                            interfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable(words[0], readAddress.ByteAddress, readAddress.BitAddress, StringToVariableType(words[1]), GetLength(words[1])));
                            readAddress = CreateNewAddress(readAddress, words[1]);
                            previousReadType = words[1];
                        }
                        if (words[0] == "#READ") readAreaFound = true;
                    }
                    if (!readAreaFound) { throw new Exception("Read Area not found"); }
                    break;
                case CommunicationInterfaceComponent.InterfaceType.WriteInterface:
                    while (true)
                    {
                        line = reader.ReadLine();
                        if (line == null) break;
                        words = line.Split(';');
                        if (writeAreaFound && words[0] == "#END") break;
                        if (writeAreaFound)
                        {
                            var writeByteOverloadedAux = writeByteOverloaded;
                            writeAddress = CheckRules(previousWriteType, words[1], writeByteOverloadedAux, writeAddress, out writeByteOverloaded);
                            interfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable(words[0], writeAddress.ByteAddress, writeAddress.BitAddress, StringToVariableType(words[1]), GetLength(words[1])));
                            writeAddress = CreateNewAddress(writeAddress, words[1]);
                            previousWriteType = words[1];
                        }
                        if (words[0] == "#WRITE") writeAreaFound = true;
                    }
                    if (!writeAreaFound) { throw new Exception("Write Area not found"); }
                    break;
                default: throw new Exception("Error: Wrong interface type.");    
            }
            return interfaceComposite;
        }

        internal static CommunicationInterfaceComponent.VariableType StringToVariableType(string typeString)
        {
            var type = typeString.Split('[');
            var variableType = new CommunicationInterfaceComponent.VariableType();
            switch (type[0])
            {
                case "BOOL":
                    variableType = CommunicationInterfaceComponent.VariableType.Bit;
                    break;
                case "BYTE":
                    variableType = CommunicationInterfaceComponent.VariableType.Byte;
                    break;
                case "CHAR":
                    variableType = CommunicationInterfaceComponent.VariableType.Char;
                    break;
                case "WORD":
                    variableType = CommunicationInterfaceComponent.VariableType.Word;
                    break;
                case "DWORD":
                    variableType = CommunicationInterfaceComponent.VariableType.DoubleWord;
                    break;
                case "INT":
                    variableType = CommunicationInterfaceComponent.VariableType.Integer;
                    break;
                case "TIME":
                    variableType = CommunicationInterfaceComponent.VariableType.Integer;
                    break;
                case "DINT":
                    variableType = CommunicationInterfaceComponent.VariableType.DoubleInteger;
                    break;
                case "REAL":
                    variableType = CommunicationInterfaceComponent.VariableType.Real;
                    break;
                case "STRING ":
                    variableType = CommunicationInterfaceComponent.VariableType.String;
                    break;
            }
            return variableType;
        }

        internal static Address CreateNewAddress(Address address, string typeString)
        {
            var oldAddress = address;
            var newAddress = new Address { BitAddress = 0, ByteAddress = 0 };

            var type = typeString.Split('[');
            switch (type[0])
            {
                case "BOOL":
                    newAddress.ByteAddress = oldAddress.ByteAddress;
                    newAddress.BitAddress = oldAddress.BitAddress + 1;
                    break;
                case "BYTE":
                    newAddress.ByteAddress = oldAddress.ByteAddress + 1;
                    newAddress.BitAddress = 0;
                    break;
                case "CHAR":
                    newAddress.ByteAddress = oldAddress.ByteAddress + 1;
                    newAddress.BitAddress = 0;
                    break;
                case "WORD":
                    newAddress.ByteAddress = oldAddress.ByteAddress + 2;
                    newAddress.BitAddress = 0;
                    break;
                case "DWORD":
                    newAddress.ByteAddress = oldAddress.ByteAddress + 4;
                    newAddress.BitAddress = 0;
                    break;
                case "INT":
                    newAddress.ByteAddress = oldAddress.ByteAddress + 2;
                    newAddress.BitAddress = 0;
                    break;
                case "TIME":
                    newAddress.ByteAddress = oldAddress.ByteAddress + 2;
                    newAddress.BitAddress = 0;
                    break;
                case "DINT":
                    newAddress.ByteAddress = oldAddress.ByteAddress + 4;
                    newAddress.BitAddress = 0;
                    break;
                case "REAL":
                    newAddress.ByteAddress = oldAddress.ByteAddress + 4;
                    newAddress.BitAddress = 0;
                    break;
                case "STRING ":
                    var typeExtension = type[1].Split(']');
                    newAddress.ByteAddress = oldAddress.ByteAddress + Convert.ToInt32(typeExtension[0]);
                    newAddress.BitAddress = 0;
                    break;
            }
            return newAddress;
        }

        internal static int GetLength(string typeString)
        {
            var type = typeString.Split('[');
            var length = 0;
            switch (type[0])
            {
                case "STRING ":
                    var typeExtension = type[1].Split(']');
                    length = Convert.ToInt32(typeExtension[0]);
                    break;
            }
            return length;
        }

        internal static Address CheckRules(string previousType, string actualType, Boolean overloadIn, Address address, out Boolean overloadOut)
        {
            var newAddress = address;
            overloadOut = overloadIn;

            if (previousType == "BOOL")
            {
                if (actualType != "BOOL")
                {
                    if (overloadIn)
                    {
                        newAddress.ByteAddress = address.ByteAddress + 1;
                        overloadOut = false;
                    }
                    else if (actualType == "BYTE" || actualType == "CHAR") newAddress.ByteAddress = address.ByteAddress + 1;
                    else newAddress.ByteAddress = address.ByteAddress + 2;
                    address.BitAddress = 0;
                }
                if (actualType == "BOOL" && address.BitAddress == 8)
                {
                    newAddress.ByteAddress = address.ByteAddress + 1;
                    newAddress.BitAddress = 0;
                    overloadOut = true;
                }
            }
            if (previousType == "BYTE" || previousType == "CHAR")
            {
                var moduloAddress = address.ByteAddress % 2;
                if (actualType != "BYTE" && actualType != "CHAR" && moduloAddress != 0)
                {
                    newAddress.ByteAddress = address.ByteAddress + 1;
                }
            }

            return newAddress;
        }
    }
}
