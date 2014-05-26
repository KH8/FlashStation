using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.Output;
using _3880_80_FlashStation.PLC;
using _3880_80_FlashStation.Vector;
using _3880_80_FlashStation.Visual.Gui;
using VFlashTypeBankFile = _3880_80_FlashStation.Vector.VFlashTypeBankFile;

namespace _3880_80_FlashStation.Visual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Thread _communicationThread;

        private PlcCommunicator _plcCommunication;
        private CommunicationInterfaceHandler _communicationHandler;
        private VFlashHandler _vFlash;
        
        private readonly ObservableCollection<DataDisplayer.DisplayData> _readInterfaceCollection = new ObservableCollection<DataDisplayer.DisplayData>();
        private readonly ObservableCollection<DataDisplayer.DisplayData> _writeInterfaceCollection = new ObservableCollection<DataDisplayer.DisplayData>();
        private readonly ObservableCollection<VFlashDisplayProjectData> _vFlashProjectCollection = new ObservableCollection<VFlashDisplayProjectData>();

        public ObservableCollection<DataDisplayer.DisplayData> ReadInterfaceCollection
        { get { return _readInterfaceCollection; } }

        public ObservableCollection<DataDisplayer.DisplayData> WriteInterfaceCollection
        { get { return _writeInterfaceCollection; } }

        public ObservableCollection<VFlashDisplayProjectData> VFlashProjectCollection
        { get { return _vFlashProjectCollection; } }

        public MainWindow()
        {
            InitializeComponent();
            Logger.Log("Program Started");

            var registry = new Registry.Registry();
            registry.AddPlcCommunicator();

            try
            {
                InitializeInterface();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Initialization Failed");
                Logger.Log("Program Closed");
                Environment.Exit(0);
            }
            try
            {
                InitializePlcCommunication();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Initialization Failed");
                Logger.Log("Program Closed");
                Environment.Exit(0);
            }
            try
            {
                InitializeVFlash();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Initialization Failed");
                Logger.Log("Program Closed");
                Environment.Exit(0);
            }

            var gridGuiCommunicationStatus = new GuiCommunicationStatus(_plcCommunication, PlcStartUpConnection.Default);
            gridGuiCommunicationStatus.Initialize(1, 0, 0);
            ConnectionStatusGrid.Children.Add(gridGuiCommunicationStatus.GeneralGrid);

            var gridGuiCommunicationStatusBar = new GuiCommunicationStatusBar(_plcCommunication);
            gridGuiCommunicationStatusBar.Initialize(1, 0, 5);
            FooterGrid.Children.Add(gridGuiCommunicationStatusBar.GeneralGrid);

            var gridGuiPlcConfiguration = new GuiPlcConfiguration(_plcCommunication, _communicationHandler, PlcConfigurationFile.Default, CommunicationInterfacePath.Default);
            gridGuiPlcConfiguration.Initialize(1, 0, 0);
            ConfigurationGrid.Children.Add(gridGuiPlcConfiguration.GeneralGrid);

            var gridVFlash = new GuiVFlash(_vFlash);
            gridVFlash.Initialize(1, 0, 0);
            VFlashGrid.Children.Add(gridVFlash.GeneralGrid);

            var gridGuiVFlashStatusBar = new GuiVFlashStatusBar(_vFlash);
            gridGuiVFlashStatusBar.Initialize(1, 0, 20);
            FooterGrid.Children.Add(gridGuiVFlashStatusBar.GeneralGrid);

            var gridGuiOutputCreator = new GuiOutputCreator(_communicationHandler, OutputCreatorFile.Default);
            gridGuiOutputCreator.Initialize(1, 0, 0);
            OutputCreatorGrid.Children.Add(gridGuiOutputCreator.GeneralGrid);

            _communicationThread = new Thread(CommunicationHandler);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;
            _communicationThread.Start();

            if (!PlcStartUpConnection.Default.ConnectAtStartUp || _plcCommunication.ConnectionStatus == 1) return;
            gridGuiCommunicationStatus.ConnectionButtonClick(null, new RoutedEventArgs());
            Logger.Log("Connected with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress + " at start up");//*/
        }

        #region Init Methods

        internal void InitializeInterface()
        {
            Logger.Log("Initialization of the interface");

            _communicationHandler = new CommunicationInterfaceHandler(CommunicationInterfacePath.Default);

            if (CommunicationInterfacePath.Default.ConfigurationStatus == 1)
            {
                try {_communicationHandler.Initialize(); }
                catch (Exception)
                {
                    CommunicationInterfacePath.Default.ConfigurationStatus = 0;
                    CommunicationInterfacePath.Default.Save();
                    MessageBox.Show("Interface initialization failed,\nRestart application", "Initialization Failed");
                    Logger.Log("PLC Communication interface initialization failed");
                    Environment.Exit(0);
                }
            }
            else
            {
                CommunicationInterfacePath.Default.Path = "DataAquisition\\DB1000.csv";
                CommunicationInterfacePath.Default.ConfigurationStatus = 1;
                CommunicationInterfacePath.Default.Save();
                
                try { _communicationHandler.Initialize(); }
                catch (Exception)
                {
                    CommunicationInterfacePath.Default.ConfigurationStatus = 0;
                    CommunicationInterfacePath.Default.Save();
                    MessageBox.Show("Interface initialization failed,\nRestart application", "Initialization Failed");
                    Logger.Log("PLC Communication interface initialization failed");
                    Environment.Exit(0);
                }
                Logger.Log("PLC Communication interface initialized with file: " + CommunicationInterfacePath.Default.Path);
            }
            Logger.Log("Interface Initialized");
        }

        internal void InitializePlcCommunication()
        {
            Logger.Log("Initialization of PLC communication");
            _plcCommunication = new PlcCommunicator();
            Logger.Log("PLC communication initialized");
        }

        internal void InitializeVFlash()
        {
            Logger.Log("Initialization of the vFlash");

            VFlashTab.IsEnabled = true;
            VFlashProjectsTab.IsEnabled = true;
            
            try { _vFlash = new VFlashHandler(_communicationHandler.ReadInterfaceComposite, _communicationHandler.WriteInterfaceComposite, 1); }
            catch (Exception)
            {
                MessageBox.Show("VFlash initialization failed", "VFlash Failed");
                Environment.Exit(0);
            }

            VFlashTypeConverter.StringsToVFlashChannels(VFlashTypeBankFile.Default.TypeBank, _vFlash.VFlashTypeBank);
            foreach (var type in _vFlash.VFlashTypeBank.Children.Cast<VFlashTypeComponent>())
            {
                _vFlashProjectCollection.Add(new VFlashDisplayProjectData
                {
                    Type = type.Type.ToString(CultureInfo.InvariantCulture),
                    Version = type.Version,
                    Path = type.Path
                });
            }
            Logger.Log("vFlash Initialized");
        }

        #endregion

        #region Thread Methods

        private void CommunicationHandler()
        {
            while (_communicationThread.IsAlive)
            {
                if (_plcCommunication != null && _plcCommunication.ConnectionStatus == 1)
                {
                    _communicationHandler.MaintainConnection(_plcCommunication);
                }
                if (_plcCommunication != null)
                {
                    DataDisplayer.Display(_readInterfaceCollection, _writeInterfaceCollection, _plcCommunication, _communicationHandler);
                }
                Thread.Sleep(21);
            }
        }

        #endregion

        #region Buttons

        private void CloseApp(object sender, CancelEventArgs cancelEventArgs)
        {
            if(_vFlash != null) _vFlash.Deinitialize();
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        private void TypeCreation(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".vflashpack", Filter = "Flash Path (.vflashpack)|*.vflashpack" };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                _vFlash.VFlashTypeBank.Add(new VFlashTypeComponent(Convert.ToUInt16(TypeNumberBox.Text), TypeVersionBox.Text, dlg.FileName));

                VFlashTypeBankFile.Default.TypeBank = VFlashTypeConverter.VFlashTypesToStrings(_vFlash.VFlashTypeBank.Children);
                VFlashTypeBankFile.Default.Save();

                _vFlashProjectCollection.Clear();
                foreach (var vFlashType in _vFlash.VFlashTypeBank.Children)
                {
                    var type = (VFlashTypeComponent) vFlashType;
                    _vFlashProjectCollection.Add(new VFlashDisplayProjectData
                    {
                        Type = type.Type.ToString(CultureInfo.InvariantCulture),
                        Version = type.Version,
                        Path = type.Path
                    });
                }
            }
        }

        #endregion

        #region GUI Parameters Handleing

        private void UpdateLog(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            LogListBox.Dispatcher.BeginInvoke((new Action(() => Logger.DumpLog(LogListBox))));
        }

        private void VFlashProjectbankListViewSelection(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            var projectdata = (VFlashDisplayProjectData)listView.SelectedItem;
            if (projectdata != null) TypeNumberBox.Text = projectdata.Type;
        }

        #endregion

    }
}
