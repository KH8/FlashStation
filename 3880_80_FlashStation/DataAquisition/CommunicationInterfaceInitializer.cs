using System;

namespace _3880_80_FlashStation.DataAquisition
{
    static class CommunicationInterfaceInitializer
    {
        private static CommunicationInterfaceComposite _readInterfaceComposite;
        private static CommunicationInterfaceComposite _writeInterfaceComposite;

        public static CommunicationInterfaceComposite Initialize(string type)
        {
            switch (type)
            {
                case "readInterface":
                    _readInterfaceComposite = new CommunicationInterfaceComposite("SPS");
                    _readInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("BEFEHL", 0, "Integer"));
                    _readInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("PROGRAMMTYP", 2, "Integer"));
                    return _readInterfaceComposite;
                case "writeInterface":
                    _writeInterfaceComposite = new CommunicationInterfaceComposite("PC");
                    _writeInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("ANTWORT", 50, "Integer"));
                    _writeInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("STATUS", 52, "Integer"));
                    _writeInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("FEHLERCODE", 54, "Integer"));
                    return _writeInterfaceComposite;
                default:
                    throw new InitializerException("Error: Wrong interface type");
            }
        }

        #region Auxiliaries

        public class InitializerException : ApplicationException
        {
            public InitializerException(string info) : base(info) { }
        }

        #endregion
    }
}
