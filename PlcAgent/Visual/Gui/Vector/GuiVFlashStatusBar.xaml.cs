using System;
using System.Threading;
using System.Windows.Media;
using _PlcAgent.Vector;

namespace _PlcAgent.Visual.Gui.Vector
{
    /// <summary>
    /// Interaction logic for GuiVFlashStatusBar_p.xaml
    /// </summary>
    public partial class GuiVFlashStatusBar
    {
        private readonly Thread _updateThread;

        public GuiVFlashStatusBar(VFlashHandler vFlashHandler)
            : base(vFlashHandler)
        {
            InitializeComponent();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();

            StatusLabel.Content = "VFLASH__" + VFlashHandler.Header.Id + ":";
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);
                if (channel == null) return;

                Brush colourBrush;
                switch (channel.Status)
                {
                    case VFlashStationComponent.VFlashStatus.Created:
                        colourBrush = Brushes.DarkGray;
                        break;
                    case VFlashStationComponent.VFlashStatus.Loading:
                        colourBrush = Brushes.DarkGray;
                        break;
                    case VFlashStationComponent.VFlashStatus.Loaded:
                        colourBrush = Brushes.GreenYellow;
                        break;
                    case VFlashStationComponent.VFlashStatus.Unloading:
                        colourBrush = Brushes.DarkGray;
                        break;
                    case VFlashStationComponent.VFlashStatus.Unloaded:
                        colourBrush = Brushes.GreenYellow;
                        break;
                    case VFlashStationComponent.VFlashStatus.Flashing:
                        colourBrush = Brushes.DarkGray;
                        break;
                    case VFlashStationComponent.VFlashStatus.Aborting:
                        colourBrush = Brushes.Red;
                        break;
                    case VFlashStationComponent.VFlashStatus.Flashed:
                        colourBrush = Brushes.GreenYellow;
                        break;
                    default:
                        colourBrush = Brushes.Red;
                        break;
                }
                StatusLabel.Dispatcher.BeginInvoke((new Action(delegate
                {
                    StatusRectangle.ToolTip = "vFlash channel: " + channel.Status;
                    StatusRectangle.Fill = colourBrush;
                })));
                Thread.Sleep(21);
            }
        }
    }
}
