using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.MainRegistry;
using _3880_80_FlashStation.Vector;

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
            _registry.Initialize();

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
            if (newId == 0) return;

            _registry.PlcCommunicators[newId].InitializeConnection();

            UpdateGui();
            UpdateTreeView();
        }

        private void AddInterface(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem { Header = "PLC Connections" };
            foreach (var record in _registry.PlcCommunicators)
            { newHeader.Items.Add(new TreeViewItem
            {
                Header = "PLC_" + record.Key,
                AlternationCount = (int)record.Key
            }); }

            var window = new ComponentCreationWindow("Select a PLC connection to be assigned with a new Communication Interface", newHeader, AssignInterface);
            window.Show();
        }

        private void AssignInterface(uint plcConnectionId)
        {
            var newId = _registry.AddCommunicationInterface(plcConnectionId);
            if (newId == 0) return;

            _registry.CommunicationInterfaceHandlers[newId].InitializeInterface();

            UpdateGui();
            UpdateTreeView();
        }

        private void AddOutputFileHandler(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem { Header = "Communication Interfaces" };
            foreach (var record in _registry.CommunicationInterfaceHandlers)
            {
                newHeader.Items.Add(new TreeViewItem
                {
                    Header = "INT_" + record.Key,
                    AlternationCount = (int)record.Key
                });
            }

            var window = new ComponentCreationWindow("Select a Communication Interface to be assigned with a new Output Handler", newHeader, AssignOutputFileHandler);
            window.Show();
        }

        private void AssignOutputFileHandler(uint communicationInterfaceId)
        {
            var newId = _registry.AddOutputWriter(communicationInterfaceId);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AddVFlashBank(object sender, RoutedEventArgs e)
        {
            AssignVFlashBank();
        }

        private void AssignVFlashBank()
        {
            var newId = _registry.AddVFlashBank();
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AddVFlashChannel(object sender, RoutedEventArgs e)
        {
            var newHeaderCommunicationInterface = new TreeViewItem { Header = "Communication Interfaces" };
            foreach (var record in _registry.CommunicationInterfaceHandlers)
            {
                newHeaderCommunicationInterface.Items.Add(new TreeViewItem
                {
                    Header = "INT_" + record.Key,
                    AlternationCount = (int)record.Key
                });
            }

            var newHeaderVFlashBank = new TreeViewItem { Header = "vFlash Banks" };
            foreach (var record in _registry.VFlashTypeBanks)
            {
                newHeaderVFlashBank.Items.Add(new TreeViewItem
                {
                    Header = "VFLASH_BANK_" + record.Key,
                    AlternationCount = (int)record.Key
                });
            }

            var window = new ComponentCreationWindow("Select components to be assigned with a new vFlash Channel", newHeaderCommunicationInterface, newHeaderVFlashBank, AssignVFlashChannel);
            window.Show();
        }

        private void AssignVFlashChannel(uint communicationInterfaceId, uint vFlashBankId)
        {
            var newId = _registry.AddVFlashChannel(communicationInterfaceId, vFlashBankId);
            if (newId == 0) return;

            _registry.VFlashHandlers[newId].InitializeVFlash();
            _registry.VFlashHandlers[newId].VFlashTypeBank = _registry.VFlashTypeBanks[vFlashBankId];

            UpdateGui();
            UpdateTreeView();
        }



        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = AbouTabItem;
        }

        private void ShowComponentManager(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = ComponentManagerTabItem;
        }

        private void ShowLog(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = LogTabItem;
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

        private void UpdateGui()
        {
            MainTabControl.Items.Clear();
            OutputTabControl.Items.Clear();
            ConnectionTabControl.Items.Clear();
            FooterGrid.Children.Clear();

            var labelConnectionTabControl = new Label { Content = "Use EDIT menu to create new connections and interfaces." };
            ConnectionTabControl.Items.Add(labelConnectionTabControl);
            ConnectionTabControl.SelectedItem = labelConnectionTabControl;

            var labelOutputTabControl = new Label { Content = "Use EDIT menu to create new output handlers." };
            OutputTabControl.Items.Add(labelOutputTabControl);
            OutputTabControl.SelectedItem = labelOutputTabControl;

            foreach (var record in _registry.PlcGuiCommunicationStatuses)
            {
                var newtabItem = new TabItem { Header = "PLC__" + record.Key };
                ConnectionTabControl.Items.Add(newtabItem);
                ConnectionTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer();
                newtabItem.Content = newScrollViewer;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridGuiCommunicationStatus = _registry.PlcGuiCommunicationStatuses[record.Key];
                gridGuiCommunicationStatus.Initialize(0, 0, newGrid);

                var gridGuiPlcConfiguration = _registry.PlcGuiConfigurations[record.Key];
                gridGuiPlcConfiguration.Initialize(0, 260, newGrid);
            }

            foreach (var record in _registry.PlcGuiCommunicationStatusBars)
            {
                var gridGuiCommunicationStatusBar = _registry.PlcGuiCommunicationStatusBars[record.Key];
                gridGuiCommunicationStatusBar.Initialize(0, 5, FooterGrid);
            }

            foreach (var record in _registry.CommunicationInterfaceHandlers)
            {
                var newtabItem = new TabItem { Header = "INT__" + record.Key };
                ConnectionTabControl.Items.Add(newtabItem);
                ConnectionTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer();
                newtabItem.Content = newScrollViewer;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridGuiCommunicationInterfaceConfiguration = _registry.GuiComInterfacemunicationConfigurations[record.Key];
                gridGuiCommunicationInterfaceConfiguration.Initialize(0, 0, newGrid);

                newtabItem = new TabItem { Header = "INT__" + record.Key + " Online" };
                MainTabControl.Items.Add(newtabItem);
                MainTabControl.SelectedItem = newtabItem;

                newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = MainTabControl.Height - 32;
                newGrid.Width = MainTabControl.Width - 10;

                var gridGuiCommunicationInterfaceOnline = _registry.GuiCommunicationInterfaceOnlines[record.Key];
                gridGuiCommunicationInterfaceOnline.Initialize(0, 0, newGrid);
            }

            foreach (var registry in _registry.OutputWriters)
            {
                var newtabItem = new TabItem { Header = "OUTPUT__" + registry.Key };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                var gridGuiOutputCreator = _registry.GuiOutputCreators[registry.Key];
                gridGuiOutputCreator.Initialize(0, 0, newGrid);
            }

            foreach (var registry in _registry.VFlashTypeBanks)
            {
                var newtabItem = new TabItem { Header = "VFLASH__BANK__" + registry.Key };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = OutputTabControl.Height - 32;
                newGrid.Width = OutputTabControl.Width - 10;

                var gridGuiVFlashPathBank = _registry.GuiVFlashPathBanks[registry.Key];
                gridGuiVFlashPathBank.Initialize(0, 0, newGrid);
            }

            foreach (var registry in _registry.VFlashHandlers)
            {
                
                var newtabItem = new TabItem { Header = "VFLASH__" + registry.Key };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                var gridVFlash = _registry.GuiVFlashes[registry.Key];
                gridVFlash.Initialize(0, 0, newGrid);

                var gridGuiVFlashStatusBar = _registry.GuiVFlashStatusBars[registry.Key];
                gridGuiVFlashStatusBar.Initialize(0, 20, FooterGrid);
            }

            MainTabControl.Items.Add(ComponentManagerTabItem);
            if (MainTabControl.SelectedItem == null) MainTabControl.SelectedItem = ComponentManagerTabItem;
            MainTabControl.Items.Add(AbouTabItem);
            MainTabControl.Items.Add(LogTabItem);

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
            { newHeader.Items.Add(new TreeViewItem { Header = "INT_" + record.Key + " ; assigned components: " + "PLC_" + _registry.CommunicationInterfaceHandlersAssignemenTuples[record.Key].Item1}); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "Output Handlers" };
            foreach (var record in _registry.OutputWriters)
            { newHeader.Items.Add(new TreeViewItem { Header = "OUT_" + record.Key + " ; assigned components: " + "INT_" + _registry.OutputWritersAssignemenTuples[record.Key].Item2 }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "vFlash Banks" };
            foreach (var record in _registry.VFlashTypeBanks)
            { newHeader.Items.Add(new TreeViewItem { Header = "VFLASH_BANK_" + record.Key }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "vFlash Channels" };
            foreach (var record in _registry.GuiVFlashes)
            { newHeader.Items.Add(new TreeViewItem { Header = "VFLASH_" + record.Key + " ; assigned components: " + "INT_" + _registry.VFlashHandlersAssignemenTuples[record.Key].Item2 + " ; " + "VFLASH_BANK_" + _registry.VFlashHandlersAssignemenTuples[record.Key].Item3 }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }
            
        }

        #endregion
    }
}
