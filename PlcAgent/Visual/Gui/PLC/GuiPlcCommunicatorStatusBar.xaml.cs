using System;
using System.Windows.Media;
using _PlcAgent.PLC;

namespace _PlcAgent.Visual.Gui.PLC
{
    /// <summary>
    /// Interaction logic for GuiVFlashStatusBar_p.xaml
    /// </summary>
    public partial class GuiPlcCommunicatorStatusBar
    {
        #region Constructors

        public GuiPlcCommunicatorStatusBar(PlcCommunicator plcCommunicator)
            : base(plcCommunicator)
        {
            InitializeComponent();

            StatusLabel.Content = "PLC__" + PlcCommunicator.Header.Id + ":";
            Update();
        }

        #endregion


        #region Methods

        public void Update()
        {
            var statusBar = "0";
            Brush brush = Brushes.Red;
            if (PlcCommunicator.ConfigurationStatus != 1)
            {
                statusBar = "Wrong Configuration";
                brush = Brushes.Red;
            }
            if (PlcCommunicator.ConfigurationStatus == 1)
            {
                statusBar = "Configuration verified, ready to connect.";
                brush = Brushes.DarkGray;
            }
            if (PlcCommunicator.ConnectionStatus == 1)
            {
                statusBar = "Connected to IP address " + PlcCommunicator.PlcConfiguration.PlcIpAddress;
                brush = Brushes.GreenYellow;
            }
            if (PlcCommunicator.ConnectionStatus == -2)
            {
                statusBar = "A connection with IP address " + PlcCommunicator.PlcConfiguration.PlcIpAddress +
                            " was interrupted.";
                brush = Brushes.Red;
            }
            StatusLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                StatusRectangle.ToolTip = statusBar;
                StatusRectangle.Fill = brush;
            })));
        }

        #endregion


        #region Event Handlers

        protected override void OnConnectionStatusChanged()
        {
            Update();
        }

        protected override void OnConfigurationStatusChanged()
        {
            Update();
        }

        #endregion

    }
}
