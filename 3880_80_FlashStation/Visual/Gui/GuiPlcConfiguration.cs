using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiPlcConfiguration : Gui
    {
        public Grid GeneralGrid;

        public override void Initialize(uint id, int xPosition, int yPosition)
        {
            Id = id;
            XPosition = xPosition;
            YPosition = yPosition;

            GeneralGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Center, VerticalAlignment.Top, 280);
            GeneralGrid.Background = Brushes.Aqua;

            var guiCommunicationGroupBox = GuiFactory.CreateGroupBox("PLC Communication Setup", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 150, 335);
            GeneralGrid.Children.Add(guiCommunicationGroupBox);

            var guiCommunicationGrid = GuiFactory.CreateGrid();
            guiCommunicationGroupBox.Content = guiCommunicationGrid;

            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("IP Address:", 68, 10, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("Port:", 68, 37, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("Rack:", 68, 64, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("Slot:", 68, 91, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateTextBox("IpAddressBox", "192.168.10.80", 148, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, IpAddressBoxChanged));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateTextBox("PortBox", "102", 148, 37, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, PortBoxChanged));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateTextBox("RackBox", "0", 148, 64, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, RackBoxChanged));
            guiCommunicationGrid.Children.Add(GuiFactory.CreateTextBox("SlotBox", "0", 148, 91, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, SlotBoxChanged));

            var guiDataGroupBox = GuiFactory.CreateGroupBox("PLC Data Setup", 339, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 205, 335);
            GeneralGrid.Children.Add(guiDataGroupBox);

            var guiDataGrid = GuiFactory.CreateGrid();
            guiDataGroupBox.Content = guiDataGrid;

            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Read DB Number:", 63, 10, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Read Start Address:", 63, 37, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Read Data Length:", 63, 64, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Write DB Number:", 63, 92, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("WriteDbStartAddressBox", 63, 119, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateLabel("Write Data Length:", 63, 149, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("ReadDbAddressBox", "1000", 148, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, ReadDbNumberBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("ReadDbStartAddressBox", "0", 148, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, ReadStartAddressBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("ReadDbLengthBox", "100", 148, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, ReadLengthBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("WriteDbAddressBox", "1000", 148, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, WriteDbNumberBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("WriteDbStartAddressBox", "512", 148, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, WriteStartAddressBoxChanged));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("WriteDbLengthBox", "100", 148, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, WriteLengthBoxChanged));

            var guiInterfaceGroupBox = GuiFactory.CreateGroupBox("Interface Configuration", 0, 148, HorizontalAlignment.Left, VerticalAlignment.Top, 58, 335);
            GeneralGrid.Children.Add(guiInterfaceGroupBox);

            var guiInterfaceGrid = GuiFactory.CreateGrid();
            guiInterfaceGroupBox.Content = guiInterfaceGrid;

            guiInterfaceGrid.Children.Add(GuiFactory.CreateLabel("Configuration File:", 31, 5, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 112));
            guiDataGrid.Children.Add(GuiFactory.CreateTextBox("InterfacePathBox", "File not loaded", 148, 5, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 165));

            GeneralGrid.Children.Add(GuiFactory.CreateButton("UseSettingsButton", "Use Settings", 0, 211, HorizontalAlignment.Right, VerticalAlignment.Top, 25, 100, StoreSettings));
            GeneralGrid.Children.Add(GuiFactory.CreateButton("LoadFileButton", "Load File", 339, 211, HorizontalAlignment.Right, VerticalAlignment.Top, 25, 100, LoadSettingFile));
        }

        private void StoreSettings(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void LoadSettingFile(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public override void MakeVisible(uint id)
        {
            GeneralGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible(uint id)
        {
            GeneralGrid.Visibility = Visibility.Hidden;
        }

        private static void IpAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private static void PortBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void RackBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void SlotBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void WriteLengthBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void WriteStartAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void WriteDbNumberBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ReadLengthBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ReadStartAddressBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ReadDbNumberBoxChanged(object sender, TextChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
