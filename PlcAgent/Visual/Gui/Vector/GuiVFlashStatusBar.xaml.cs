using System;
using System.Windows.Media;
using _PlcAgent.Vector;

namespace _PlcAgent.Visual.Gui.Vector
{
    /// <summary>
    /// Interaction logic for GuiVFlashStatusBar_p.xaml
    /// </summary>
    public sealed partial class GuiVFlashStatusBar
    {
        #region Constructors

        public GuiVFlashStatusBar(VFlashHandler vFlashHandler)
            : base(vFlashHandler)
        {
            InitializeComponent();

            StatusLabel.Content = "VFLASH__" + VFlashHandler.Header.Id + ":";

            OnCommandChanged();
            OnStatusChanged();
            OnProjectHandleChanged();
            OnFlashProjectPathChanged();
            OnResultChanged();
            OnProgressPercentageChanged();
            OnRemainingTimeInSecsChanged();
        }

        #endregion


        #region Event Handlers

        protected override void OnCommandChanged()
        {
        }

        protected override void OnStatusChanged()
        {
            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);

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
        }

        protected override void OnProjectHandleChanged()
        {
        }

        protected override void OnFlashProjectPathChanged()
        {
        }

        protected override void OnFlashingSequenceChanged()
        {
        }

        protected override void OnFlashingStepChanged()
        {
        }

        protected override void OnResultChanged()
        {
        }

        protected override void OnProgressPercentageChanged()
        {
        }

        protected override void OnRemainingTimeInSecsChanged()
        {
        }

        #endregion

    }
}
