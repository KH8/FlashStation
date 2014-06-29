using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.Log;
using _ttAgent.PLC;

namespace _ttAgent.Visual.Gui
{
    class GuiCommunicationStatus : Gui
    {
        private Grid _generalGrid;

        private readonly PlcCommunicator _plcCommunication;
        private readonly PlcConfigurationFile _plcConfigurationFile;

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
        private Button _connectButton = new Button();

        private readonly Thread _updateThread;

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiCommunicationStatus(uint id, string name, PlcCommunicator plcCommunication) : base(id, name)
        {
            _plcCommunication = plcCommunication;
            _plcConfigurationFile = _plcCommunication.PlcConfigurationFile;

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }

        public override void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = generalGrid;
            var grid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Left, VerticalAlignment.Top);
            _generalGrid.Children.Add(grid);

            var guiGroupBox = GuiFactory.CreateGroupBox("PLC Configuration", 0, 0, HorizontalAlignment.Left,
                VerticalAlignment.Top, 226, 320);
            grid.Children.Add(guiGroupBox);

            var guiGrid = GuiFactory.CreateGrid(0, 10, HorizontalAlignment.Left, VerticalAlignment.Top, 240, 320);
            guiGroupBox.Content = guiGrid;

            guiGrid.Children.Add(GuiFactory.CreateLabel("IP Address:", 38, -2, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Port:", 38, 16, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Rack:", 38, 33, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Slot:", 38, 50, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Read DB Number:", 38, 68, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Read Start Address:", 38, 86, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Read Data Length:", 38, 104, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Write DB Number:", 38, 122, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Write Start Address:", 38, 140, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));
            guiGrid.Children.Add(GuiFactory.CreateLabel("Write Data Length:", 38, 158, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 115));

            guiGrid.Children.Add(_actIpAddressLabel = GuiFactory.CreateLabel("ActIpAddressLabel", "", 170, -2, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(_actPortLabel = GuiFactory.CreateLabel("ActPortLabel", "", 170, 16, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(_actRackLabel = GuiFactory.CreateLabel("ActRackLabel", "", 170, 33, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(_actSlotLabel = GuiFactory.CreateLabel("ActSlotLabel", "", 170, 50, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100));
            guiGrid.Children.Add(_actReadDbNumberLabel = GuiFactory.CreateLabel("ActReadDbNumberLabel", "", 185, 68, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actReadStartAddressLabel = GuiFactory.CreateLabel("ActReadStartAddressLabel", "", 185, 86, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actReadLengthLabel = GuiFactory.CreateLabel("ActReadLengthLabel", "", 185, 104, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actWriteDbNumberLabel = GuiFactory.CreateLabel("ActWriteDbNumberLabel", "", 185, 122, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actWriteStartAddressLabel = GuiFactory.CreateLabel("ActWriteStartAddressLabel", "", 185, 140, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));
            guiGrid.Children.Add(_actWriteLengthLabel = GuiFactory.CreateLabel("ActWriteLengthLabel", "", 185, 158, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85));

            grid.Children.Add(_connectButton = GuiFactory.CreateButton("ConnectButton", "Connect", 0, 232, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100, ConnectionButtonClick));
            grid.Children.Add(_startUpConnectionControlBox = GuiFactory.CreateCheckBox("StartUpConnectionControlBox", "Connect at Start Up", 184, 243, HorizontalAlignment.Left, VerticalAlignment.Top, 134, ConnectionAtStartUpChecked));
            _startUpConnectionControlBox.IsChecked = _plcConfigurationFile.ConnectAtStartUp[Header.Id];
        }

        public override void MakeVisible()
        {
            _generalGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible()
        {
            _generalGrid.Visibility = Visibility.Hidden;
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                GuiFactory.UpdateLabel(_actIpAddressLabel, _plcCommunication.PlcConfiguration.PlcIpAddress);
                GuiFactory.UpdateLabel(_actPortLabel,
                    _plcCommunication.PlcConfiguration.PlcPortNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(_actRackLabel,
                    _plcCommunication.PlcConfiguration.PlcRackNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(_actSlotLabel,
                    _plcCommunication.PlcConfiguration.PlcSlotNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(_actReadDbNumberLabel,
                    _plcCommunication.PlcConfiguration.PlcReadDbNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(_actReadStartAddressLabel,
                    _plcCommunication.PlcConfiguration.PlcReadStartAddress.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(_actReadLengthLabel,
                    _plcCommunication.PlcConfiguration.PlcReadLength.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(_actWriteDbNumberLabel,
                    _plcCommunication.PlcConfiguration.PlcWriteDbNumber.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(_actWriteStartAddressLabel,
                    _plcCommunication.PlcConfiguration.PlcWriteStartAddress.ToString(CultureInfo.InvariantCulture));
                GuiFactory.UpdateLabel(_actWriteLengthLabel,
                    _plcCommunication.PlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture));
                _connectButton.Dispatcher.BeginInvoke((new Action(delegate{
                    _connectButton.Content = "Connect";
                    if (_plcCommunication.ConnectionStatus == 1) { _connectButton.Content = "Disconnect"; }
                })));
                Thread.Sleep(21);
            }
        }

        public void ConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            if (_plcCommunication == null) return;
            try
            {
                if (_plcCommunication.ConnectionStatus != 1)
                {
                    Logger.Log("ID: " + Header.Id + " : Connection requested by the user");
                    _plcCommunication.OpenConnection();
                }
                else
                {
                    Logger.Log("ID: " + Header.Id + " : Disconnection requested by the user");
                    _plcCommunication.CloseConnection();
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
            if (startUpConnectionControlBox.IsChecked != null)
            {
                _plcConfigurationFile.ConnectAtStartUp[Header.Id] = (bool)startUpConnectionControlBox.IsChecked;
                _plcConfigurationFile.Save();
            } 
        }

        public override void UpdateAssignment()
        {
            throw new NotImplementedException();
        }
    }
}
