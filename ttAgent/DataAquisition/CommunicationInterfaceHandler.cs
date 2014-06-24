using System;
using System.Windows;
using _ttAgent.Log;
using _ttAgent.MainRegistry;
using _ttAgent.PLC;

namespace _ttAgent.DataAquisition
{
    public class CommunicationInterfaceHandler : RegistryComponent
    {
        private CommunicationInterfaceComposite _readInterfaceComposite;
        private CommunicationInterfaceComposite _writeInterfaceComposite;

        public CommunicationInterfaceHandler(uint id, string name, CommunicationInterfacePath pathFile) : base(id, name)
        {
            PathFile = pathFile;
            Logger.Log("ID: " + Header.Id + " Communication interface component created");
        }

        public CommunicationInterfaceComposite ReadInterfaceComposite
        {
            get { return _readInterfaceComposite; }
        }

        public CommunicationInterfaceComposite WriteInterfaceComposite
        {
            get { return _writeInterfaceComposite; }
        }

        public CommunicationInterfacePath PathFile { get; set; }

        public void InitializeInterface()
        {
            if (PathFile.ConfigurationStatus[Header.Id] == 1)
            {
                try { Initialize(); }
                catch (Exception)
                {
                    PathFile.ConfigurationStatus[Header.Id] = 0;
                    PathFile.Save();
                    MessageBox.Show("ID: " + Header.Id + " Interface initialization failed\nRestart application", "Initialization Failed");
                    Logger.Log("ID: " + Header.Id + " Communication interface initialization failed");
                    Environment.Exit(0);
                }
            }
            else
            {
                PathFile.Path[Header.Id] = "DataAquisition\\DB1000_NEW.csv";
                PathFile.ConfigurationStatus[Header.Id] = 1;
                PathFile.Save();

                try { Initialize(); }
                catch (Exception)
                {
                    PathFile.ConfigurationStatus[Header.Id] = 0;
                    PathFile.Save();
                    MessageBox.Show("ID: " + Header.Id + " Interface initialization failed\nRestart application", "Initialization Failed");
                    Logger.Log("ID: " + Header.Id + " Communication interface initialization failed");
                    Environment.Exit(0);
                }
                Logger.Log("ID: " + Header.Id + " Communication interface initialized with file: " + PathFile.Path[Header.Id]);
            }
        }

        internal void Initialize()
        {
            _readInterfaceComposite = CommunicationInterfaceBuilder.InitializeInterface(Header.Id, CommunicationInterfaceComponent.InterfaceType.ReadInterface, PathFile);
            _writeInterfaceComposite = CommunicationInterfaceBuilder.InitializeInterface(Header.Id, CommunicationInterfaceComponent.InterfaceType.WriteInterface, PathFile);
        }

        public void MaintainConnection(PlcCommunicator communication)
        {
            if (communication.ConnectionStatus == 1)
            {
                if(_readInterfaceComposite != null) _readInterfaceComposite.ReadValue(communication.ReadBytes);
                if (_writeInterfaceComposite != null) _writeInterfaceComposite.WriteValue(communication.WriteBytes);
            }
            else { throw new InitializerException("Error: ID: " + Header.Id + " Connection can not be maintained."); }
        }

        #region Auxiliaries

        public class InitializerException : ApplicationException
        { public InitializerException(string info) : base(info) { }}

        #endregion
    }
}
