using System;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.DataAquisition
{
    class CommunicationInterfaceHandler
    {
        private CommunicationInterfaceComposite _readInterfaceComposite;
        private CommunicationInterfaceComposite _writeInterfaceComposite;

        public CommunicationInterfaceComposite ReadInterfaceComposite
        {
            get { return _readInterfaceComposite; }
        }

        public CommunicationInterfaceComposite WriteInterfaceComposite
        {
            get { return _writeInterfaceComposite; }
            set { _writeInterfaceComposite = value; }
        }

        public void Initialize(string type)
        {
            switch (type)
            {
                case "readInterface":
                    _readInterfaceComposite = new CommunicationInterfaceComposite("SPS");
                    _readInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("BEFEHL", 0, "Integer"));
                    _readInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("PROGRAMMTYP", 2, "Integer"));
                    break;
                case "writeInterface":
                    _writeInterfaceComposite = new CommunicationInterfaceComposite("PC");
                    _writeInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("ANTWORT", 0, "Integer"));
                    _writeInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("STATUS", 2, "Integer"));
                    _writeInterfaceComposite.Add(CommunicationInterfaceFactory.CreateVariable("FEHLERCODE", 4, "Integer"));
                    break;
                default: throw new InitializerException("Error: Wrong interface type.");
            }
        }

        public void MaintainConnection(PlcCommunicator communication)
        {
            if (_readInterfaceComposite != null && _writeInterfaceComposite != null &&
                communication.ConnectionStatus == 1)
            {
                _readInterfaceComposite.ReadValue(communication.ReadBytes);
                _writeInterfaceComposite.WriteValue(communication.WriteBytes);
            }
            else { throw new InitializerException("Error: Connection can not be maintained."); }
        }

        #region Auxiliaries

        public class InitializerException : ApplicationException
        { public InitializerException(string info) : base(info) { }}

        #endregion
    }
}
