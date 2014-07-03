using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using _ttAgent.PLC;
using _ttAgent.Vector;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiVFlashStatusBar_p.xaml
    /// </summary>
    public partial class GuiCommunicationStatusBar : UserControl
    {
        private readonly PlcCommunicator _plcCommunicator;
        private readonly Thread _updateThread;

        public GuiCommunicationStatusBar(PlcCommunicator plcCommunicator)
        {
            _plcCommunicator = plcCommunicator;
            InitializeComponent();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();

            StatusLabel.Content = "PLC__" + _plcCommunicator.Header.Id + ":";
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                var statusBar = "0";
                Brush brush = Brushes.Red;
                if (_plcCommunicator.ConfigurationStatus != 1)
                {
                    statusBar = "Wrong Configuration";
                    brush = Brushes.Red;
                }
                if (_plcCommunicator.ConfigurationStatus == 1)
                {
                    statusBar = "Configuration verified, ready to connect.";
                    brush = Brushes.DarkGray;
                }
                if (_plcCommunicator.ConnectionStatus == 1)
                {
                    statusBar = "Connected to IP address " + _plcCommunicator.PlcConfiguration.PlcIpAddress;
                    brush = Brushes.GreenYellow;
                }
                if (_plcCommunicator.ConnectionStatus == -2)
                {
                    statusBar = "A connection with IP address " + _plcCommunicator.PlcConfiguration.PlcIpAddress + " was interrupted.";
                    brush = Brushes.Red;
                }
                StatusLabel.Dispatcher.BeginInvoke((new Action(delegate
                {
                    StatusRectangle.ToolTip = statusBar;
                    StatusRectangle.Fill = brush;
                })));
                Thread.Sleep(21);
            }
        }
    }
}
