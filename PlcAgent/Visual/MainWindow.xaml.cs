using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.License;
using _PlcAgent.Log;
using _PlcAgent.MainRegistry;
using _PlcAgent.Output.OutputHandler;
using _PlcAgent.Output.Template;
using _PlcAgent.Signature;
using _PlcAgent.Vector;
using _PlcAgent.Visual.Gui;
using _PlcAgent.Visual.Gui.Analyzer;
using _PlcAgent.Visual.Gui.DataAquisition;
using _PlcAgent.Visual.Gui.Output;
using _PlcAgent.Visual.Gui.Vector;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Registry = _PlcAgent.MainRegistry.Registry;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TreeView = System.Windows.Controls.TreeView;

namespace _PlcAgent.Visual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Variables

        private Dictionary<TreeViewItem, RegistryComponent> _registryComponents;

        #endregion


        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            Logger.Log("Program Started");

            if (!LicenseHandler.CheckLicense()) CloseApp();
            LicenseLabel.Content = "License generated for: " + LicenseHandler.LicenseOwnerName + ", date: " +
                                   LicenseHandler.LicenseCreationTime;

            Logger.Log("Registry initialization");
            RegistryContext.Registry = new Registry();
            RegistryContext.Registry.Initialize();

            UpdateGui();
            UpdateTreeView();
        }

        #endregion


        #region Buttons

        private void CloseApp(object sender, RoutedEventArgs routedEventArgs)
        {
            CloseApp();
        }

        private void CloseApp(object sender, CancelEventArgs e)
        {
            CloseApp();
        }

        private void AddConnection(object sender, RoutedEventArgs e)
        {
            AssignConnection();
        }

        private void AddInterface(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem {Header = "PLC Connections", IsExpanded = true};
            foreach (PLC.PlcCommunicator record in RegistryContext.Registry.PlcCommunicators)
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

        private void AddOutputDataTemplate(object sender, RoutedEventArgs e)
        {
            AssignOutputDataTemplate();
        }

        private void AddOutputFileHandler(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem {Header = "Communication Interfaces", IsExpanded = true};
            foreach (CommunicationInterfaceHandler record in RegistryContext.Registry.CommunicationInterfaceHandlers)
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
            foreach (CommunicationInterfaceHandler record in RegistryContext.Registry.CommunicationInterfaceHandlers)
            {
                newHeaderCommunicationInterface.Items.Add(new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.PlcCommunicator.Header.Name,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var newHeaderVFlashBank = new TreeViewItem {Header = "vFlash Banks", IsExpanded = true};
            foreach (VFlashTypeBank record in RegistryContext.Registry.VFlashTypeBanks)
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

        private void AddAnalyzer(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem { Header = "Communication Interfaces", IsExpanded = true };
            foreach (CommunicationInterfaceHandler record in RegistryContext.Registry.CommunicationInterfaceHandlers)
            {
                newHeader.Items.Add(new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.PlcCommunicator.Header.Name,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var window = new ComponentCreationWindow("Select a Communication Interface to be assigned with a new Analyzer", newHeader, AssignAnalyzer);
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
            RegistryContext.Registry.MakeNewConfiguration();

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
                var serializationString = (string)formatter.Deserialize(stream);
                stream.Close();

                serializationString = new BlowFish(HexKey.ConfigurationSignatureValue).Decrypt_CTR(serializationString);

                // convert string to stream
                var byteArray = Convert.FromBase64String(serializationString);
                //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                var memoryStream = new MemoryStream(byteArray);

                var projectData = (ProjectFileStruture.ProjectSavedData)new BinaryFormatter().Deserialize(memoryStream);

                RegistryContext.Registry.LoadConfiguration(projectData);

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
                var projectData = RegistryContext.Registry.SaveConfiguration();

                IFormatter formatter = new BinaryFormatter();

                var memoryStream = new MemoryStream();
                formatter.Serialize(memoryStream, projectData);

                var serializationString = new BlowFish(HexKey.ConfigurationSignatureValue).Encrypt_CTR(Convert.ToBase64String(memoryStream.ToArray()));

                Stream stream = new FileStream(dlg.FileName,
                                         FileMode.Create,
                                         FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, serializationString);
                stream.Close();
            }
            Logger.Log("Configuration saved");
        }

        private void RemoveComponent(object sender, RoutedEventArgs e)
        {
            var selection = (TreeViewItem)ComponentManagerTreeView.SelectedItem;
            try
            {
                RegistryContext.Registry.RemoveComponent(_registryComponents[selection]);
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
            UpdateSizes();
        }

        private void UpdateSizes()
        {
            MainTabControl.Height = Limiter.DoubleLimit(ActualHeight - 390, 0);
            MainTabControl.Width = Limiter.DoubleLimit(ActualWidth - 402, 0);

            OutputTabControlGrid.Height = 270;
            OutputTabControlGrid.Width = Limiter.DoubleLimit(ActualWidth - 400, 0);
            OutputTabControl.Height = 268;
            OutputTabControl.Width = Limiter.DoubleLimit(ActualWidth - 402, 0);

            OutputTabControlLabel.Content = "Hide";

            if (MainWindowConfigurationFile.Default.OutputTabControlMinimized)
            {
                OutputTabControlGrid.Height = 28;
                OutputTabControl.Height = 26;
                OutputTabControlLabel.Content = "Show";

                MainTabControl.Height = Limiter.DoubleLimit(ActualHeight - 148, 0);
            }

            ConnectionTabControlGrid.Height = Limiter.DoubleLimit(ActualHeight - 118, 0);
            ConnectionTabControlGrid.Width = 380;
            ConnectionTabControl.Height = Limiter.DoubleLimit(ActualHeight - 120, 0);
            ConnectionTabControl.Width = 378;

            ConnectionTabControlLabel.Content = "Hide";

            if (MainWindowConfigurationFile.Default.ConnectionTabControlMinimized)
            {
                ConnectionTabControlGrid.Width = 32;
                ConnectionTabControl.Width = 30;
                ConnectionTabControlLabel.Content = "Show";

                MainTabControl.Width = Limiter.DoubleLimit(ActualWidth - 54, 0);

                OutputTabControlGrid.Width = Limiter.DoubleLimit(ActualWidth - 52, 0);
                OutputTabControl.Width = Limiter.DoubleLimit(ActualWidth - 54, 0);
            }

            LogListBox.Height = Limiter.DoubleLimit(MainTabControl.Height - 32, 0);
            LogListBox.Width = Limiter.DoubleLimit(MainTabControl.Width - 10, 0);

            AboutGrid.Height = Limiter.DoubleLimit(MainTabControl.Height - 32, 0);
            AboutGrid.Width = Limiter.DoubleLimit(MainTabControl.Width - 10, 0);

            ComponentManagerTreeView.Height = Limiter.DoubleLimit(MainTabControl.Height - 62, 0);
            ComponentManagerTreeView.Width = Limiter.DoubleLimit(MainTabControl.Width - 10, 0);

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
            var selection = (TabItem) MainTabControl.SelectedItem;
            var mainTabControlSelection = selection != null ? selection.Header : null;
            selection = (TabItem)OutputTabControl.SelectedItem;
            var outputTabControlSelection = selection != null ? selection.Header : null;
            selection = (TabItem)ConnectionTabControl.SelectedItem;
            var connectionTabControlSelection = selection != null ? selection.Header : null;

            MainTabControl.Items.Clear();
            OutputTabControl.Items.Clear();
            ConnectionTabControl.Items.Clear();
            FooterGrid.Children.Clear();

            var labelConnectionTabControl = new Label { Content = "Use EDIT menu to create new connections and interfaces." };
            var newTabForLabel = new TabItem { Header = "", Content = labelConnectionTabControl };
            newTabForLabel.MouseDoubleClick += ConnectionTabControlLabel_OnMouseDown;
            ConnectionTabControl.Items.Add(newTabForLabel);
            ConnectionTabControl.SelectedItem = labelConnectionTabControl;

            var labelOutputTabControl = new Label { Content = "Use EDIT menu to create new output handlers." };
            newTabForLabel = new TabItem { Header = "", Content = labelOutputTabControl };
            newTabForLabel.MouseDoubleClick += OutputTabControlLabel_OnMouseDown;
            OutputTabControl.Items.Add(newTabForLabel);
            OutputTabControl.SelectedItem = labelOutputTabControl;

            foreach (PLC.PlcCommunicator record in RegistryContext.Registry.PlcCommunicators)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                ConnectionTabControl.Items.Add(newtabItem);
                ConnectionTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer();
                newtabItem.Content = newScrollViewer;
                newtabItem.MouseDoubleClick += ConnectionTabControlLabel_OnMouseDown;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridGuiCommunicationStatus = (GuiComponent)RegistryContext.Registry.GuiPlcCommunicatorStatuses.ReturnComponent(record.Header.Id);
                gridGuiCommunicationStatus.Initialize(0, 0, newGrid);

                var gridGuiPlcConfiguration = (GuiComponent)RegistryContext.Registry.GuiPlcCommunicatorConfigurations.ReturnComponent(record.Header.Id);
                gridGuiPlcConfiguration.Initialize(0, 275, newGrid);

                var gridGuiPlcConfigurationStatusBar = (GuiComponent)RegistryContext.Registry.GuiPlcCommunicatorStatusBars.ReturnComponent(record.Header.Id);
                gridGuiPlcConfigurationStatusBar.Initialize(95 * ((int)record.Header.Id - 1), -25, FooterGrid);
            }

            foreach (CommunicationInterfaceHandler record in RegistryContext.Registry.CommunicationInterfaceHandlers)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                ConnectionTabControl.Items.Add(newtabItem);
                ConnectionTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer();
                newtabItem.Content = newScrollViewer;
                newtabItem.MouseDoubleClick += ConnectionTabControlLabel_OnMouseDown;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridGuiCommunicationInterfaceConfiguration = (GuiComponent)RegistryContext.Registry.GuiComInterfacemunicationConfigurations.ReturnComponent(record.Header.Id);
                gridGuiCommunicationInterfaceConfiguration.Initialize(0, 0, newGrid);

                newtabItem = new TabItem { Header = record.Header.Name + " Online" };
                MainTabControl.Items.Add(newtabItem);
                MainTabControl.SelectedItem = newtabItem;

                newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = Limiter.DoubleLimit(MainTabControl.Height - 32, 0);
                newGrid.Width = Limiter.DoubleLimit(MainTabControl.Width - 10, 0);

                var gridGuiCommunicationInterfaceOnline = (GuiComponent)RegistryContext.Registry.GuiCommunicationInterfaceOnlines.ReturnComponent(record.Header.Id);
                gridGuiCommunicationInterfaceOnline.Initialize(0, 0, newGrid);
                var guiComponent = (GuiCommunicationInterfaceOnlineHierarchical) gridGuiCommunicationInterfaceOnline.UserControl;
                guiComponent.UpdateSizes(newGrid.Height, newGrid.Width);
                guiComponent.TabItem = newtabItem;
            }

            foreach (OutputDataTemplate record in RegistryContext.Registry.OutputDataTemplates)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                MainTabControl.Items.Add(newtabItem);
                MainTabControl.SelectedItem = newtabItem;

                var newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = Limiter.DoubleLimit(MainTabControl.Height - 32, 0);
                newGrid.Width = Limiter.DoubleLimit(MainTabControl.Width - 10, 0);

                var gridGuiOutputDataTemplate = (GuiComponent)RegistryContext.Registry.GuiOutputDataTemplates.ReturnComponent(record.Header.Id);
                gridGuiOutputDataTemplate.Initialize(0, 0, newGrid);

                var gridGuiOutputDataTemplateGrid = (GuiOutputDataTemplate)gridGuiOutputDataTemplate.UserControl;
                gridGuiOutputDataTemplateGrid.UpdateSizes(newGrid.Height, newGrid.Width);
            }

            foreach (OutputHandler record in RegistryContext.Registry.OutputHandlers)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
                };
                newtabItem.Content = newScrollViewer;
                newtabItem.MouseDoubleClick += OutputTabControlLabel_OnMouseDown;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var guiOutputHandlerComponent = (GuiComponent)RegistryContext.Registry.GuiOutputHandlerComponents.ReturnComponent(record.Header.Id);
                guiOutputHandlerComponent.Initialize(0, 0, newGrid);

                var gridGuiInterfaceAssignment = (GuiComponent)RegistryContext.Registry.GuiOutputHandlerInterfaceAssignmentComponents.ReturnComponent(record.Header.Id);
                gridGuiInterfaceAssignment.Initialize(402, 0, newGrid);
            }

            foreach (VFlashTypeBank record in RegistryContext.Registry.VFlashTypeBanks)
            {
                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
                };
                newtabItem.Content = newScrollViewer;
                newtabItem.MouseDoubleClick += OutputTabControlLabel_OnMouseDown;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                newGrid.Height = Limiter.DoubleLimit(OutputTabControl.Height - 50.0, 0);
                newGrid.Width = Limiter.DoubleLimit(OutputTabControl.Width - 10, 0);

                var gridGuiVFlashPathBank = (GuiComponent)RegistryContext.Registry.GuiVFlashPathBanks.ReturnComponent(record.Header.Id);
                gridGuiVFlashPathBank.Initialize(0, 0, newGrid);
                var guiComponent = (GuiVFlashPathBank)gridGuiVFlashPathBank.UserControl;
                guiComponent.UpdateSizes(newGrid.Height, newGrid.Width);
            }

            foreach (VFlashHandler record in RegistryContext.Registry.VFlashHandlers)
            {

                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
                };
                newtabItem.Content = newScrollViewer;
                newtabItem.MouseDoubleClick += OutputTabControlLabel_OnMouseDown;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridVFlashComponent = (GuiComponent)RegistryContext.Registry.GuiVFlashHandlerComponents.ReturnComponent(record.Header.Id);
                gridVFlashComponent.Initialize(0, 0, newGrid);

                var gridGuiVFlashStatusBar = (GuiComponent)RegistryContext.Registry.GuiVFlashStatusBars.ReturnComponent(record.Header.Id);
                gridGuiVFlashStatusBar.Initialize(95 * ((int)record.Header.Id - 1), 18, FooterGrid);

                var gridGuiInterfaceAssignment = (GuiComponent)RegistryContext.Registry.GuiVFlashHandlerInterfaceAssignmentComponents.ReturnComponent(record.Header.Id);
                gridGuiInterfaceAssignment.Initialize(402, 0, newGrid);
            }

            foreach (Analyzer.Analyzer record in RegistryContext.Registry.Analyzers)
            {

                var newtabItem = new TabItem { Header = record.Header.Name };
                OutputTabControl.Items.Add(newtabItem);
                OutputTabControl.SelectedItem = newtabItem;

                var newScrollViewer = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
                };
                newtabItem.Content = newScrollViewer;
                newtabItem.MouseDoubleClick += OutputTabControlLabel_OnMouseDown;

                var newGrid = new Grid();
                newScrollViewer.Content = newGrid;

                var gridAnalyzerConfiguration = (GuiComponent)RegistryContext.Registry.GuiAnalyzerConfigurations.ReturnComponent(record.Header.Id);
                gridAnalyzerConfiguration.Initialize(0, 0, newGrid);

                var gridAnalyzerControl = (GuiComponent)RegistryContext.Registry.GuiAnalyzerControls.ReturnComponent(record.Header.Id);
                gridAnalyzerControl.Initialize(0, 150, newGrid);

                var gridGuiInterfaceAssignment = (GuiComponent)RegistryContext.Registry.GuiAnalyzerInterfaceAssignmentComponents.ReturnComponent(record.Header.Id);
                gridGuiInterfaceAssignment.Initialize(402, 0, newGrid);

                var gridGuiDataCursorTable = (GuiComponent)RegistryContext.Registry.GuiAnalyzerDataCursorTables.ReturnComponent(record.Header.Id);
                gridGuiDataCursorTable.Initialize(927, 0, newGrid);

                newtabItem = new TabItem { Header = record.Header.Name };
                MainTabControl.Items.Add(newtabItem);
                MainTabControl.SelectedItem = newtabItem;

                newGrid = new Grid();
                newtabItem.Content = newGrid;

                newGrid.Height = Limiter.DoubleLimit(MainTabControl.Height - 32, 0);
                newGrid.Width = Limiter.DoubleLimit(MainTabControl.Width - 10, 0);

                var analyzerMainFrameGrid = (GuiComponent)RegistryContext.Registry.GuiAnalyzerMainFrames.ReturnComponent(record.Header.Id);
                analyzerMainFrameGrid.Initialize(0, 0, newGrid);

                var guiAnalyzerMainFrameGrid = (GuiAnalyzerMainFrame) analyzerMainFrameGrid.UserControl;
                guiAnalyzerMainFrameGrid.UpdateSizes(newGrid.Height, newGrid.Width);
            }

            MainTabControl.Items.Add(ComponentManagerTabItem);
            MainTabControl.SelectedItem = ComponentManagerTabItem;
            MainTabControl.Items.Add(AboutTabItem);
            MainTabControl.Items.Add(LogTabItem);

            SelectTabItem(MainTabControl, mainTabControlSelection);
            SelectTabItem(ConnectionTabControl, connectionTabControlSelection);
            SelectTabItem(OutputTabControl, outputTabControlSelection);
        }

        private static void SelectTabItem(TabControl tabControl, object tabItemHeader)
        {
            if (tabItemHeader == null) return;
            foreach (var tabItem in tabControl.Items.Cast<object>().Where(item => item.GetType() == typeof (TabItem)).Cast<TabItem>().Where(tabItem => Equals(tabItem.Header, tabItemHeader)))
            {
                tabControl.SelectedItem = tabItem;
            }
        }

        private void UpdateTreeView()
        {
            ComponentManagerTreeView.Items.Clear();
            _registryComponents = new Dictionary<TreeViewItem, RegistryComponent>();

            var mainHeader = new TreeViewItem {Header = "Components", IsExpanded = true};
            ComponentManagerTreeView.Items.Add(mainHeader);
            
            var newHeader= new TreeViewItem {Header = "PLC Connections", IsExpanded = true};
            foreach (PLC.PlcCommunicator record in RegistryContext.Registry.PlcCommunicators)
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
            foreach (CommunicationInterfaceHandler record in RegistryContext.Registry.CommunicationInterfaceHandlers)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.PlcCommunicator.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem);
            }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem { Header = "Output Data Templates", IsExpanded = true };
            foreach (OutputDataTemplate record in RegistryContext.Registry.OutputDataTemplates)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem);
            }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }

            newHeader = new TreeViewItem {Header = "Output Handlers", IsExpanded = true};
            foreach (OutputHandler record in RegistryContext.Registry.OutputHandlers)
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
            foreach (VFlashTypeBank record in RegistryContext.Registry.VFlashTypeBanks)
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
            foreach (VFlashHandler record in RegistryContext.Registry.VFlashHandlers)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.CommunicationInterfaceHandler.Header.Name + " ; " + record.VFlashTypeBank.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem);
            }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }   

            newHeader = new TreeViewItem { Header = "Analyzers", IsExpanded = true };
            foreach (Analyzer.Analyzer record in RegistryContext.Registry.Analyzers)
            {
                var treeViewItem = new TreeViewItem
                {
                    Header = record.Header.Name + " ; assigned components: " + record.CommunicationInterfaceHandler.Header.Name
                };
                _registryComponents.Add(treeViewItem, record);
                newHeader.Items.Add(treeViewItem);
            }
            if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }     
        }

        private void Label_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            label.Foreground = Brushes.DarkGray;
        }

        private void Label_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            label.Foreground = Brushes.Black;
        }

        private void OutputTabControlLabel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ModifyTabControlMinimizedSetting("Output");
        }

        private void ConnectionTabControlLabel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ModifyTabControlMinimizedSetting("Connection");
        }

        private void ModifyTabControlMinimizedSetting(string tabDescription)
        {
            switch (tabDescription)
            {
                case "Output":
                    MainWindowConfigurationFile.Default.OutputTabControlMinimized = !MainWindowConfigurationFile.Default.OutputTabControlMinimized;
                    break;
                case "Connection":
                    MainWindowConfigurationFile.Default.ConnectionTabControlMinimized = !MainWindowConfigurationFile.Default.ConnectionTabControlMinimized;
                    break;
            }
            MainWindowConfigurationFile.Default.Save();
            UpdateSizes();
        }

        #endregion


        #region Assignment Methods

        private void AssignConnection()
        {
            var newId = RegistryContext.Registry.AddPlcCommunicator(true);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignInterface(uint plcConnectionId)
        {
            var newId = RegistryContext.Registry.AddCommunicationInterface(true, plcConnectionId);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignOutputDataTemplate()
        {
            var newId = RegistryContext.Registry.AddOutputDataTemplate(true);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignOutputFileHandler(uint communicationInterfaceId)
        {
            var newId = RegistryContext.Registry.AddOutputHandler(true, communicationInterfaceId);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignVFlashBank()
        {
            var newId = RegistryContext.Registry.AddVFlashBank(true);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignVFlashChannel(uint communicationInterfaceId, uint vFlashBankId)
        {
            var newId = RegistryContext.Registry.AddVFlashChannel(true, communicationInterfaceId, vFlashBankId);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        private void AssignAnalyzer(uint communicationInterfaceId)
        {
            var newId = RegistryContext.Registry.AddAnalyzer(true, communicationInterfaceId);
            if (newId == 0) return;

            UpdateGui();
            UpdateTreeView();
        }

        #endregion


        #region Methods

        private void CloseApp()
        {
            if(RegistryContext.Registry != null) RegistryContext.Registry.Deinitialize();
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        #endregion

    }
}
