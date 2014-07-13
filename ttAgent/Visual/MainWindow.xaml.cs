using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using _ttAgent.DataAquisition;
using _ttAgent.General;
using _ttAgent.Log;
using _ttAgent.MainRegistry;
using _ttAgent.Output;
using _ttAgent.PLC;
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
        private Dictionary<TreeViewItem, RegistryComponent> _registryComponents;

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
                foreach (CommunicationInterfaceHandler communicationInterfaceHandler in _registry.CommunicationInterfaceHandlers)
                {
                    communicationInterfaceHandler.MaintainConnection();
                }
                Thread.Sleep(10);
            }
        }

        #endregion

        #region Buttons

        private void CloseApp(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (VFlashHandler vFlashHandler in _registry.VFlashHandlers) { vFlashHandler.Deinitialize(); }
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        private void CloseApp(object sender, CancelEventArgs e)
        {
            foreach (VFlashHandler vFlashHandler in _registry.VFlashHandlers) { vFlashHandler.Deinitialize(); }
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        private void AddConnection(object sender, RoutedEventArgs e)
        {
            AssignConnection();
        }

        private void AddInterface(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem {Header = "PLC Connections", IsExpanded = true};
            foreach (PlcCommunicator record in _registry.PlcCommunicators)
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
            var newHeader = new TreeViewItem {Header = "Communication Interfaces", IsExpanded = true};
            foreach (CommunicationInterfaceHandler record in _registry.CommunicationInterfaceHandlers)
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
            var newHeaderCommunicationInterface = new TreeViewItem
            {
                Header = "Communication Interfaces",
                IsExpanded = true
            };
            foreach (CommunicationInterfaceHandler record in _registry.CommunicationInterfaceHandlers)
            {
                newHeaderCommunicationInterface.Items.Add(new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.PlcCommunicator.Header.Name,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var newHeaderVFlashBank = new TreeViewItem {Header = "vFlash Banks", IsExpanded = true};
            foreach (VFlashTypeBank record in _registry.VFlashTypeBanks)
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
            MainTabControl.SelectedItem = AboutTabItem;
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
            _registry.MakeNewConfiguration();

            UpdateGui();
            UpdateTreeView();
            Logger.Log("New configuration");
        }

        private void LoadConfiguration(object sender, RoutedEventArgs e)
        {
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

                _registry.LoadConfiguration(projectData);

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
                var projectData = _registry.SaveConfiguration();

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(dlg.FileName,
                                         FileMode.Create,
                                         FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, projectData);
                stream.Close();
            }
            Logger.Log("Configuration saved");
        }

        private void RemoveComponent(object sender, RoutedEventArgs e)
        {
            var selection = (TreeViewItem)ComponentManagerTreeView.SelectedItem;
            try
            {
                _registry.RemoveComponent(_registryComponents[selection]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Component cannot be removed");
            }
            
            UpdateGui();
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

            ComponentManagerTreeView.Height = MainTabControl.Height - 62;
            ComponentManagerTreeView.Width = MainTabControl.Width - 10;

            UpdateGui();
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

            foreach (PlcCommunicator record in _registry.PlcCommunicators)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                ConnectionTabControl.Items.Add(newtabItem);
                ConnectionTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer();
                newtabItem.Content = newScrollViewer;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridGuiCommunicationStatus = (GuiComponent)_registry.PlcGuiCommunicationStatuses.ReturnComponent(record.Header.Id);
                gridGuiCommunicationStatus.Initialize(0, 0, newGrid);

                var gridGuiPlcConfiguration = (GuiComponent)_registry.PlcGuiConfigurations.ReturnComponent(record.Header.Id);
                gridGuiPlcConfiguration.Initialize(0, 275, newGrid);

                var gridGuiPlcConfigurationStatusBar = (GuiComponent)_registry.PlcGuiCommunicationStatusBars.ReturnComponent(record.Header.Id);
                gridGuiPlcConfigurationStatusBar.Initialize(95 * ((int)record.Header.Id - 1), -25, FooterGrid);
            }

            foreach (CommunicationInterfaceHandler record in _registry.CommunicationInterfaceHandlers)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                ConnectionTabControl.Items.Add(newtabItem);
                ConnectionTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer();
                newtabItem.Content = newScrollViewer;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridGuiCommunicationInterfaceConfiguration = (GuiComponent)_registry.GuiComInterfacemunicationConfigurations.ReturnComponent(record.Header.Id);
                gridGuiCommunicationInterfaceConfiguration.Initialize(0, 0, newGrid);

                newtabItem = new TabItem { Header = record.Header.Name + " Online" };
                MainTabControl.Items.Add(newtabItem);
                MainTabControl.SelectedItem = newtabItem;

                newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = MainTabControl.Height - 32;
                newGrid.Width = MainTabControl.Width - 10;

                var gridGuiCommunicationInterfaceOnline = (GuiComponent)_registry.GuiCommunicationInterfaceOnlines.ReturnComponent(record.Header.Id);
                gridGuiCommunicationInterfaceOnline.Initialize(0, 0, newGrid);
                var guiComponent = (GuiCommunicationInterfaceOnline) gridGuiCommunicationInterfaceOnline.UserControl;
                guiComponent.UpdateSizes(newGrid.Height, newGrid.Width);
                guiComponent.TabItem = newtabItem;

                MainTabControl.SelectionChanged += guiComponent.SelectionChanged;
            }

            foreach (OutputHandler record in _registry.OutputHandlers)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                var guiOutputHandlerComponent = (GuiComponent)_registry.GuiOutputHandlerComponents.ReturnComponent(record.Header.Id);
                guiOutputHandlerComponent.Initialize(0, 0, newGrid);

                var gridGuiInterfaceAssignment = (GuiComponent)_registry.GuiOutputHandlerInterfaceAssignmentComponents.ReturnComponent(record.Header.Id);
                gridGuiInterfaceAssignment.Initialize(402, 0, newGrid);
            }

            foreach (VFlashTypeBank record in _registry.VFlashTypeBanks)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = OutputTabControl.Height - 32;
                newGrid.Width = OutputTabControl.Width - 10;

                var gridGuiVFlashPathBank = (GuiComponent)_registry.GuiVFlashPathBanks.ReturnComponent(record.Header.Id);
                gridGuiVFlashPathBank.Initialize(0, 0, newGrid);
                var guiComponent = (GuiVFlashPathBank)gridGuiVFlashPathBank.UserControl;
                guiComponent.UpdateSizes(newGrid.Height, newGrid.Width);
            }

            foreach (VFlashHandler record in _registry.VFlashHandlers)
            {

                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                var gridVFlashComponent = (GuiComponent)_registry.GuiVFlashHandlerComponents.ReturnComponent(record.Header.Id);
                gridVFlashComponent.Initialize(0, 0, newGrid);

                var gridGuiVFlashStatusBar = (GuiComponent)_registry.GuiVFlashStatusBars.ReturnComponent(record.Header.Id);
                gridGuiVFlashStatusBar.Initialize(95 * ((int)record.Header.Id - 1), 18, FooterGrid);

                var gridGuiInterfaceAssignment = (GuiComponent)_registry.GuiVFlashHandlerInterfaceAssignmentComponents.ReturnComponent(record.Header.Id);
                gridGuiInterfaceAssignment.Initialize(402, 0, newGrid);
            }

            MainTabControl.Items.Add(ComponentManagerTabItem);
            MainTabControl.SelectedItem = ComponentManagerTabItem;
            MainTabControl.Items.Add(AboutTabItem);
            MainTabControl.Items.Add(LogTabItem);
        }

        private void UpdateTreeView()
        {
            ComponentManagerTreeView.Items.Clear();
            _registryComponents = new Dictionary<TreeViewItem, RegistryComponent>();

            var mainHeader = new TreeViewItem {Header = "Components", IsExpanded = true};
            ComponentManagerTreeView.Items.Add(mainHeader);
            
            var newHeader= new TreeViewItem {Header = "PLC Connections", IsExpanded = true};
            foreach (PlcCommunicator record in _registry.PlcCommunicators)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem); 
            }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem {Header = "Communication Interfaces", IsExpanded = true};
            foreach (CommunicationInterfaceHandler record in _registry.CommunicationInterfaceHandlers)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.PlcCommunicator.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem);
            }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem {Header = "Output Handlers", IsExpanded = true};
            foreach (OutputHandler record in _registry.OutputHandlers)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.CommunicationInterfaceHandler.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem);
            }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem {Header = "vFlash Banks", IsExpanded = true};
            foreach (VFlashTypeBank record in _registry.VFlashTypeBanks)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem);
            }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem {Header = "vFlash Channels", IsExpanded = true};
            foreach (VFlashHandler record in _registry.VFlashHandlers)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.CommunicationInterfaceHandler.Header.Name + " ; " + record.VFlashTypeBank.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem);
            }
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
