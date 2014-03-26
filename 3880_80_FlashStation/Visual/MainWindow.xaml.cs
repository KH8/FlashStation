using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private readonly Thread _synchronizationThread;
        private readonly PlcCommunicator _plcCommunication;
        private readonly PlcConfigurator _plcConfiguration;
        private readonly VFlashHandler _vector;
        private PlcCommunicatorBase.PlcConfig _guiPlcConfiguration;
        private readonly CommunicationInterfaceHandler _communicationHandler;

        public MainWindow()
        {
            InitializeComponent();

            _communicationHandler = new CommunicationInterfaceHandler();
            _communicationHandler.Initialize("readInterface");
            _communicationHandler.Initialize("writeInterface");

            OnlineReadDataListBox.Items.Add("Read area: ");
            OnlineWriteDataListBox.Items.Add("Write area: ");

            _plcCommunication = new PlcCommunicator();
            _plcConfiguration = new PlcConfigurator();

            _guiPlcConfiguration = PlcConfigurationFile.Default.Configuration;
            UpdateSettings();

            if (PlcConfigurationFile.Default.Configuration.PlcConfigurationStatus == 1)
            {
                StoreSettings();
            }

            _vector = new VFlashHandler();

            _statusThread = new Thread(StatusHandler);
            _statusThread.SetApartmentState(ApartmentState.STA);
            _statusThread.IsBackground = true;
            _statusThread.Start();

            _synchronizationThread = new Thread(SynchronizationHandler);
            _synchronizationThread.SetApartmentState(ApartmentState.STA);
            _synchronizationThread.IsBackground = true;
            _synchronizationThread.Start();
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
                }
                Thread.Sleep(10);
            }
        }

        private void SynchronizationHandler()
        {
            _vector.OutputInterface = _communicationHandler.WriteInterfaceComposite;

            while (_synchronizationThread.IsAlive)
            {
                _communicationHandler.WriteInterfaceComposite = _vector.OutputInterface;
                if (_plcCommunication != null && _plcCommunication.ConnectionStatus == 1) { _communicationHandler.MaintainConnection(_plcCommunication); }
                _vector.InputInterface = _communicationHandler.ReadInterfaceComposite;
                Thread.Sleep(10);
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
                    }
                    else
                    {
                        _plcCommunication.CloseConnection();
                        ConnectButton.Dispatcher.BeginInvoke((new Action(delegate { ConnectButton.Content = "Connect"; })));
                    } 
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Connection Failed");
            }
        }

        #endregion

        #region GUI Parameters Handleing

        private void IpAddressBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox) sender;
            _guiPlcConfiguration.PlcIpAddress = box.Text;
        }

        private void PortBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
            _guiPlcConfiguration.PlcPortNumber = Convert.ToInt32(box.Text);
        }

        private void RackBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
            _guiPlcConfiguration.PlcRackNumber = Convert.ToInt32(box.Text);
        }

        private void SlotBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
            _guiPlcConfiguration.PlcSlotNumber = Convert.ToInt32(box.Text);
        }

        private void ReadDbNumberBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
            _guiPlcConfiguration.PlcReadDbNumber = Convert.ToInt32(box.Text);
        }

        private void ReadStartAddressBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
            _guiPlcConfiguration.PlcReadStartAddress = Convert.ToInt32(box.Text);
        }

        private void ReadLengthBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
            _guiPlcConfiguration.PlcReadLength = Convert.ToInt32(box.Text);
        }

        private void WriteDbNumberBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
            _guiPlcConfiguration.PlcWriteDbNumber = Convert.ToInt32(box.Text);
        }

        private void WriteStartAddressBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
            _guiPlcConfiguration.PlcWriteStartAddress = Convert.ToInt32(box.Text);
        }

        private void WriteLengthBoxChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            TextBox box = (TextBox)sender;
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
