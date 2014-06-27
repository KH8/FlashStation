using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using _ttAgent.DataAquisition;
using _ttAgent.Log;
using _ttAgent.MainRegistry;
using _ttAgent.Output;
using _ttAgent.PLC;
using _ttAgent.Project;
using _ttAgent.Vector;
using _ttAgent.Visual.Gui;
using Registry = _ttAgent.MainRegistry.Registry;

namespace _ttAgent.Visual
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

            Logger.Log("Registry initialization");
            _registry = new Registry();
            _registry.Initialize();

            _communicationThread = new Thread(CommunicationHandler);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;
            _communicationThread.Start();

            UpdateGui();
            UpdateTreeView();
        }

        #region Thread Methods

        private void CommunicationHandler()
        {
            while (_communicationThread.IsAlive)
            {
                foreach (var communicationInterfaceHandler in _registry.CommunicationInterfaceHandlers.Cast<CommunicationInterfaceHandler>())
                {
                    communicationInterfaceHandler.MaintainConnection();
                }
                Thread.Sleep(21);
            }
        }

        #endregion

        #region Buttons

        private void CloseApp(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (var vFlashHandler in _registry.VFlashHandlers.Cast<VFlashHandler>()) { vFlashHandler.Deinitialize(); }
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        private void CloseApp(object sender, CancelEventArgs e)
        {
            foreach (var vFlashHandler in _registry.VFlashHandlers.Cast<VFlashHandler>()) { vFlashHandler.Deinitialize(); }
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        private void AddConnection(object sender, RoutedEventArgs e)
        {
            AssignConnection();
        }

        private void AddInterface(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem { Header = "PLC Connections" };
            foreach (var record in _registry.PlcCommunicators.Cast<PlcCommunicator>())
            { 
                newHeader.Items.Add(new TreeViewItem
                {
                    Header = record.Header.Name,
                    AlternationCount = (int)record.Header.Id
                }); 
            }

            var window = new ComponentCreationWindow("Select a PLC connection to be assigned with a new Communication Interface", newHeader, AssignInterface);
            window.Show();
        }

        private void AddOutputFileHandler(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem { Header = "Communication Interfaces" };
            foreach (var record in _registry.CommunicationInterfaceHandlers.Cast<CommunicationInterfaceHandler>())
            {
                newHeader.Items.Add(new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.PlcCommunicator.Header.Name,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var window = new ComponentCreationWindow("Select a Communication Interface to be assigned with a new Output Handler", newHeader, AssignOutputFileHandler);
            window.Show();
        }

        private void AddVFlashBank(object sender, RoutedEventArgs e)
        {
            AssignVFlashBank();
        }

        private void AddVFlashChannel(object sender, RoutedEventArgs e)
        {
            var newHeaderCommunicationInterface = new TreeViewItem { Header = "Communication Interfaces" };
            foreach (var record in _registry.CommunicationInterfaceHandlers.Cast<CommunicationInterfaceHandler>())
            {
                newHeaderCommunicationInterface.Items.Add(new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.PlcCommunicator.Header.Name,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var newHeaderVFlashBank = new TreeViewItem { Header = "vFlash Banks" };
            foreach (var record in _registry.VFlashTypeBanks.Cast<VFlashTypeBank>())
            {
                newHeaderVFlashBank.Items.Add(new TreeViewItem
                {
                    Header = record.Header.Name,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var window = new ComponentCreationWindow("Select components to be assigned with a new vFlash Channel", newHeaderCommunicationInterface, newHeaderVFlashBank, AssignVFlashChannel);
            window.Show();
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

        private void NewConfiguration(object sender, RoutedEventArgs e)
        {
            _registry.RemoveAll();

            MainRegistryFile.Default.Reset();
            PlcConfigurationFile.Default.Reset();
            CommunicationInterfacePath.Default.Reset();
            OutputHandlerFile.Default.Reset();
            VFlashTypeBankFile.Default.Reset();

            UpdateGui();
            UpdateTreeView();
            Logger.Log("New configuration");
        }

        private void LoadConfiguration(object sender, RoutedEventArgs e)
        {
            //todo won do rejestru!
            var dlg = new OpenFileDialog 
            { 
                DefaultExt = ".ttac", 
                Filter = "ttAgent Configuration (.ttac)|*.ttac" 
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                Logger.Log("Loading configuration from file: " + dlg.FileName);
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(dlg.FileName,
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.Read);
                var projectData = (ProjectFileStruture.ProjectSavedData)formatter.Deserialize(stream);
                stream.Close();

                _registry.RemoveAll();

                MainRegistryFile.Default.Reset();
                PlcConfigurationFile.Default.Reset();
                CommunicationInterfacePath.Default.Reset();
                OutputHandlerFile.Default.Reset();
                VFlashTypeBankFile.Default.Reset();

                MainRegistryFile.Default.PlcCommunicators = projectData.PlcCommunicators;
                MainRegistryFile.Default.CommunicationInterfaceHandlers = projectData.CommunicationInterfaceHandlers;
                MainRegistryFile.Default.OutputHandlers = projectData.OutputHandlers;
                MainRegistryFile.Default.VFlashTypeBanks = projectData.VFlashTypeBanks;
                MainRegistryFile.Default.VFlashHandlers = projectData.VFlashHandlers;
                MainRegistryFile.Default.Save();

                PlcConfigurationFile.Default.Configuration = projectData.Configuration;
                PlcConfigurationFile.Default.ConnectAtStartUp = projectData.ConnectAtStartUp;
                PlcConfigurationFile.Default.Save();

                CommunicationInterfacePath.Default.Path = projectData.Path;
                CommunicationInterfacePath.Default.ConfigurationStatus = projectData.ConfigurationStatus;
                CommunicationInterfacePath.Default.Save();

                OutputHandlerFile.Default.FileNameSuffixes = projectData.FileNameSuffixes;
                OutputHandlerFile.Default.StartAddress = projectData.StartAddress;
                OutputHandlerFile.Default.EndAddress = projectData.EndAddress;
                OutputHandlerFile.Default.SelectedIndex = projectData.SelectedIndex;
                OutputHandlerFile.Default.Save();

                VFlashTypeBankFile.Default.TypeBank = projectData.TypeBank;
                VFlashTypeBankFile.Default.Save();

                Logger.Log("Registry initialization");
                _registry.Initialize();

                UpdateGui();
                UpdateTreeView();
            }
            Logger.Log("Configuration loaded");
        }

        private void SaveConfiguration(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "configuration",
                DefaultExt = ".ttac",
                Filter = "ttAgent Configuration (.ttac)|*.ttac"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                Logger.Log("Saveing configuration to file: " + dlg.FileName);
                var projectData = new ProjectFileStruture.ProjectSavedData
                {
                    PlcCommunicators = MainRegistryFile.Default.PlcCommunicators,
                    CommunicationInterfaceHandlers = MainRegistryFile.Default.CommunicationInterfaceHandlers,
                    OutputHandlers = MainRegistryFile.Default.OutputHandlers,
                    VFlashTypeBanks = MainRegistryFile.Default.VFlashTypeBanks,
                    VFlashHandlers = MainRegistryFile.Default.VFlashHandlers,

                    Configuration = PlcConfigurationFile.Default.Configuration,
                    ConnectAtStartUp = PlcConfigurationFile.Default.ConnectAtStartUp,

                    Path = CommunicationInterfacePath.Default.Path,
                    ConfigurationStatus = CommunicationInterfacePath.Default.ConfigurationStatus,

                    FileNameSuffixes = OutputHandlerFile.Default.FileNameSuffixes,
                    StartAddress = OutputHandlerFile.Default.StartAddress,
                    EndAddress = OutputHandlerFile.Default.EndAddress,
                    SelectedIndex = OutputHandlerFile.Default.SelectedIndex,

                    TypeBank = VFlashTypeBankFile.Default.TypeBank,
                };

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(dlg.FileName,
                                         FileMode.Create,
                                         FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, projectData);
                stream.Close();
            }
            Logger.Log("Configuration saved");
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

            ComponentManagerTreeView.Height = MainTabControl.Height - 62;
            ComponentManagerTreeView.Width = MainTabControl.Width - 10;

            //todo pomysl o castach!
            foreach (var gui in _registry.GuiCommunicationInterfaceOnlines.Cast<GuiCommunicationInterfaceOnline>()) { gui.UpdateSizes(MainTabControl.Height - 32, MainTabControl.Width - 10); }
            foreach (var gui in _registry.GuiVFlashPathBanks.Cast<GuiVFlashPathBank>()) { gui.UpdateSizes(OutputTabControl.Height - 32, OutputTabControl.Width - 10); }
        }

        private void ComponentManagerSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = (TreeView) sender;
            var selection = (TreeViewItem)treeView.SelectedItem;
            if (selection != null) ComponentManagerSelectionLabel.Content = selection.Header;
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

            foreach (var record in _registry.PlcCommunicators.Cast<PlcCommunicator>())
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                ConnectionTabControl.Items.Add(newtabItem);
                ConnectionTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer();
                newtabItem.Content = newScrollViewer;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridGuiCommunicationStatus = (GuiCommunicationStatus)_registry.PlcGuiCommunicationStatuses.ReturnComponent(record.Header.Id);
                gridGuiCommunicationStatus.Initialize(0, 0, newGrid);

                var gridGuiPlcConfiguration = (GuiPlcConfiguration)_registry.PlcGuiConfigurations.ReturnComponent(record.Header.Id);
                gridGuiPlcConfiguration.Initialize(0, 260, newGrid);
            }

            foreach (var record in _registry.PlcGuiCommunicationStatusBars.Cast<GuiCommunicationStatusBar>())
            {
                record.Initialize(95 * ( (int)record.Header.Id - 1 ), -1, FooterGrid);
            }

            foreach (var record in _registry.CommunicationInterfaceHandlers.Cast<CommunicationInterfaceHandler>())
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                ConnectionTabControl.Items.Add(newtabItem);
                ConnectionTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer();
                newtabItem.Content = newScrollViewer;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridGuiCommunicationInterfaceConfiguration = (GuiComInterfacemunicationConfiguration)_registry.GuiComInterfacemunicationConfigurations.ReturnComponent(record.Header.Id);
                gridGuiCommunicationInterfaceConfiguration.Initialize(0, 0, newGrid);

                newtabItem = new TabItem { Header = record.Header.Name + " Online" };
                MainTabControl.Items.Add(newtabItem);
                MainTabControl.SelectedItem = newtabItem;

                newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = MainTabControl.Height - 32;
                newGrid.Width = MainTabControl.Width - 10;

                var gridGuiCommunicationInterfaceOnline = (GuiCommunicationInterfaceOnline)_registry.GuiCommunicationInterfaceOnlines.ReturnComponent(record.Header.Id);
                gridGuiCommunicationInterfaceOnline.Initialize(0, 0, newGrid);
            }

            foreach (var record in _registry.OutputHandlers.Cast<OutputHandler>())
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                var gridGuiOutputCreator = (GuiOutputHandler)_registry.GuiOutputCreators.ReturnComponent(record.Header.Id);
                gridGuiOutputCreator.Initialize(0, 0, newGrid);
            }

            foreach (var record in _registry.VFlashTypeBanks.Cast<VFlashTypeBank>())
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = OutputTabControl.Height - 32;
                newGrid.Width = OutputTabControl.Width - 10;

                var gridGuiVFlashPathBank = (GuiVFlashPathBank)_registry.GuiVFlashPathBanks.ReturnComponent(record.Header.Id);
                gridGuiVFlashPathBank.Initialize(0, 0, newGrid);
            }

            foreach (var record in _registry.VFlashHandlers.Cast<VFlashHandler>())
            {

                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                var gridVFlash = (GuiVFlash)_registry.GuiVFlashes.ReturnComponent(record.Header.Id);
                gridVFlash.Initialize(0, 0, newGrid);

                var gridGuiVFlashStatusBar = (GuiVFlashStatusBar)_registry.GuiVFlashStatusBars.ReturnComponent(record.Header.Id);
                gridGuiVFlashStatusBar.Initialize(95 * ((int)record.Header.Id - 1), 20, FooterGrid);
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
            foreach (var record in _registry.PlcCommunicators.Cast<PlcCommunicator>())
            { newHeader.Items.Add(new TreeViewItem { Header = record.Header.Name }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "Communication Interfaces" };
            foreach (var record in _registry.CommunicationInterfaceHandlers.Cast<CommunicationInterfaceHandler>())
            { newHeader.Items.Add(new TreeViewItem { Header = record.Header.Name + " ; assigned components: " + record.PlcCommunicator.Header.Name }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "Output Handlers" };
            foreach (var record in _registry.OutputHandlers.Cast<OutputHandler>())
            { newHeader.Items.Add(new TreeViewItem { Header = record.Header.Name + " ; assigned components: " + record.CommunicationInterfaceHandler.Header.Name }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "vFlash Banks" };
            foreach (var record in _registry.VFlashTypeBanks.Cast<VFlashTypeBank>())
            { newHeader.Items.Add(new TreeViewItem { Header = record.Header.Name }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "vFlash Channels" };
            foreach (var record in _registry.VFlashHandlers.Cast<VFlashHandler>())
            { newHeader.Items.Add(new TreeViewItem { Header = record.Header.Name + " ; assigned components: " + record.CommunicationInterfaceHandler.Header.Name + " ; " + record.VFlashTypeBank.Header.Name }); }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }
            
        }

        #endregion

        #region Assignment Methods

        private void AssignConnection()
        {
            var newId = _registry.AddPlcCommunicator(true);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignInterface(uint plcConnectionId)
        {
            var newId = _registry.AddCommunicationInterface(true, plcConnectionId);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignOutputFileHandler(uint communicationInterfaceId)
        {
            var newId = _registry.AddOutputHandler(true, communicationInterfaceId);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignVFlashBank()
        {
            var newId = _registry.AddVFlashBank(true);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignVFlashChannel(uint communicationInterfaceId, uint vFlashBankId)
        {
            var newId = _registry.AddVFlashChannel(true, communicationInterfaceId, vFlashBankId);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        #endregion

    }
}
