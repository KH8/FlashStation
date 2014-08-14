using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _PlcAgent.Log;
using _PlcAgent.PLC;

namespace _PlcAgent.Visual.Gui.PLC
{
    /// <summary>
    /// Interaction logic for GuiPlcCommunicatorStatus.xaml
    /// </summary>
    public partial class GuiPlcCommunicatorStatus
    {
        #region Constructors

        public GuiPlcCommunicatorStatus(PlcCommunicator plcCommunicator)
            : base(plcCommunicator)
        {
            InitializeComponent();
            StartUpConnectionControlBox.IsChecked =
                PlcCommunicator.PlcConfigurationFile.ConnectAtStartUp[PlcCommunicator.Header.Id];

            OnConnectionStatusChanged();
            OnConfigurationStatusChanged();
        }

        #endregion


        #region Event Handlers

        protected override sealed void OnConnectionStatusChanged()
        {
            ConnectButton.Dispatcher.BeginInvoke((new Action(delegate
            {
                ConnectButton.Content = "Connect";
                if (PlcCommunicator.ConnectionStatus == 1)
                {
                    ConnectButton.Content = "Disconnect";
                }
            })));
        }

        protected override sealed void OnConfigurationStatusChanged()
        {
            UpdateLabel(ActIpAddressLabel, PlcCommunicator.PlcConfiguration.PlcIpAddress);
            UpdateLabel(ActPortLabel,
                PlcCommunicator.PlcConfiguration.PlcPortNumber.ToString(CultureInfo.InvariantCulture));
            UpdateLabel(ActRackLabel,
                PlcCommunicator.PlcConfiguration.PlcRackNumber.ToString(CultureInfo.InvariantCulture));
            UpdateLabel(ActSlotLabel,
                PlcCommunicator.PlcConfiguration.PlcSlotNumber.ToString(CultureInfo.InvariantCulture));
            UpdateLabel(ActReadDbNumberLabel,
                PlcCommunicator.PlcConfiguration.PlcReadDbNumber.ToString(CultureInfo.InvariantCulture));
            UpdateLabel(ActReadStartAddressLabel,
                PlcCommunicator.PlcConfiguration.PlcReadStartAddress.ToString(CultureInfo.InvariantCulture));
            UpdateLabel(ActReadLengthLabel,
                PlcCommunicator.PlcConfiguration.PlcReadLength.ToString(CultureInfo.InvariantCulture));
            UpdateLabel(ActWriteDbNumberLabel,
                PlcCommunicator.PlcConfiguration.PlcWriteDbNumber.ToString(CultureInfo.InvariantCulture));
            UpdateLabel(ActWriteStartAddressLabel,
                PlcCommunicator.PlcConfiguration.PlcWriteStartAddress.ToString(CultureInfo.InvariantCulture));
            UpdateLabel(ActWriteLengthLabel,
                PlcCommunicator.PlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture));
        }

        public void ConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlcCommunicator == null) return;
            try
            {
                if (PlcCommunicator.ConnectionStatus != 1)
                {
                    Logger.Log("ID: " + PlcCommunicator.Header.Id + " : Connection requested by the user");
                    PlcCommunicator.OpenConnection();
                }
                else
                {
                    Logger.Log("ID: " + PlcCommunicator.Header.Id + " : Disconnection requested by the user");
                    PlcCommunicator.CloseConnection();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Connection Failed");
            }
        }

        private void ConnectionAtStartUpChecked(object sender, RoutedEventArgs e)
        {
            var startUpConnectionControlBox = (CheckBox) sender;
            if (startUpConnectionControlBox.IsChecked == null) return;
            PlcCommunicator.PlcConfigurationFile.ConnectAtStartUp[PlcCommunicator.Header.Id] =
                (bool) startUpConnectionControlBox.IsChecked;
            PlcCommunicator.PlcConfigurationFile.Save();
        }

        private static void UpdateLabel(ContentControl label, string text)
        {
            label.Dispatcher.BeginInvoke((new Action(delegate
            {
                label.Content = text;
            })));
        }

        #endregion

    }
}
