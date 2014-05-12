using System;
using System.Windows;
using System.Windows.Controls;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiCommunicationStatus : Gui
    {
        public Grid GeneralGrid;
        private PlcCommunicator _plcCommunication;

        public override void Initialize(uint id, int xPosition, int yPosition)
        {
            Id = id;
            XPosition = xPosition;
            YPosition = yPosition;

            GeneralGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Center, VerticalAlignment.Top, 250, 672);

            var guiGroupBox = GuiFactory.CreateGroupBox("PLC Configuration", 0, 0, HorizontalAlignment.Center,
                VerticalAlignment.Top, 206, 314);
            GeneralGrid.Children.Add(guiGroupBox);

            var guiGrid = GuiFactory.CreateGrid();
            guiGroupBox.Content = guiGrid;

            guiGrid.Children.Add(GuiFactory.CreateLabel("IP Address:", 9, -2, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Port:", 9, 16, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Rack:", 9, 33, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Slot:", 9, 50, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Read DB Number:", 9, 68, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Read Start Address:", 9, 86, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Read Data Length:", 9, 104, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Write DB Number:", 9, 122, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Write Start Address:", 9, 140, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Write Data Length:", 9, 158, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));

            guiGrid.Children.Add(GuiFactory.CreateLabel("ActIpAddressLabel", "192.168.100.100", 195, -2, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActPortLabel", "102", 195, 16, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActRackLabel", "0", 195, 33, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActSlotLabel", "0", 195, 50, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActReadDbNumberLabel", "1000", 211, 68, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActReadStartAddressLabel", "0", 211, 86, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActReadLengthLabel", "512", 211, 104, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActWriteDbNumberLabel", "1000", 211, 122, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActWriteStartAddressLabel", "512", 211, 140, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(GuiFactory.CreateLabel("ActWriteLengthLabel", "512", 211, 158, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));

            GeneralGrid.Children.Add(GuiFactory.CreateButton("StartUpConnectionControlBox", "Connect", 0, 212, HorizontalAlignment.Center, VerticalAlignment.Top, 25, 100, ConnectionButtonClick));
            GeneralGrid.Children.Add(GuiFactory.CreateCheckBox("ConnectButton", "Connect at Start Up", 0, 223, HorizontalAlignment.Right, VerticalAlignment.Top, 134, ConnectionAtStartUpChecked));
        }

        public override void MakeVisible(uint id)
        {
            GeneralGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible(uint id)
        {
            GeneralGrid.Visibility = Visibility.Hidden;
        }

        private static void ConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            var connectButton = (Button) sender;

            if (_plcCommunication == null) return;
            try
            {
                if (_plcCommunication.ConnectionStatus != 1)
                {
                    _plcCommunication.OpenConnection();
                    connectButton.Dispatcher.BeginInvoke((new Action(delegate { connectButton.Content = "Disconnect"; })));
                    Logger.Log("Connected with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress);
                }
                else
                {
                    _plcCommunication.CloseConnection();
                    connectButton.Dispatcher.BeginInvoke((new Action(delegate { connectButton.Content = "Connect"; })));
                    Logger.Log("Disconnected with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Connection Failed");
                if (_plcCommunication != null)
                    Logger.Log("Connection trial with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress + " failed");
            }
        }

        private static void ConnectionAtStartUpChecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
