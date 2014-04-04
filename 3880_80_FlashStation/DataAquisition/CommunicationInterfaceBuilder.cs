using System;
using System.IO;

namespace _3880_80_FlashStation.DataAquisition
{
    static class CommunicationInterfaceBuilder
    {
        public static CommunicationInterfaceComposite InitializeInterface(string type)
        {
            bool readAreaFound = false;
            bool writeAreaFound = false;

            int readStartAddress = -1;
            int writeStartAddress = -1;

            var interfaceComposite = new CommunicationInterfaceComposite(type);
            var reader = new StreamReader(CommunicationInterfacePath.Default.Path);

            string line;
            string[] words;

            switch (type)
            {
                case "readInterface":
                    while (true)
                    {
                        line = reader.ReadLine();
                        if (line == null) break;
                        words = line.Split(';');
                        if (readAreaFound && words[0] == "#END") break;
                        if (readAreaFound)
                        {
                            if (readStartAddress == -1) readStartAddress = Convert.ToUInt16(words[0]);
                            switch (words[2])
                            {
                                case "INT":
                                    words[2] = "Integer";
                                    break;
                                case "REAL":
                                    words[2] = "Real";
                                    break;
                            }
                            interfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable(words[1], Convert.ToUInt16(words[0]) - readStartAddress, words[2]));
                        }
                        if (words[0] == "#READ") readAreaFound = true;
                    }
                    if (!readAreaFound) { throw new Exception("Read Area not found"); }
                    break;
                case "writeInterface":
                    while (true)
                    {
                        line = reader.ReadLine();
                        if (line == null) break;
                        words = line.Split(';');
                        if (writeAreaFound && words[0] == "#END") break;
                        if (writeAreaFound)
                        {
                            if (writeStartAddress == -1) writeStartAddress = Convert.ToUInt16(words[0]);
                            if (words[2] == "INT") words[2] = "Integer";
                            switch (words[2])
                            {
                                case "INT":
                                    words[2] = "Integer";
                                    break;
                                case "REAL":
                                    words[2] = "Real";
                                    break;
                            }
                            interfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable(words[1], Convert.ToUInt16(words[0]) - writeStartAddress, words[2]));
                        }
                        if (words[0] == "#WRITE") writeAreaFound = true;
                    }
                    if (!writeAreaFound) { throw new Exception("Write Area not found"); }
                    break;
                default: throw new Exception("Error: Wrong interface type.");    
            }
            return interfaceComposite;
        }
    }
}
