using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.MainRegistry;

namespace _3880_80_FlashStation.Visual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Thread _communicationThread;
        private readonly Registry _registry;

        private readonly ObservableCollection<DataDisplayer.DisplayData> _readInterfaceCollection = new ObservableCollection<DataDisplayer.DisplayData>();
        private readonly ObservableCollection<DataDisplayer.DisplayData> _writeInterfaceCollection = new ObservableCollection<DataDisplayer.DisplayData>();

        public ObservableCollection<DataDisplayer.DisplayData> ReadInterfaceCollection
        { get { return _readInterfaceCollection; } }

        public ObservableCollection<DataDisplayer.DisplayData> WriteInterfaceCollection
        { get { return _writeInterfaceCollection; } }

        public MainWindow()
        {
            InitializeComponent();
            Logger.Log("Program Started");

            _registry = new Registry();

            _communicationThread = new Thread(CommunicationHandler);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;
            _communicationThread.Start();
        }

        #region Thread Methods

        private void CommunicationHandler()
        {
            while (_communicationThread.IsAlive)
            {
                foreach (var plcCommunicator in _registry.PlcCommunicators)
                {
                    if (_registry.CommunicationInterfaceHandlers.ContainsKey(plcCommunicator.Key))
                    {
                        if (plcCommunicator.Value.ConnectionStatus == 1) _registry.CommunicationInterfaceHandlers[plcCommunicator.Key].MaintainConnection(plcCommunicator.Value);
                        DataDisplayer.Display(_readInterfaceCollection, _writeInterfaceCollection, plcCommunicator.Value, _registry.CommunicationInterfaceHandlers[plcCommunicator.Key]);
                    }
                }
                Thread.Sleep(21);
            }
        }

        #endregion

        #region Buttons

        private void CloseApp(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (var vFlashHandler in _registry.VFlashHandlers) { vFlashHandler.Value.Deinitialize(); }
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        private void CloseApp(object sender, CancelEventArgs e)
        {
            foreach (var vFlashHandler in _registry.VFlashHandlers) { vFlashHandler.Value.Deinitialize(); }
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        private void AddConnection(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddPlcCommunicator();
            _registry.PlcCommunicators[newId].InitializeConnection();

            var gridGuiCommunicationStatus = _registry.PlcGuiCommunicationStatuses[newId];
            gridGuiCommunicationStatus.Initialize(0, 0);
            ConnectionStatusGrid.Children.Add(gridGuiCommunicationStatus.GeneralGrid);

            var gridGuiCommunicationStatusBar = _registry.PlcGuiCommunicationStatusBars[newId];
            gridGuiCommunicationStatusBar.Initialize(0, 5);
            FooterGrid.Children.Add(gridGuiCommunicationStatusBar.GeneralGrid);

            var gridGuiPlcConfiguration = _registry.PlcGuiConfigurations[newId];
            gridGuiPlcConfiguration.Initialize(0, 0);
            ConfigurationGrid.Children.Add(gridGuiPlcConfiguration.GeneralGrid);
        }

        private void AddInterface(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddCommunicationInterface();
            _registry.CommunicationInterfaceHandlers[newId].InitializeInterface();

            var gridGuiCommunicationInterfaceConfiguration = _registry.GuiComInterfacemunicationConfigurations[newId];
            gridGuiCommunicationInterfaceConfiguration.Initialize(0, 0);
            ConfigurationGrid.Children.Add(gridGuiCommunicationInterfaceConfiguration.GeneralGrid);
        }

        private void AddVFlashBank(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddVFlashBank();

            var gridGuiVFlashPathBank = _registry.GuiVFlashPathBanks[newId];
            gridGuiVFlashPathBank.Initialize(0, 0);
            VFlashProjectsGrid.Children.Add(gridGuiVFlashPathBank.GeneralGrid);
        }

        private void AddOutputFileHandlerChannel(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddOutputWriter();

            var gridGuiOutputCreator = _registry.GuiOutputCreators[newId];
            gridGuiOutputCreator.Initialize(0, 0);
            OutputCreatorGrid.Children.Add(gridGuiOutputCreator.GeneralGrid);
        }

        private void AddVFlashChannel(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddVFlashChannel();
            _registry.VFlashHandlers[newId].InitializeVFlash();
            _registry.VFlashHandlers[newId].VFlashTypeBank = _registry.VFlashTypeBanks[newId];

            var gridVFlash = _registry.GuiVFlashes[newId];
            gridVFlash.Initialize(0, 0);
            VFlashGrid.Children.Add(gridVFlash.GeneralGrid);

            var gridGuiVFlashStatusBar = _registry.GuiVFlashStatusBars[newId];
            gridGuiVFlashStatusBar.Initialize(0, 20);
            FooterGrid.Children.Add(gridGuiVFlashStatusBar.GeneralGrid);
        }

        #endregion

        #region GUI Parameters Handleing

        private void UpdateLog(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            LogListBox.Dispatcher.BeginInvoke((new Action(() => Logger.DumpLog(LogListBox))));
        }

        #endregion

    }
}
