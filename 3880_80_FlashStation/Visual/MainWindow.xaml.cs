﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.Output;
using _3880_80_FlashStation.PLC;
using _3880_80_FlashStation.Resources.Vector;
using _3880_80_FlashStation.Vector;

namespace _3880_80_FlashStation.Visual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Thread _statusThread;
        private readonly Thread _communicationThread;

        private readonly PlcCommunicator _plcCommunication;
        private readonly PlcConfigurator _plcConfiguration;

        private readonly VFlashHandler _vFlash;

        private PlcCommunicatorBase.PlcConfig _guiPlcConfiguration;
        private readonly CommunicationInterfaceHandler _communicationHandler;

        private FaultReport _windowReport;

        public MainWindow()
        {
            InitializeComponent();
            Logger.Log("Program Started");

            VFlash1UnloadButton.IsEnabled = false;
            VFlash1FlashButton.IsEnabled = false;

            _communicationHandler = new CommunicationInterfaceHandler();
            _communicationHandler.Initialize("readInterface");
            _communicationHandler.Initialize("writeInterface");

            OnlineReadDataListBox.Items.Add("Read area: ");
            OnlineWriteDataListBox.Items.Add("Write area: ");

            _plcCommunication = new PlcCommunicator();
            _plcConfiguration = new PlcConfigurator();

            _guiPlcConfiguration = PlcConfigurationFile.Default.Configuration;
            UpdateSettings();
            if (PlcConfigurationFile.Default.Configuration.PlcConfigurationStatus == 1) { StoreSettings(); }

            try { _vFlash = new VFlashHandler(_communicationHandler.ReadInterfaceComposite,_communicationHandler.WriteInterfaceComposite);}
            catch (Exception)
            {
                MessageBox.Show("VFlash initialization failed", "VFlash Failed");
                Environment.Exit(0);
            }

            VFlashTypeConverter.StringsToVFlashChannels(VFlashTypeBankFile.Default.TypeBank, _vFlash.VFlashTypeBank);
            foreach (var type in _vFlash.VFlashTypeBank.Children.Cast<VFlashTypeComponent>()) { VFlashBankListBox.Items.Add("Type: " + type.Type + " : " + "File: " + type.Path); }

            _statusThread = new Thread(StatusHandler);
            _statusThread.SetApartmentState(ApartmentState.STA);
            _statusThread.IsBackground = true;
            _statusThread.Start();

            _communicationThread = new Thread(CommunicationHandler);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;
            _communicationThread.Start();
        }

        private void StatusHandler()
        {
            while (_statusThread.IsAlive)
            {
                if (_plcCommunication != null)
                {
                    ActualConfigurationHandler(_plcCommunication.PlcConfiguration);
                    StatusBarHandler(_plcCommunication);
                    OnlineDataDisplayHandler(_plcCommunication);
                    VFlashDisplayHandler(_vFlash);
                }
                Thread.Sleep(21);
            }
        }

        private void CommunicationHandler()
        {
            while (_communicationThread.IsAlive)
            {
                if (_plcCommunication != null && _plcCommunication.ConnectionStatus == 1)
                {
                    _communicationHandler.MaintainConnection(_plcCommunication);
                }
                Thread.Sleep(11);
            }
        }

        private void StoreSettings()
        {
            _guiPlcConfiguration.PlcConfigurationStatus = 1;
            PlcConfigurationFile.Default.Configuration = _guiPlcConfiguration;
            PlcConfigurationFile.Default.Save();
            _plcConfiguration.UpdateConfiguration(PlcConfigurationFile.Default.Configuration);
            _plcCommunication.SetupConnection(_plcConfiguration);
        }

        #region Buttons

        private void CloseApp(object sender, CancelEventArgs cancelEventArgs)
        {
            _vFlash.Deinitialize();
            Logger.Log("Program Closed");
            Environment.Exit(0);
        }

        private void StoreSettings(object sender, RoutedEventArgs e)
        {
            Logger.Log("Communication configuration has been changed");
            StoreSettings();
        }

        private void ConnectDisconnect(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_plcCommunication != null)
                {
                    if (_plcCommunication.ConnectionStatus != 1)
                    {
                        _plcCommunication.OpenConnection();
                        ConnectButton.Dispatcher.BeginInvoke((new Action(delegate { ConnectButton.Content = "Disconnect"; })));
                        Logger.Log("Connected with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress);
                    }
                    else
                    {
                        _plcCommunication.CloseConnection();
                        ConnectButton.Dispatcher.BeginInvoke((new Action(delegate { ConnectButton.Content = "Connect"; })));
                        Logger.Log("Disconnected with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress);
                    } 
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Connection Failed");
                if (_plcCommunication != null)
                    Logger.Log("Connection trial with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress + " failed");
            }
        }

        private void LoadVFlashProject(object sender, RoutedEventArgs e)
        {
            var loadButton = (Button) sender;
            // Create OpenFileDialog
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".vflashpack", Filter = "Flash Project (.vflashpack)|*.vflashpack" };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                switch (loadButton.Name)
                {
                    case "VFlash1LoadButton":
                        try
                        {
                            _vFlash.SetProjectPath(1, dlg.FileName);
                            _vFlash.LoadProject(1);
                            Logger.Log("Project load requested by the operator");
                        }
                        catch (Exception exception) { MessageBox.Show(exception.Message, "Project Loading Failed"); }
                        break;
                        //todo: implement for the others
                }
            }
        }

        private void UnloadVFlashProject(object sender, RoutedEventArgs e)
        {
            var unloadButton = (Button)sender;
            switch (unloadButton.Name)
            {
                case "VFlash1UnloadButton":
                    try
                    {
                        _vFlash.UnloadProject(1);
                        Logger.Log("Project unload requested by the operator");
                    }
                    catch (Exception exception) { MessageBox.Show(exception.Message, "Project Unloading Failed"); }
                    break;
                //todo: implement for the others
            }
        }

        private void FlashVFlashProject(object sender, RoutedEventArgs e)
        {
            var flashButton = (Button)sender;
            switch (flashButton.Name)
            {
                case "VFlash1FlashButton":
                    var channel = _vFlash.ReturnChannelSetup(1);
                    if (channel.Status == "Flashing")
                    {
                        try
                        {
                            _vFlash.AbortFlashing(1);
                            Logger.Log("Flash abort requested by the operator");
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Flash Abort Failed");
                        }
                    }
                    if (channel.Status != "Flashing")
                    {
                        try
                        {
                            _vFlash.StartFlashing(1);
                            Logger.Log("Project start requested by the operator");
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Project Flashing Failed");
                        }
                    }
                    break;
                    //todo implement for the others
            }
        }

        private void VFlashShowFaults(object sender, RoutedEventArgs e)
        {
            _windowReport = new FaultReport(ClearFaults);
            _windowReport.Show();
            _windowReport.FaultListBox.Items.Add(_vFlash.ErrorCollector.CreateReport());
            _vFlash.ErrorCollector.CreateReport();
        }

        private void ClearFaults()
        {
            _vFlash.ErrorCollector.Clear();
            _windowReport.FaultListBox.Items.Clear();
            _windowReport.FaultListBox.Items.Add(_vFlash.ErrorCollector.CreateReport());
            Logger.Log("VFlash: Fault list ereased");
        }

        private void TypeCreation(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".vflashpack", Filter = "Flash Project (.vflashpack)|*.vflashpack" };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                _vFlash.VFlashTypeBank.Add(new VFlashTypeComponent(Convert.ToUInt16(TypeNumberBox.Text), dlg.FileName));

                VFlashTypeBankFile.Default.TypeBank = VFlashTypeConverter.VFlashTypesToStrings(_vFlash.VFlashTypeBank.Children);
                VFlashTypeBankFile.Default.Save();

                VFlashBankListBox.Items.Clear();
                foreach (var vFlashType in _vFlash.VFlashTypeBank.Children)
                {
                    var type = (VFlashTypeComponent) vFlashType;
                    VFlashBankListBox.Items.Add("Type: " + type.Type + " : " + "File: " + type.Path);
                }
            }
        }

        private void CreateOutput(object sender, RoutedEventArgs e)
        {
            var selection = OutputTypeComboBox.SelectedValue;
            if (selection == null) 
            { 
                MessageBox.Show("No file type selected!", "Error");
                return;
            }
            switch (selection.ToString())
            {
                case "System.Windows.Controls.ComboBoxItem: *.xml":
                    var xmlWriter = new OutputXmlWriter();
                    xmlWriter.CreateOutput("out", xmlWriter.InterfaceToStrings(_communicationHandler.WriteInterfaceComposite, 0, 10));
                    break;
                case "System.Windows.Controls.ComboBoxItem: *.csv":
                    var csvWriter = new OutputCsvWriter();
                    csvWriter.CreateOutput("out", csvWriter.InterfaceToStrings(_communicationHandler.WriteInterfaceComposite, 0, 10));
                    break;
                case "System.Windows.Controls.ComboBoxItem: *.xls":
                    var xlsWriter = new OutputXlsWriter();
                    xlsWriter.CreateOutput("out", xlsWriter.InterfaceToStrings(_communicationHandler.WriteInterfaceComposite, 0, 10));
                    break;
            }
        }

        #endregion

        #region GUI Parameters Handleing

        private void IpAddressBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox) sender;
            _guiPlcConfiguration.PlcIpAddress = box.Text;
        }

        private void PortBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcPortNumber = Convert.ToInt32(box.Text);
        }

        private void RackBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcRackNumber = Convert.ToInt32(box.Text);
        }

        private void SlotBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcSlotNumber = Convert.ToInt32(box.Text);
        }

        private void ReadDbNumberBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcReadDbNumber = Convert.ToInt32(box.Text);
        }

        private void ReadStartAddressBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcReadStartAddress = Convert.ToInt32(box.Text);
        }

        private void ReadLengthBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcReadLength = Convert.ToInt32(box.Text);
        }

        private void WriteDbNumberBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcWriteDbNumber = Convert.ToInt32(box.Text);
        }

        private void WriteStartAddressBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcWriteStartAddress = Convert.ToInt32(box.Text);
        }

        private void WriteLengthBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var box = (TextBox)sender;
            _guiPlcConfiguration.PlcWriteLength = Convert.ToInt32(box.Text);
        }

        private void VFlashControlModeChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            var box = (CheckBox)sender;
            _vFlash.PcControlMode = true;
            box.IsChecked = _vFlash.PcControlMode;
        }

        private void UpdateLog(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            LogListBox.Dispatcher.BeginInvoke((new Action(() => Logger.DumpLog(LogListBox))));
        }

        #endregion

        #region Auxiliaries

        private void ActualConfigurationHandler(PlcCommunicatorBase.PlcConfig configuration)
        {
            ActIpAddressLabel.Dispatcher.BeginInvoke((new Action(delegate { ActIpAddressLabel.Content = configuration.PlcIpAddress; })));
            ActPortLabel.Dispatcher.BeginInvoke((new Action(delegate { ActPortLabel.Content = configuration.PlcPortNumber; })));
            ActRackLabel.Dispatcher.BeginInvoke((new Action(delegate { ActRackLabel.Content = configuration.PlcRackNumber; })));
            ActSlotLabel.Dispatcher.BeginInvoke((new Action(delegate { ActSlotLabel.Content = configuration.PlcSlotNumber; })));
            ActReadDbNumberLabel.Dispatcher.BeginInvoke((new Action(delegate { ActReadDbNumberLabel.Content = configuration.PlcReadDbNumber; })));
            ActReadStartAddressLabel.Dispatcher.BeginInvoke((new Action(delegate { ActReadStartAddressLabel.Content = configuration.PlcReadStartAddress; })));
            ActReadLengthLabel.Dispatcher.BeginInvoke((new Action(delegate { ActReadLengthLabel.Content = configuration.PlcReadLength; })));
            ActWriteDbNumberLabel.Dispatcher.BeginInvoke((new Action(delegate { ActWriteDbNumberLabel.Content = configuration.PlcWriteDbNumber; })));
            ActWriteStartAddressLabel.Dispatcher.BeginInvoke((new Action(delegate { ActWriteStartAddressLabel.Content = configuration.PlcWriteStartAddress; })));
            ActWriteLengthLabel.Dispatcher.BeginInvoke((new Action(delegate { ActWriteLengthLabel.Content = configuration.PlcWriteLength; })));
        }

        private void StatusBarHandler(PlcCommunicator communication)
        {
            string statusBar = "0";
            Brush brush = Brushes.Red;
            if (communication.ConfigurationStatus != 1)
            {
                statusBar = "Wrong Configuration";
                brush = Brushes.Red;
            }
            if (communication.ConfigurationStatus == 1)
            {
                statusBar = "Configuration verified, ready to connect."; 
                brush = Brushes.Black;
            }
            if (communication.ConnectionStatus == 1)
            {
                statusBar = "Connected to IP address " +_plcCommunication.PlcConfiguration.PlcIpAddress;
                brush = Brushes.Green;
                ConnectButton.Dispatcher.BeginInvoke((new Action(delegate { ConnectButton.Content = "Disconnect"; })));
            }
            if (communication.ConnectionStatus == -2)
            {
                statusBar = "A connection with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress + " was interrupted.";
                brush = Brushes.Red;
                ConnectButton.Dispatcher.BeginInvoke((new Action(delegate { ConnectButton.Content = "Connect"; })));
            }
            PlcStatusLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                PlcStatusLabel.Content = statusBar;
                PlcStatusLabel.Foreground = brush;
            })));
        }

        private void OnlineDataDisplayHandler(PlcCommunicator communication)
        {
            if (communication.ConnectionStatus == 1)
            {
                DataDisplay.Display(OnlineReadDataListBox,OnlineWriteDataListBox,communication,_communicationHandler);
            }
        }

        private void VFlashDisplayHandler(VFlashHandler vector)
        {
            string path = "File path is not specified";
            string status;
            Brush colourBrush;

            var channel = vector.ReturnChannelSetup(1);
            if (channel == null) { return; }
            switch (channel.Status)
            {
                case "Created":
                    status = "Channel created";
                    colourBrush = Brushes.Black;
                    break;
                case "Loading":
                    status = "Loading ...";
                    colourBrush = Brushes.Black;
                    break;
                case "Loaded":
                    status = "Project was loaded succesfully";
                    colourBrush = Brushes.Green;
                    VFlash1LoadButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1LoadButton.IsEnabled = false; })));
                    VFlash1UnloadButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1UnloadButton.IsEnabled = true; })));
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.IsEnabled = true; })));
                    break;
                case "Unloading":
                    status = "Unloading ...";
                    colourBrush = Brushes.Black;
                    break;
                case "Unloaded":
                    status = "Project was unloaded succesfully";
                    colourBrush = Brushes.Green;
                    VFlash1LoadButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1LoadButton.IsEnabled = true; })));
                    VFlash1UnloadButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1UnloadButton.IsEnabled = false; })));
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.IsEnabled = false; })));
                    break;
                case "Flashing":
                    status = "Flashing ...";
                    colourBrush = Brushes.Black;
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.Content = "Abort"; })));
                    break;
                case "Aborting":
                    status = "Flash Aborting ...";
                    colourBrush = Brushes.Red;
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.Content = "Abort"; })));
                    break;
                case "Flashed":
                    status = "Flashing succeed";
                    colourBrush = Brushes.Green;
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.Content = "Flash"; })));
                    break;
                default:
                    status = channel.Status;
                    colourBrush = Brushes.Red;
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.Content = "Flash"; })));
                    break;
            }

            if (channel.FlashProjectPath != "")
            {
                string[] words = channel.FlashProjectPath.Split('\\');
                path = words[words.GetLength(0) - 1];
            }
            VFlash1ProjectPathLabel.Dispatcher.BeginInvoke((new Action(delegate { VFlash1ProjectPathLabel.Content = path; })));
            VFlash1StatusLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                VFlash1StatusLabel.Content = status;
                VFlash1StatusLabel.Foreground = colourBrush;
            })));

            VFlashStatusHandler(1, colourBrush);
        }

        private void VFlashStatusHandler(uint chanId, Brush colourBrush)
        {
            var channel = _vFlash.ReturnChannelSetup(chanId);
            
            switch (chanId)
            {
                case 1:
                    VFlashChannel1StatusLabel.Dispatcher.BeginInvoke((new Action(delegate
                    {
                        VFlashChannel1StatusLabel.Content = "vFlash channel " + channel.ChannelId + ": " + channel.Status;
                        VFlashChannel1StatusLabel.Foreground = colourBrush;
                    })));
                    VFlash1TimeLabel.Dispatcher.BeginInvoke((new Action(delegate { VFlash1TimeLabel.Content = "Remaining: " + channel.RemainingTimeInSecs + "sec"; })));
                    VFlash1ProgressBar.Dispatcher.BeginInvoke((new Action(delegate { VFlash1ProgressBar.Value = channel.ProgressPercentage; })));
                    break;
            }
        }

        private void UpdateSettings()
        {
            IpAddressBox.Text = _guiPlcConfiguration.PlcIpAddress;
            PortBox.Text = _guiPlcConfiguration.PlcPortNumber.ToString(CultureInfo.InvariantCulture);
            SlotBox.Text = _guiPlcConfiguration.PlcSlotNumber.ToString(CultureInfo.InvariantCulture);
            RackBox.Text = _guiPlcConfiguration.PlcRackNumber.ToString(CultureInfo.InvariantCulture);
            ReadDbAddressBox.Text = _guiPlcConfiguration.PlcReadDbNumber.ToString(CultureInfo.InvariantCulture);
            ReadDbStartAddressBox.Text = _guiPlcConfiguration.PlcReadStartAddress.ToString(CultureInfo.InvariantCulture);
            ReadDbLengthBox.Text = _guiPlcConfiguration.PlcReadLength.ToString(CultureInfo.InvariantCulture);
            WriteDbAddressBox.Text = _guiPlcConfiguration.PlcWriteDbNumber.ToString(CultureInfo.InvariantCulture);
            WriteDbStartAddressBox.Text = _guiPlcConfiguration.PlcWriteStartAddress.ToString(CultureInfo.InvariantCulture);
            WriteDbLengthBox.Text = _guiPlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

    }
}
