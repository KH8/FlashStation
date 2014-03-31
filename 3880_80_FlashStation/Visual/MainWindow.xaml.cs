using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vector.vFlash.Automation;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.PLC;
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

            _vFlash = new VFlashHandler(_communicationHandler.ReadInterfaceComposite, _communicationHandler.WriteInterfaceComposite, UpdateProgress, UpdateStatus);
            foreach (VFlashTypeComponent type in _vFlash.VFlashTypeBank.Children) { VFlashBankListBox.Items.Add("Type: " + type.Type + " : " + "File: " + type.Path); }

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
                    VectorDisplayHandler(_vFlash);
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
            Environment.Exit(0);
        }

        private void StoreSettings(object sender, RoutedEventArgs e)
        {
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
                        LogListBox.Items.Add(DateTime.Now + " connected with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress);
                    }
                    else
                    {
                        _plcCommunication.CloseConnection();
                        ConnectButton.Dispatcher.BeginInvoke((new Action(delegate { ConnectButton.Content = "Connect"; })));
                        LogListBox.Items.Add(DateTime.Now + " disconnected with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress);
                    } 
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Connection Failed");
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
                        }
                        catch (Exception exception) { MessageBox.Show(exception.Message, "Project Loading Failed"); }
                        LogListBox.Items.Add(DateTime.Now + " project has been loaded to the channel nr 1");
                        //todo: logging sucks
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
                    try {_vFlash.UnloadProject(1);}
                    catch (Exception exception) { MessageBox.Show(exception.Message, "Project Unloading Failed"); }
                    LogListBox.Items.Add(DateTime.Now + " project has been Unloaded from the channel nr 1");
                    //todo: logging sucks
                    break;
                //todo: implement for the others
            }
        }

        private void FlashVFlashProject(object sender, RoutedEventArgs e)
        {
            var flashButton = (Button)sender;
            switch (flashButton.Name)
            {
                case "VFlash1UnloadButton":
                    var channel = _vFlash.ReturnChannelSetup(1);
                    if (channel.Status == "Flashing")
                    {
                        try
                        {
                            _vFlash.AbortFlashing(1);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Flash Abort Failed");
                        }
                        LogListBox.Items.Add(DateTime.Now + " project flashing on the channel nr 1 aborted");
                        //todo: logging sucks
                    }
                    if (channel.Status != "Flashing")
                    {
                        try
                        {
                            _vFlash.StartFlashing(1);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Project Flashing Failed");
                        }
                        LogListBox.Items.Add(DateTime.Now + " project flashing on the channel nr 1 started");
                        //todo: logging sucks
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
        }

        private void TypeCreation(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".vflashpack", Filter = "Flash Project (.vflashpack)|*.vflashpack" };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true) { _vFlash.VFlashTypeBank.Add(new VFlashTypeComponent(Convert.ToUInt16(TypeNumberBox.Text), dlg.FileName));}

            VFlashTypeBankFile.Default.TypeBank.Clear();
            foreach (VFlashType type in _vFlash.VFlashTypeBank.Children) { VFlashTypeBankFile.Default.TypeBank.Add(type.Type,type.Path); }
            VFlashTypeBankFile.Default.Save();

            VFlashBankListBox.Items.Clear();
            foreach (VFlashTypeComponent type in _vFlash.VFlashTypeBank.Children) { VFlashBankListBox.Items.Add("Type: " + type.Type + " : " + "File: " + type.Path); }
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
            StatusLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                StatusLabel.Content = statusBar;
                StatusLabel.Foreground = brush;
            })));
        }

        private void OnlineDataDisplayHandler(PlcCommunicator communication)
        {
            if (communication.ConnectionStatus == 1)
            {
                DataDisplay.Display(OnlineReadDataListBox,OnlineWriteDataListBox,communication,_communicationHandler);
            }
        }

        private void VectorDisplayHandler(VFlashHandler vector)
        {
            string path;
            string status;
            Brush colourBrush;

            var channel = vector.ReturnChannelSetup(1);
            if (channel == null) { return; }
            switch (channel.Status)
            {
                case "Created":
                    path = "No project loaded yet.";
                    status = "Channel created";
                    colourBrush = Brushes.Black;
                    break;
                case "Loading":
                    path = "Channel is not activated";
                    status = "Loading ...";
                    colourBrush = Brushes.Black;
                    break;
                case "Loaded":
                    path = channel.FlashProjectPath;
                    status = "Project was loaded succesfully";
                    colourBrush = Brushes.Green;

                    VFlash1UnloadButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1UnloadButton.IsEnabled = true; })));
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.IsEnabled = true; })));
                    break;
                case "Unloading":
                    path = channel.FlashProjectPath;
                    status = "Unloading ...";
                    colourBrush = Brushes.Black;
                    break;
                case "Unloaded":
                    path = "Channel is not activated";
                    status = "Project was unloaded succesfully";
                    colourBrush = Brushes.Green;

                    VFlash1UnloadButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1UnloadButton.IsEnabled = false; })));
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.IsEnabled = false; })));
                    break;
                case "Flashing":
                    path = channel.FlashProjectPath;
                    VFlash1ProjectPathLabel.Dispatcher.BeginInvoke((new Action(delegate{ VFlash1ProjectPathLabel.Content = path; })));
                    status = VFlash1StatusLabel.Content.ToString();
                    colourBrush = Brushes.Black;

                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.Content = "Abort"; })));
                    break;
                case "Aborting":
                    path = channel.FlashProjectPath;
                    status = "Flash Aborting ...";
                    colourBrush = Brushes.Red;
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.Content = "Abort"; })));
                    break;
                case "Flashed":
                    path = channel.FlashProjectPath;
                    status = "Flashing succeed";
                    colourBrush = Brushes.Green;
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.Content = "Flash"; })));
                    break;
                default:
                    path = "Channel is not activated";
                    status = channel.Status;
                    colourBrush = Brushes.Red;
                    VFlash1FlashButton.Dispatcher.BeginInvoke((new Action(delegate { VFlash1FlashButton.Content = "Flash"; })));
                    break;
            }

            VFlash1ProjectPathLabel.Dispatcher.BeginInvoke((new Action(delegate{ VFlash1ProjectPathLabel.Content = path; })));
            VFlash1StatusLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                VFlash1StatusLabel.Content = status;
                VFlash1StatusLabel.Foreground = colourBrush;
            })));
        }

        internal void UpdateProgress(long handle, uint progressInPercent, uint remainingTimeInSecs)
        {
            VFlash1TimeLabel.Dispatcher.BeginInvoke((new Action(delegate { VFlash1TimeLabel.Content = "Remaining: " + remainingTimeInSecs + " sec."; })));
            VFlash1ProgressBar.Dispatcher.BeginInvoke((new Action(delegate
            {
                if (progressInPercent > 100) { VFlash1ProgressBar.Value = 0; }
                else { VFlash1ProgressBar.Value = (int)progressInPercent; }
            })));
        }

        internal void UpdateStatus(long handle, VFlashStationStatus status)
        {
            VFlash1StatusLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                VFlash1StatusLabel.Content = status;
            })));
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
