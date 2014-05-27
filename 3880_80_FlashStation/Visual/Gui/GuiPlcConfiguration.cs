using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiPlcConfiguration : Gui
    {
        private Grid _generalGrid;

        private readonly PlcCommunicator _plcCommunication;
        private PlcCommunicatorBase.PlcConfig _guiPlcConfiguration;
        private readonly PlcConfigurationFile _plcConfigurationFile;

        private TextBox _interfacePathBox = new TextBox();

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiPlcConfiguration(uint id, PlcCommunicator plcCommunication, PlcConfigurationFile plcConfigurationFile)
        {
            Id = id;

            _plcCommunication = plcCommunication;
            _plcConfigurationFile = plcConfigurationFile;
            _guiPlcConfiguration = _plcConfigurationFile.Configuration[Id];

            if (PlcConfigurationFile.Default.Configuration[Id].PlcConfigurationStatus == 1) { StoreSettings(); }
        }

        public override void Initialize(int xPosition, int yPosition)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Center, VerticalAlignment.Top, 240, 800);

            var guiCommunicationGroupBox = GuiFactory.CreateGroupBox("PLC Communication Setup", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 150, 398);
            _generalGrid.Children.Add(guiCommunicationGroupBox);

            var guiCommunicationGrid = GuiFactory.CreateGrid();
            guiCommunicationGroupBox.Content = guiCommunicationGrid;

            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("IP Address:", 68, 10, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("Port:", 68, 37, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("Rack:", 68, 64, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("Slot:", 68, 91, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateTextBox("IpAddressBox", _guiPlcConfiguration.PlcIpAddress, 180, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, IpAddressBoxChanged));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateTextBox("PortBox", _guiPlcConfiguration.PlcPortNumber.ToString(CultureInfo.InvariantCulture), 180, 37, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, PortBoxChanged));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateTextBox("RackBox", _guiPlcConfiguration.PlcRackNumber.ToString(CultureInfo.InvariantCulture), 180, 64, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, RackBoxChanged));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateTextBox("SlotBox", _guiPlcConfiguration.PlcSlotNumber.ToString(CultureInfo.InvariantCulture), 180, 91, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, SlotBoxChanged));

            var guiDataGroupBox = GuiFactory.CreateGroupBox("PLC Data Setup", 0, 0, HorizontalAlignment.Right, VerticalAlignment.Top, 206, 398);
            _generalGrid.Children.Add(guiDataGroupBox);

            var guiDataGrid = GuiFactory.CreateGrid();
            guiDataGroupBox.Content = guiDataGrid;

            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Read DB Number:", 63, 10, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Read Start Address:", 63, 37, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Read Data Length:", 63, 64, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Write DB Number:", 63, 92, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("WriteDbStartAddressBox", 63, 119, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Write Data Length:", 63, 149, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("ReadDbAddressBox", _guiPlcConfiguration.PlcReadDbNumber.ToString(CultureInfo.InvariantCulture), 200, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, ReadDbNumberBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("ReadDbStartAddressBox", _guiPlcConfiguration.PlcReadStartAddress.ToString(CultureInfo.InvariantCulture), 200, 37, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, ReadStartAddressBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("ReadDbLengthBox", _guiPlcConfiguration.PlcReadLength.ToString(CultureInfo.InvariantCulture), 200, 64, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, ReadLengthBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("WriteDbAddressBox", _guiPlcConfiguration.PlcWriteDbNumber.ToString(CultureInfo.InvariantCulture), 200, 92, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, WriteDbNumberBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("WriteDbStartAddressBox", _guiPlcConfiguration.PlcWriteStartAddress.ToString(CultureInfo.InvariantCulture), 200, 119, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, WriteStartAddressBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("WriteDbLengthBox", _guiPlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture), 200, 146, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, WriteLengthBoxChanged));

            _generalGrid.Children.Add(GuiFactory.CreateButton("UseSettingsButton", "Use Settings", 0, 211, HorizontalAlignment.Right, VerticalAlignment.Top, 25, 100, StoreSettingsButton));
        }

        private void StoreSettingsButton(object sender, RoutedEventArgs e)
        {
            Logger.Log("Communication configuration has been changed");
            StoreSettings();
        }

        public void StoreSettings()
        {
            _guiPlcConfiguration.PlcConfigurationStatus = 1;
            _plcConfigurationFile.Configuration[Id] = _guiPlcConfiguration;
            _plcConfigurationFile.Save();
            _plcCommunication.SetupConnection(_plcConfigurationFile.Configuration[Id]);
        }

        public override void MakeVisible()
        {
            _generalGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible()
        {
            _generalGrid.Visibility = Visibility.Hidden;
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
