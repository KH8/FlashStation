using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiCommunicationStatus : Gui
    {
        private Grid _generalGrid;

        private readonly PlcCommunicator _plcCommunication;
        private readonly PlcStartUpConnection _plcStartUpConnection;

        private Label _actIpAddressLabel = new Label();
        private Label _actPortLabel = new Label();
        private Label _actRackLabel = new Label();
        private Label _actSlotLabel = new Label();
        private Label _actReadDbNumberLabel = new Label();
        private Label _actReadStartAddressLabel = new Label();
        private Label _actReadLengthLabel = new Label();
        private Label _actWriteDbNumberLabel = new Label();
        private Label _actWriteStartAddressLabel = new Label();
        private Label _actWriteLengthLabel = new Label();

        private CheckBox _startUpConnectionControlBox = new CheckBox();

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiCommunicationStatus(PlcCommunicator plcCommunication, PlcStartUpConnection plcStartUpConnection)
        {
            _plcCommunication = plcCommunication;
            _plcStartUpConnection = plcStartUpConnection;
        }

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

            guiGrid.Children.Add(_actIpAddressLabel = GuiFactory.CreateLabel("ActIpAddressLabel", "", 195, -2, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(_actPortLabel = GuiFactory.CreateLabel("ActPortLabel", "", 195, 16, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(_actRackLabel = GuiFactory.CreateLabel("ActRackLabel", "", 195, 33, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(_actSlotLabel = GuiFactory.CreateLabel("ActSlotLabel", "", 195, 50, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(_actReadDbNumberLabel = GuiFactory.CreateLabel("ActReadDbNumberLabel", "", 211, 68, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actReadStartAddressLabel = GuiFactory.CreateLabel("ActReadStartAddressLabel", "", 211, 86, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actReadLengthLabel = GuiFactory.CreateLabel("ActReadLengthLabel", "", 211, 104, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actWriteDbNumberLabel = GuiFactory.CreateLabel("ActWriteDbNumberLabel", "", 211, 122, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actWriteStartAddressLabel = GuiFactory.CreateLabel("ActWriteStartAddressLabel", "", 211, 140, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actWriteLengthLabel = GuiFactory.CreateLabel("ActWriteLengthLabel", "", 211, 158, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));

            GeneralGrid.Children.Add(GuiFactory.CreateButton("ConnectButton", "Connect", 0, 212, HorizontalAlignment.Center, VerticalAlignment.Top, 25, 100, ConnectionButtonClick));
            GeneralGrid.Children.Add(_startUpConnectionControlBox = GuiFactory.CreateCheckBox("StartUpConnectionControlBox", "Connect at Start Up", 0, 223, HorizontalAlignment.Right, VerticalAlignment.Top, 134, ConnectionAtStartUpChecked));
            _startUpConnectionControlBox.IsChecked = _plcStartUpConnection.ConnectAtStartUp;
        }

        public override void MakeVisible(uint id)
        {
            GeneralGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible(uint id)
        {
            GeneralGrid.Visibility = Visibility.Hidden;
        }

        public void Update()
        {
            GuiFactory.UpdateLabel(_actIpAddressLabel, _plcCommunication.PlcConfiguration.PlcIpAddress);
            GuiFactory.UpdateLabel(_actPortLabel, _plcCommunication.PlcConfiguration.PlcPortNumber.ToString(CultureInfo.InvariantCulture));
            GuiFactory.UpdateLabel(_actRackLabel, _plcCommunication.PlcConfiguration.PlcRackNumber.ToString(CultureInfo.InvariantCulture));
            GuiFactory.UpdateLabel(_actSlotLabel, _plcCommunication.PlcConfiguration.PlcSlotNumber.ToString(CultureInfo.InvariantCulture));
            GuiFactory.UpdateLabel(_actReadDbNumberLabel, _plcCommunication.PlcConfiguration.PlcReadDbNumber.ToString(CultureInfo.InvariantCulture));
            GuiFactory.UpdateLabel(_actReadStartAddressLabel, _plcCommunication.PlcConfiguration.PlcReadStartAddress.ToString(CultureInfo.InvariantCulture));
            GuiFactory.UpdateLabel(_actReadLengthLabel, _plcCommunication.PlcConfiguration.PlcReadLength.ToString(CultureInfo.InvariantCulture));
            GuiFactory.UpdateLabel(_actWriteDbNumberLabel, _plcCommunication.PlcConfiguration.PlcWriteDbNumber.ToString(CultureInfo.InvariantCulture));
            GuiFactory.UpdateLabel(_actWriteStartAddressLabel, _plcCommunication.PlcConfiguration.PlcWriteStartAddress.ToString(CultureInfo.InvariantCulture));
            GuiFactory.UpdateLabel(_actWriteLengthLabel, _plcCommunication.PlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture));
        }

        public void ConnectionButtonClick(object sender, RoutedEventArgs e)
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

        private void ConnectionAtStartUpChecked(object sender, RoutedEventArgs e)
        {
            var startUpConnectionControlBox = (CheckBox) sender;
            if (startUpConnectionControlBox.IsChecked != null)
            {
                _plcStartUpConnection.ConnectAtStartUp = (bool)startUpConnectionControlBox.IsChecked;
                _plcStartUpConnection.Save();
            } 
        }
    }
}
