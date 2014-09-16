using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _PlcAgent.Log;
using _PlcAgent.PLC;

namespace _PlcAgent.Visual.Gui.PLC
{
    /// <summary>
    /// Interaction logic for GuiPlcCommunicatorConfiguration.xaml
    /// </summary>
    public partial class GuiPlcCommunicatorConfiguration
    {
        #region Constructors

        public GuiPlcCommunicatorConfiguration(PlcCommunicator plcCommunication)
            : base(plcCommunication)
        {
            if (PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcConfigurationStatus == 1)
            { StoreSettings();}

            InitializeComponent();

            IpAddressBox.Text = PlcCommunicator.PlcConfiguration.PlcIpAddress;
            PortBox.Text = PlcCommunicator.PlcConfiguration.PlcPortNumber.ToString(CultureInfo.InvariantCulture);
            RackBox.Text = PlcCommunicator.PlcConfiguration.PlcRackNumber.ToString(CultureInfo.InvariantCulture);
            SlotBox.Text = PlcCommunicator.PlcConfiguration.PlcSlotNumber.ToString(CultureInfo.InvariantCulture);

            ReadDbAddressBox.Text =
                PlcCommunicator.PlcConfiguration.PlcReadDbNumber.ToString(CultureInfo.InvariantCulture);
            ReadDbStartAddressBox.Text =
                PlcCommunicator.PlcConfiguration.PlcReadStartAddress.ToString(CultureInfo.InvariantCulture);
            ReadDbLengthBox.Text = PlcCommunicator.PlcConfiguration.PlcReadLength.ToString(CultureInfo.InvariantCulture);
            WriteDbAddressBox.Text =
                PlcCommunicator.PlcConfiguration.PlcWriteDbNumber.ToString(CultureInfo.InvariantCulture);
            WriteDbStartAddressBox.Text =
                PlcCommunicator.PlcConfiguration.PlcWriteStartAddress.ToString(CultureInfo.InvariantCulture);
            WriteDbLengthBox.Text =
                PlcCommunicator.PlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture);

            if (PlcCommunicator.ConnectionStatus != -1) UseSettingsButton.IsEnabled = false;
        }

        #endregion


        #region Methods

        public void StoreSettings()
        {
            PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcConfigurationStatus = 1;
            PlcCommunicator.PlcConfigurationFile.Save();
            PlcCommunicator.SetupConnection(
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id]);
        }

        #endregion


        #region Event Handlers

        protected override void OnConnectionStatusChanged()
        {
            UseSettingsButton.Dispatcher.BeginInvoke((new Action(delegate
            {
                UseSettingsButton.IsEnabled = true;
                if (PlcCommunicator.ConnectionStatus != -1) UseSettingsButton.IsEnabled = false;
            })));
        }

        protected override void OnConfigurationStatusChanged()
        {}

        private void StoreSettingsButton(object sender, RoutedEventArgs e)
        {
            Logger.Log("Communication configuration has been changed by the user");
            StoreSettings();
        }

        private void IpAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcIpAddress = box.Text;
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcIpAddress = "0";
            }
        }

        private void PortBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcPortNumber =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcPortNumber = 0;
            }
        }

        private void RackBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcRackNumber =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcRackNumber = 0;
            }
        }

        private void SlotBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcSlotNumber =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcSlotNumber = 0;
            }
        }

        private void WriteLengthBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcWriteLength =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcWriteLength = 0;
            }
        }

        private void WriteStartAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcWriteStartAddress =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcWriteStartAddress = 0;
            }
        }

        private void WriteDbNumberBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcWriteDbNumber =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcWriteDbNumber = 0;
            }
        }

        private void ReadLengthBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcReadLength =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcReadLength = 0;
            }
        }

        private void ReadStartAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcReadStartAddress =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcReadStartAddress = 0;
            }
        }

        private void ReadDbNumberBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;
            try
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcReadDbNumber =
                    Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                PlcCommunicator.PlcConfigurationFile.Configuration[PlcCommunicator.Header.Id].PlcReadDbNumber = 0;
            }
        }

        #endregion

    }
}
