using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiCommunicationStatusBar : Gui
    {
        private Grid _generalGrid;

        private readonly PlcCommunicator _plcCommunication;

        private Label _plcStatusLabel = new Label();

        private readonly Thread _updateThread;

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiCommunicationStatusBar(uint id, PlcCommunicator plcCommunication)
        {
            Id = id;

            _plcCommunication = plcCommunication;

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
            var grid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Right, VerticalAlignment.Top);
            _generalGrid.Children.Add(grid);

            grid.Children.Add(_plcStatusLabel = GuiFactory.CreateLabel("PlcStatusLabel", "Wrong Configuration!", 0, 0, HorizontalAlignment.Center, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 810));
            _plcStatusLabel.FontSize = 10;
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
                string statusBar = "0";
                Brush brush = Brushes.Red;
                if (_plcCommunication.ConfigurationStatus != 1)
                {
                    statusBar = "Wrong Configuration";
                    brush = Brushes.Red;
                }
                if (_plcCommunication.ConfigurationStatus == 1)
                {
                    statusBar = "Configuration verified, ready to connect.";
                    brush = Brushes.Black;
                }
                if (_plcCommunication.ConnectionStatus == 1)
                {
                    statusBar = "Connected to IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress;
                    brush = Brushes.Green;
                }
                if (_plcCommunication.ConnectionStatus == -2)
                {
                    statusBar = "A connection with IP address " + _plcCommunication.PlcConfiguration.PlcIpAddress + " was interrupted.";
                    brush = Brushes.Red;
                }
                _plcStatusLabel.Dispatcher.BeginInvoke((new Action(delegate
                {
                    _plcStatusLabel.Content = statusBar;
                    _plcStatusLabel.Foreground = brush;
                })));
                Thread.Sleep(21);
            }
        }
    }
}
