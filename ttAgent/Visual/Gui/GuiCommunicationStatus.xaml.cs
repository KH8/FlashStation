using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.Log;
using _ttAgent.PLC;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiCommunicationStatus.xaml
    /// </summary>
    public partial class GuiCommunicationStatus
    {
        private readonly PlcCommunicator _plcCommunicator;
        private readonly PlcConfigurationFile _plcConfigurationFile;

        private readonly Thread _updateThread;

        public GuiCommunicationStatus(PlcCommunicator plcCommunicator)
        {
            _plcCommunicator = plcCommunicator;
            _plcConfigurationFile = _plcCommunicator.PlcConfigurationFile;

            InitializeComponent();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();

            StartUpConnectionControlBox.IsChecked = _plcCommunicator.PlcConfigurationFile.ConnectAtStartUp[_plcCommunicator.Header.Id];
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                GuiFactory.UpdateLabel(ActIpAddressLabel, _plcCommunicator.PlcConfiguration.PlcIpAddress);
                GuiFactory.UpdateLabel(ActPortLabel,
                    _plcCommunicator.PlcConfiguration.PlcPortNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(ActRackLabel,
                    _plcCommunicator.PlcConfiguration.PlcRackNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(ActSlotLabel,
                    _plcCommunicator.PlcConfiguration.PlcSlotNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(ActReadDbNumberLabel,
                    _plcCommunicator.PlcConfiguration.PlcReadDbNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(ActReadStartAddressLabel,
                    _plcCommunicator.PlcConfiguration.PlcReadStartAddress.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(ActReadLengthLabel,
                    _plcCommunicator.PlcConfiguration.PlcReadLength.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(ActWriteDbNumberLabel,
                    _plcCommunicator.PlcConfiguration.PlcWriteDbNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(ActWriteStartAddressLabel,
                    _plcCommunicator.PlcConfiguration.PlcWriteStartAddress.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(ActWriteLengthLabel,
                    _plcCommunicator.PlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture));
                ConnectButton.Dispatcher.BeginInvoke((new Action(delegate
                {
                    ConnectButton.Content = "Connect";
                    if (_plcCommunicator.ConnectionStatus == 1) { ConnectButton.Content = "Disconnect"; }
                })));
                Thread.Sleep(21);
            }
        }

        public void ConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            if (_plcCommunicator == null) return;
            try
            {
                if (_plcCommunicator.ConnectionStatus != 1)
                {
                    Logger.Log("ID: " + _plcCommunicator.Header.Id + " : Connection requested by the user");
                    _plcCommunicator.OpenConnection();
                }
                else
                {
                    Logger.Log("ID: " + _plcCommunicator.Header.Id + " : Disconnection requested by the user");
                    _plcCommunicator.CloseConnection();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Connection Failed");
            }
        }

        private void ConnectionAtStartUpChecked(object sender, RoutedEventArgs e)
        {
            var startUpConnectionControlBox = (CheckBox)sender;
            if (startUpConnectionControlBox.IsChecked != null)
            {
                _plcConfigurationFile.ConnectAtStartUp[_plcCommunicator.Header.Id] = (bool)startUpConnectionControlBox.IsChecked;
                _plcConfigurationFile.Save();
            }
        }
    }
}
