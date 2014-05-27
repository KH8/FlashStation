using System;
using System.IO;

namespace _3880_80_FlashStation.DataAquisition
{
    static class CommunicationInterfaceBuilder
    {
        public static CommunicationInterfaceComposite InitializeInterface(uint id, CommunicationInterfaceComponent.InterfaceType type, CommunicationInterfacePath pathFile)
        {
            bool readAreaFound = false;
            bool writeAreaFound = false;

            int readStartAddress = -1;
            int writeStartAddress = -1;

            var interfaceComposite = new CommunicationInterfaceComposite(type.ToString());
            var reader = new StreamReader(pathFile.Path[id]);

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
                            if (readStartAddress == -1) readStartAddress = Convert.ToUInt16(words[0]);
                            interfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable(words[2], Convert.ToUInt16(words[0]) - readStartAddress, Convert.ToUInt16(words[1]), StringToVariableType(words[3]), Convert.ToUInt16(words[4])));
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
                            if (writeStartAddress == -1) writeStartAddress = Convert.ToUInt16(words[0]);
                            interfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable(words[2], Convert.ToUInt16(words[0]) - writeStartAddress, Convert.ToUInt16(words[1]), StringToVariableType(words[3]), Convert.ToUInt16(words[4])));
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
            var variableType = new CommunicationInterfaceComponent.VariableType();
            switch (typeString)
            {
                case "BOOL":
                    variableType = CommunicationInterfaceComponent.VariableType.Bit;
                    break;
                case "BYTE":
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
                case "REAL":
                    variableType = CommunicationInterfaceComponent.VariableType.Real;
                    break;
                case "STRING":
                    variableType = CommunicationInterfaceComponent.VariableType.String;
                    //todo:
                    break;
            }
            return variableType;
        }
    }
}
