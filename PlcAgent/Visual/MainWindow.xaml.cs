using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.License;
using _PlcAgent.Log;
using _PlcAgent.MainRegistry;
using _PlcAgent.Output.Template;
using _PlcAgent.Signature;
using _PlcAgent.Vector;
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
            RegistryContext.Registry.OnRegistryUpdated += OnRegistryUpdated;
            RegistryContext.Registry.Initialize();
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
            RegistryContext.Registry.AddPlcCommunicator(true);
        }

        private void AddInterface(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem {Header = "PLC Connections", IsExpanded = true};
            foreach (PLC.PlcCommunicator record in RegistryContext.Registry.PlcCommunicators)
            { 
                newHeader.Items.Add(new TreeViewItem
                {
                    Header = record.Description,
                    AlternationCount = (int)record.Header.Id
                }); 
            }

            var window = new ComponentCreationWindow("Select a PLC connection to be assigned with a new Communication Interface", newHeader, RegistryContext.Registry.AddCommunicationInterface);
            window.Show();
        }

        private void AddOutputDataTemplate(object sender, RoutedEventArgs e)
        {
            RegistryContext.Registry.AddOutputDataTemplate(true);
        }

        private void AddOutputFileCreator(object sender, RoutedEventArgs e)
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
                    Header = record.Description,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var newHeaderOutputDataTemplate = new TreeViewItem
            {
                Header = "Output Data Templates", 
                IsExpanded = true
            };
            foreach (OutputDataTemplate record in RegistryContext.Registry.OutputDataTemplates)
            {
                newHeaderOutputDataTemplate.Items.Add(new TreeViewItem
                {
                    Header = record.Description,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var window = new ComponentCreationWindow("Select components to be assigned with a new Output File Creator", newHeaderCommunicationInterface, newHeaderOutputDataTemplate, RegistryContext.Registry.AddOutputFileCreator);
            window.Show();
        }

        private void AddOutputFileHandler(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem {Header = "Communication Interfaces", IsExpanded = true};
            foreach (CommunicationInterfaceHandler record in RegistryContext.Registry.CommunicationInterfaceHandlers)
            {
                newHeader.Items.Add(new TreeViewItem
                {
                    Header = record.Description,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var window = new ComponentCreationWindow("Select a Communication Interface to be assigned with a new Output Handler", newHeader, RegistryContext.Registry.AddOutputHandler);
            window.Show();
        }

        private void AddDbConnectionHandler(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem { Header = "Communication Interfaces", IsExpanded = true };
            foreach (CommunicationInterfaceHandler record in RegistryContext.Registry.CommunicationInterfaceHandlers)
            {
                newHeader.Items.Add(new TreeViewItem
                {
                    Header = record.Description,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var window = new ComponentCreationWindow("Select a Communication Interface to be assigned with a new DB Connection Handler", newHeader, RegistryContext.Registry.AddDbConnectionHandler);
            window.Show();
        }

        private void AddVFlashBank(object sender, RoutedEventArgs e)
        {
            RegistryContext.Registry.AddVFlashBank(true);
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
                    Header = record.Description,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var newHeaderVFlashBank = new TreeViewItem {Header = "vFlash Banks", IsExpanded = true};
            foreach (VFlashTypeBank record in RegistryContext.Registry.VFlashTypeBanks)
            {
                newHeaderVFlashBank.Items.Add(new TreeViewItem
                {
                    Header = record.Description,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var window = new ComponentCreationWindow("Select components to be assigned with a new vFlash Channel", newHeaderCommunicationInterface, newHeaderVFlashBank, RegistryContext.Registry.AddVFlashChannel);
            window.Show();
        }

        private void AddAnalyzer(object sender, RoutedEventArgs e)
        {
            var newHeader = new TreeViewItem { Header = "Communication Interfaces", IsExpanded = true };
            foreach (CommunicationInterfaceHandler record in RegistryContext.Registry.CommunicationInterfaceHandlers)
            {
                newHeader.Items.Add(new TreeViewItem
                {
                    Header = record.Description,
                    AlternationCount = (int)record.Header.Id
                });
            }

            var window = new ComponentCreationWindow("Select a Communication Interface to be assigned with a new Analyzer", newHeader, RegistryContext.Registry.AddAnalyzer);
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
            try { RegistryContext.Registry.RemoveComponent(_registryComponents[selection]);}
            catch (Exception exception) { MessageBox.Show(exception.Message, "Component cannot be removed");}
        }

        #endregion


        #region GUI Parameters Handleing

        private void OnRegistryUpdated()
        {
            UpdateGui();
            UpdateTreeView();
        }

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

            ComponentManagerTreeView.Height = Limiter.DoubleLimit(MainTabControl.Height - 59, 0);
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

            foreach (TabItem tabItem in OutputTabControl.Items) tabItem.MouseDoubleClick += OutputTabControlLabel_OnMouseDown;
            foreach (TabItem tabItem in ConnectionTabControl.Items) tabItem.MouseDoubleClick += ConnectionTabControlLabel_OnMouseDown;
            foreach (var module in from RegistryComposite moduleComposite in RegistryContext.Registry.Modules from Module module in moduleComposite select module)
            {
                module.TemplateGuiUpdate(MainTabControl, OutputTabControl, ConnectionTabControl, FooterGrid);
            }

            MainTabControl.Items.Add(ComponentManagerTabItem);
            MainTabControl.Items.Add(AboutTabItem);
            MainTabControl.Items.Add(LogTabItem);

            SelectTabItem(MainTabControl, mainTabControlSelection);
            SelectTabItem(ConnectionTabControl, connectionTabControlSelection);
            SelectTabItem(OutputTabControl, outputTabControlSelection);
        }

        private static void SelectTabItem(Selector tabControl, object tabItemHeader)
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

            var mainHeader = new TreeViewItem { Header = "Components", IsExpanded = true };
            ComponentManagerTreeView.Items.Add(mainHeader);

            foreach (var record in RegistryContext.Registry.Modules)
            {
                var module = (RegistryComponent)record;
                var newHeader = new TreeViewItem { Header = module.Header.Name, IsExpanded = true };

                var composite = (RegistryComposite)record;
                foreach (RegistryComponent component in composite)
                {
                    var treeViewItem = new TreeViewItem
                    {
                        Header = component.Description
                    };
                    _registryComponents.Add(treeViewItem, component);
                    newHeader.Items.Add(treeViewItem);
                }
                if (!newHeader.Items.IsEmpty) { mainHeader.Items.Add(newHeader); }
            }   
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


        #region Methods

        private static void CloseApp()
        {
            if(RegistryContext.Registry != null) RegistryContext.Registry.Deinitialize();
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        #endregion

    }
}
