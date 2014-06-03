using System;
using System.IO;

namespace _3880_80_FlashStation.DataAquisition
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
            bool readAreaFound = false;
            bool writeAreaFound = false;

            var readAddress = new Address { ByteAddress = 0, BitAddress = 0, };
            var writeAddress = new Address { ByteAddress = 0, BitAddress = 0, };

            Boolean readByteOverloaded = false;
            Boolean writeByteOverloaded = false;

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
                        line = reader.ReadLine();
                        if (line == null) break;
                        words = line.Split(';');
                        if (readAreaFound && words[0] == "#END") break;
                        if (readAreaFound)
                        {
                            if (previousReadType == "BOOL")
                            {
                                if (words[1] != "BOOL")
                                {
                                    if (readByteOverloaded)
                                    {
                                        readAddress.ByteAddress += 1;
                                        readByteOverloaded = false;
                                    }
                                    else readAddress.ByteAddress += 2;
                                    readAddress.BitAddress = 0;
                                }
                                if (words[1] == "BOOL" && readAddress.BitAddress == 8)
                                {
                                    readAddress.ByteAddress += 1;
                                    readAddress.BitAddress = 0;
                                    readByteOverloaded = true;
                                }
                            }
                            interfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable(words[0], readAddress.ByteAddress, readAddress.BitAddress, StringToVariableType(words[1]), 0));
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
                            if (previousWriteType == "BOOL")
                            {
                                if (words[1] != "BOOL")
                                {
                                    if (writeByteOverloaded)
                                    {
                                        writeAddress.ByteAddress += 1;
                                        writeByteOverloaded = false;
                                    }
                                    else writeAddress.ByteAddress += 2;
                                    writeAddress.BitAddress = 0;
                                }
                                if (words[1] == "BOOL" && writeAddress.BitAddress == 8)
                                {
                                    writeAddress.ByteAddress += 1;
                                    writeAddress.BitAddress = 0;
                                    writeByteOverloaded = true;
                                }
                            }
                        interfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable(words[0], writeAddress.ByteAddress, writeAddress.BitAddress, StringToVariableType(words[1]), 0));
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
                    variableType = CommunicationInterfaceComponent.VariableType.Byte;
                    break;
                case "WORD":
                    variableType = CommunicationInterfaceComponent.VariableType.Word;
                    break;
                case "INT":
                    variableType = CommunicationInterfaceComponent.VariableType.Integer;
                    break;
                case "TIME":
                    variableType = CommunicationInterfaceComponent.VariableType.Integer;
                    break;
                case "DINT":
                    variableType = CommunicationInterfaceComponent.VariableType.Real;
                    break;
                case "DWORD":
                    variableType = CommunicationInterfaceComponent.VariableType.Real;
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
                case "DWORD":
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
    }
}
