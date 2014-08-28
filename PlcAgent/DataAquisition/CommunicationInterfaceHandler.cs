using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.PLC;
using _PlcAgent.Visual;

namespace _PlcAgent.DataAquisition
{
    public class CommunicationInterfaceHandler : Module
    {
        #region Variables

        private CommunicationInterfaceComposite _readInterfaceComposite;
        private CommunicationInterfaceComposite _writeInterfaceComposite;

        private readonly ObservableCollection<DisplayDataBuilder.DisplayData> _readInterfaceCollection =
            new ObservableCollection<DisplayDataBuilder.DisplayData>();

        private readonly ObservableCollection<DisplayDataBuilder.DisplayData> _writeInterfaceCollection =
            new ObservableCollection<DisplayDataBuilder.DisplayData>();

        private readonly Thread _communicationThread;

        #endregion


        #region Properties

        public CommunicationInterfaceComposite ReadInterfaceComposite
        { get { return _readInterfaceComposite; } }
        public CommunicationInterfaceComposite WriteInterfaceComposite
        { get { return _writeInterfaceComposite; } }

        public PlcCommunicator PlcCommunicator { get; set; }
        public CommunicationInterfacePath PathFile { get; set; }

        public ObservableCollection<DisplayDataBuilder.DisplayData> ReadInterfaceCollection { get { return _readInterfaceCollection; } }
        public ObservableCollection<DisplayDataBuilder.DisplayData> WriteInterfaceCollection { get { return _writeInterfaceCollection; }}

        public delegate void InterfaceUpdatedDelegate();
        public InterfaceUpdatedDelegate OnInterfaceUpdatedDelegate;

        #endregion


        #region Constructors

        public CommunicationInterfaceHandler(uint id, string name, PlcCommunicator plcCommunicator,
            CommunicationInterfacePath pathFile)
            : base(id, name)
        {
            PlcCommunicator = plcCommunicator;
            PathFile = pathFile;

            _communicationThread = new Thread(CommunicationHandler);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;

            Logger.Log("ID: " + Header.Id + " Communication interface component created");
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            _readInterfaceComposite = CommunicationInterfaceBuilder.InitializeInterface(Header.Id,
                CommunicationInterfaceComponent.InterfaceType.ReadInterface, PathFile);
            _writeInterfaceComposite = CommunicationInterfaceBuilder.InitializeInterface(Header.Id,
                CommunicationInterfaceComponent.InterfaceType.WriteInterface, PathFile);
            DisplayDataBuilder.Build(_readInterfaceCollection, _writeInterfaceCollection, this);

            _communicationThread.Start();

            Logger.Log("ID: " + Header.Id + " Communication interface Initialized");
        }

        public override void Deinitialize()
        {
            _communicationThread.Abort();

            Logger.Log("ID: " + Header.Id + " Communication interface Deinitialized");
        }

        public void InitializeInterface()
        {
            if (PathFile.ConfigurationStatus[Header.Id] == 1)
            {
                try
                {
                    Initialize();
                }
                catch (Exception)
                {
                    PathFile.ConfigurationStatus[Header.Id] = 0;
                    PathFile.Save();
                    MessageBox.Show("ID: " + Header.Id + " Interface initialization failed\nRestart application",
                        "Initialization Failed");
                    Logger.Log("ID: " + Header.Id + " Communication interface initialization failed");
                    Environment.Exit(0);
                }
            }
            else
            {
                PathFile.Path[Header.Id] = "DataAquisition\\DB1000_NEW.csv";
                PathFile.ConfigurationStatus[Header.Id] = 1;
                PathFile.Save();

                try
                {
                    Initialize();
                }
                catch (Exception)
                {
                    PathFile.ConfigurationStatus[Header.Id] = 0;
                    PathFile.Save();
                    MessageBox.Show("ID: " + Header.Id + " Interface initialization failed\nRestart application",
                        "Initialization Failed");
                    Logger.Log("ID: " + Header.Id + " Communication interface initialization failed");
                    Environment.Exit(0);
                }
                Logger.Log("ID: " + Header.Id + " Communication interface initialized with file: " +
                           PathFile.Path[Header.Id]);
            }
        }

        public void UpdateObservableCollections()
        {
            foreach (var displayDataComponent in _readInterfaceCollection) { displayDataComponent.Update(); }
            foreach (var displayDataComponent in _writeInterfaceCollection) { displayDataComponent.Update();}
        }

        #endregion


        #region Thread Methods

        private void CommunicationHandler()
        {
            while (_communicationThread.IsAlive)
            {
                MaintainConnection();
                Thread.Sleep(10);
            }
        }

        private void MaintainConnection()
        {
            if (OnInterfaceUpdatedDelegate != null) OnInterfaceUpdatedDelegate();

            if (PlcCommunicator.ConnectionStatus != 1) return;
            if (_readInterfaceComposite != null) _readInterfaceComposite.ReadValue(PlcCommunicator.ReadBytes);
            if (_writeInterfaceComposite != null) _writeInterfaceComposite.WriteValue(PlcCommunicator.WriteBytes);
        }

        #endregion


        #region Auxiliaries

        public class InitializerException : ApplicationException
        { public InitializerException(string info) : base(info) { }}

        #endregion
    }
}
