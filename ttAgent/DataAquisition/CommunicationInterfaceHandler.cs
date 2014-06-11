using System;
using System.Windows;
using _ttAgent.Log;
using _ttAgent.PLC;

namespace _ttAgent.DataAquisition
{
    public class CommunicationInterfaceHandler
    {
        private readonly uint _id;
        private readonly CommunicationInterfacePath _pathFile;
        private CommunicationInterfaceComposite _readInterfaceComposite;
        private CommunicationInterfaceComposite _writeInterfaceComposite;

        public CommunicationInterfaceHandler(uint id, CommunicationInterfacePath pathFile)
        {
            _id = id;
            _pathFile = pathFile;
            Logger.Log("ID: " + _id + " Communication interface component created");
        }

        public CommunicationInterfaceComposite ReadInterfaceComposite
        {
            get { return _readInterfaceComposite; }
        }

        public CommunicationInterfaceComposite WriteInterfaceComposite
        {
            get { return _writeInterfaceComposite; }
            set { _writeInterfaceComposite = value; }
        }

        public void InitializeInterface()
        {
            if (_pathFile.ConfigurationStatus[_id] == 1)
            {
                try { Initialize(); }
                catch (Exception)
                {
                    CommunicationInterfacePath.Default.ConfigurationStatus[_id] = 0;
                    CommunicationInterfacePath.Default.Save();
                    MessageBox.Show("ID: " + _id + " Interface initialization failed\nRestart application", "Initialization Failed");
                    Logger.Log("ID: " + _id + " Communication interface initialization failed");
                    Environment.Exit(0);
                }
            }
            else
            {
                _pathFile.Path[_id] = "DataAquisition\\DB1000_NEW.csv";
                _pathFile.ConfigurationStatus[_id] = 1;
                _pathFile.Save();

                try { Initialize(); }
                catch (Exception)
                {
                    _pathFile.ConfigurationStatus[_id] = 0;
                    _pathFile.Save();
                    MessageBox.Show("ID: " + _id + " Interface initialization failed\nRestart application", "Initialization Failed");
                    Logger.Log("ID: " + _id + " Communication interface initialization failed");
                    Environment.Exit(0);
                }
                Logger.Log("ID: " + _id + " Communication interface initialized with file: " + _pathFile.Path[_id]);
            }
        }

        internal void Initialize()
        {
            _readInterfaceComposite = CommunicationInterfaceBuilder.InitializeInterface(_id, CommunicationInterfaceComponent.InterfaceType.ReadInterface, _pathFile);
            _writeInterfaceComposite = CommunicationInterfaceBuilder.InitializeInterface(_id, CommunicationInterfaceComponent.InterfaceType.WriteInterface, _pathFile);
        }

        public void MaintainConnection(PlcCommunicator communication)
        {
            if (communication.ConnectionStatus == 1)
            {
                if(_readInterfaceComposite != null) _readInterfaceComposite.ReadValue(communication.ReadBytes);
                if (_writeInterfaceComposite != null) _writeInterfaceComposite.WriteValue(communication.WriteBytes);
            }
            else { throw new InitializerException("Error: ID: " + _id + " Connection can not be maintained."); }
        }

        #region Auxiliaries

        public class InitializerException : ApplicationException
        { public InitializerException(string info) : base(info) { }}

        #endregion
    }
}
