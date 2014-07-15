using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.Log;
using _ttAgent.PLC;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiPlcConfiguration.xaml
    /// </summary>
    public partial class GuiPlcConfiguration
    {
        private readonly PlcCommunicator _plcCommunication;
        private PlcCommunicator.PlcConfig _guiPlcConfiguration;
        private readonly PlcConfigurationFile _plcConfigurationFile;

        public GuiPlcConfiguration(PlcCommunicator plcCommunication)
        {
            _plcCommunication = plcCommunication;
            _plcConfigurationFile = _plcCommunication.PlcConfigurationFile;
            _guiPlcConfiguration = _plcConfigurationFile.Configuration[_plcCommunication.Header.Id];

            if (PlcConfigurationFile.Default.Configuration[_plcCommunication.Header.Id].PlcConfigurationStatus == 1) { StoreSettings(); }
            
            InitializeComponent();

            IpAddressBox.Text = _plcCommunication.PlcConfiguration.PlcIpAddress;
            PortBox.Text = _plcCommunication.PlcConfiguration.PlcPortNumber.ToString(CultureInfo.InvariantCulture);
            RackBox.Text = _plcCommunication.PlcConfiguration.PlcRackNumber.ToString(CultureInfo.InvariantCulture);
            SlotBox.Text = _plcCommunication.PlcConfiguration.PlcSlotNumber.ToString(CultureInfo.InvariantCulture);

            ReadDbAddressBox.Text = _plcCommunication.PlcConfiguration.PlcReadDbNumber.ToString(CultureInfo.InvariantCulture);
            ReadDbStartAddressBox.Text = _plcCommunication.PlcConfiguration.PlcReadStartAddress.ToString(CultureInfo.InvariantCulture);
            ReadDbLengthBox.Text = _plcCommunication.PlcConfiguration.PlcReadLength.ToString(CultureInfo.InvariantCulture);
            WriteDbAddressBox.Text = _plcCommunication.PlcConfiguration.PlcWriteDbNumber.ToString(CultureInfo.InvariantCulture);
            WriteDbStartAddressBox.Text = _plcCommunication.PlcConfiguration.PlcWriteStartAddress.ToString(CultureInfo.InvariantCulture);
            WriteDbLengthBox.Text = _plcCommunication.PlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture);
        }

        private void StoreSettingsButton(object sender, RoutedEventArgs e)
        {
            Logger.Log("Communication configuration has been changed by the user");
            StoreSettings();
        }

        public void StoreSettings()
        {
            _guiPlcConfiguration.PlcConfigurationStatus = 1;
            _plcConfigurationFile.Configuration[_plcCommunication.Header.Id] = _guiPlcConfiguration;
            _plcConfigurationFile.Save();
            _plcCommunication.SetupConnection(_plcConfigurationFile.Configuration[_plcCommunication.Header.Id]);
        }

        private void IpAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcIpAddress = box.Text; }
            catch (Exception) { _guiPlcConfiguration.PlcIpAddress = "0"; }
        }

        private void PortBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcPortNumber = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcPortNumber = 0; }
        }

        private void RackBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcRackNumber = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcRackNumber = 0; }
        }

        private void SlotBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcSlotNumber = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcSlotNumber = 0; }
        }

        private void WriteLengthBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcWriteLength = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcWriteLength = 0; }
        }

        private void WriteStartAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcWriteStartAddress = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcWriteStartAddress = 0; }
        }

        private void WriteDbNumberBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcWriteDbNumber = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcWriteDbNumber = 0; }
        }

        private void ReadLengthBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcReadLength = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcReadLength = 0; }
        }

        private void ReadStartAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcReadStartAddress = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcReadStartAddress = 0; }
        }

        private void ReadDbNumberBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _guiPlcConfiguration.PlcReadDbNumber = Convert.ToInt32(box.Text); }
            catch (Exception) { _guiPlcConfiguration.PlcReadDbNumber = 0; }
        }
    }
}
