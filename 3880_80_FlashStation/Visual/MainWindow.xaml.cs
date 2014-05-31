using System;
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

        public MainWindow()
        {
            InitializeComponent();
            Logger.Log("Program Started");

            _registry = new Registry();

            _communicationThread = new Thread(CommunicationHandler);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;
            _communicationThread.Start();

            UpdateTreeView();
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

            var newtabItem = new TabItem {Header = "PLC__" + newId};
            ConnectionTabControl.Items.Add(newtabItem);
            ConnectionTabControl.SelectedItem = newtabItem;

            var newScrollViewer = new ScrollViewer();
            newtabItem.Content = newScrollViewer;

            var newGrid = new Grid();
            newScrollViewer.Content = newGrid;

            var gridGuiCommunicationStatus = _registry.PlcGuiCommunicationStatuses[newId];
            gridGuiCommunicationStatus.Initialize(0, 0, newGrid);

            var gridGuiCommunicationStatusBar = _registry.PlcGuiCommunicationStatusBars[newId];
            gridGuiCommunicationStatusBar.Initialize(0, 5, FooterGrid);

            var gridGuiPlcConfiguration = _registry.PlcGuiConfigurations[newId];
            gridGuiPlcConfiguration.Initialize(0, 260, newGrid);

            UpdateTreeView();
        }

        private void AddInterface(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddCommunicationInterface();
            _registry.CommunicationInterfaceHandlers[newId].InitializeInterface();

            var newtabItem = new TabItem { Header = "INT__" + newId };
            ConnectionTabControl.Items.Add(newtabItem);
            ConnectionTabControl.SelectedItem = newtabItem;

            var newScrollViewer = new ScrollViewer();
            newtabItem.Content = newScrollViewer;

            var newGrid = new Grid();
            newScrollViewer.Content = newGrid;

            var gridGuiCommunicationInterfaceConfiguration = _registry.GuiComInterfacemunicationConfigurations[newId];
            gridGuiCommunicationInterfaceConfiguration.Initialize(0, 0, newGrid);

            newtabItem = new TabItem { Header = "INT__" + newId + " Online" };
            MainTabControl.Items.Add(newtabItem);
            MainTabControl.SelectedItem = newtabItem;

            newGrid = new Grid();
            newtabItem.Content = newGrid;

            newGrid.Height = MainTabControl.Height - 32;
            newGrid.Width = MainTabControl.Width - 10;

            var gridGuiCommunicationInterfaceOnline = _registry.GuiCommunicationInterfaceOnlines[newId];
            gridGuiCommunicationInterfaceOnline.Initialize(0, 0, newGrid);

            UpdateTreeView();
        }

        private void AddOutputFileHandlerChannel(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddOutputWriter();

            var newtabItem = new TabItem { Header = "OUTPUT__" + newId };
            OutputTabControl.Items.Add(newtabItem);
            OutputTabControl.SelectedItem = newtabItem;

            var newGrid = new Grid();
            newtabItem.Content = newGrid;

            var gridGuiOutputCreator = _registry.GuiOutputCreators[newId];
            gridGuiOutputCreator.Initialize(0, 0, newGrid);

            UpdateTreeView();
        }

        private void AddVFlashBank(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddVFlashBank();

            var newtabItem = new TabItem { Header = "VFLASH__BANK__" + newId };
            OutputTabControl.Items.Add(newtabItem);
            OutputTabControl.SelectedItem = newtabItem;

            var newGrid = new Grid();
            newtabItem.Content = newGrid;

            newGrid.Height = OutputTabControl.Height - 32;
            newGrid.Width = OutputTabControl.Width - 10;

            var gridGuiVFlashPathBank = _registry.GuiVFlashPathBanks[newId];
            gridGuiVFlashPathBank.Initialize(0, 0, newGrid);

            UpdateTreeView();
        }

        private void AddVFlashChannel(object sender, RoutedEventArgs e)
        {
            var newId = _registry.AddVFlashChannel();
            _registry.VFlashHandlers[newId].InitializeVFlash();
            _registry.VFlashHandlers[newId].VFlashTypeBank = _registry.VFlashTypeBanks[newId];

            var newtabItem = new TabItem { Header = "VFLASH__" + newId };
            OutputTabControl.Items.Add(newtabItem);
            OutputTabControl.SelectedItem = newtabItem;

            var newGrid = new Grid();
            newtabItem.Content = newGrid;

            var gridVFlash = _registry.GuiVFlashes[newId];
            gridVFlash.Initialize(0, 0, newGrid);

            var gridGuiVFlashStatusBar = _registry.GuiVFlashStatusBars[newId];
            gridGuiVFlashStatusBar.Initialize(0, 20, FooterGrid);

            UpdateTreeView();
        }

        #endregion

        #region GUI Parameters Handleing

        private void UpdateLog(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            LogListBox.Dispatcher.BeginInvoke((new Action(() => Logger.DumpLog(LogListBox))));
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainTabControl.Height = ActualHeight - 372;
            MainTabControl.Width = ActualWidth - 374;

            ConnectionTabControl.Height = ActualHeight - 120;
            ConnectionTabControl.Width = 350;

            OutputTabControl.Height = 250;
            OutputTabControl.Width = ActualWidth - 374;

            LogListBox.Height = MainTabControl.Height - 32;
            LogListBox.Width = MainTabControl.Width - 10;

            AboutGrid.Height = MainTabControl.Height - 32;
            AboutGrid.Width = MainTabControl.Width - 10;

            ComponentManagerTreeView.Height = MainTabControl.Height - 32;
            ComponentManagerTreeView.Width = MainTabControl.Width - 10;

            foreach (var gui in _registry.GuiCommunicationInterfaceOnlines) { gui.Value.UpdateSizes(MainTabControl.Height - 32, MainTabControl.Width - 10); }
            foreach (var gui in _registry.GuiVFlashPathBanks) { gui.Value.UpdateSizes(OutputTabControl.Height - 32, OutputTabControl.Width - 10); }
        }

        private void UpdateTreeView()
        {
            ComponentManagerTreeView.Items.Clear();

            var mainHeader = new TreeViewItem { Header = "Components" };
            ComponentManagerTreeView.Items.Add(mainHeader);

            var newHeader= new TreeViewItem {Header = "PLC Connections"};
            foreach (var record in _registry.PlcCommunicators)
            { newHeader.Items.Add(new TreeViewItem { Header = "PLC_" + record.Key }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "Communication Interfaces" };
            foreach (var record in _registry.CommunicationInterfaceHandlers)
            { newHeader.Items.Add(new TreeViewItem { Header = "INT_" + record.Key }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "Output Handlers" };
            foreach (var record in _registry.OutputWriters)
            { newHeader.Items.Add(new TreeViewItem { Header = "OUT_" + record.Key }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "vFlashBank" };
            foreach (var record in _registry.VFlashTypeBanks)
            { newHeader.Items.Add(new TreeViewItem { Header = "VFLASH_BANK_" + record.Key }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "vFlash Channels" };
            foreach (var record in _registry.GuiVFlashes)
            { newHeader.Items.Add(new TreeViewItem { Header = "VFLASH_" + record.Key }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }
            
        }

        #endregion

    }
}
