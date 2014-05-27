using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _3880_80_FlashStation.Vector;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiVFlashStatusBar : Gui
    {
        private Grid _generalGrid;

        private readonly VFlashHandler _vFlash;

        private Label _vFlashLabel = new Label();

        private readonly Thread _updateThread;

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiVFlashStatusBar(uint id, VFlashHandler vFlash)
        {
            Id = id;
            
            _vFlash = vFlash;

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }

        public override void Initialize(int xPosition, int yPosition)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            GeneralGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Right, VerticalAlignment.Top, 25, 810);
            GeneralGrid.Children.Add(_vFlashLabel = GuiFactory.CreateLabel("VFlashChannelStatusLabel", "vFlash Channel 1 Initialized", 0, 0, HorizontalAlignment.Center, VerticalAlignment.Bottom, HorizontalAlignment.Right, 25, 810));
            _vFlashLabel.FontSize = 10; 
        }

        public override void MakeVisible()
        {
            GeneralGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible()
        {
            GeneralGrid.Visibility = Visibility.Hidden;
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                var channel = _vFlash.ReturnChannelSetup(Id);
                if (channel == null) return;

                Brush colourBrush;
                switch (channel.Status)
                {
                    case VFlashStationComponent.VFlashStatus.Created:
                        colourBrush = Brushes.Black;
                        break;
                    case VFlashStationComponent.VFlashStatus.Loading:
                        colourBrush = Brushes.Black;
                        break;
                    case VFlashStationComponent.VFlashStatus.Loaded:
                        colourBrush = Brushes.Green;
                        break;
                    case VFlashStationComponent.VFlashStatus.Unloading:
                        colourBrush = Brushes.Black;
                        break;
                    case VFlashStationComponent.VFlashStatus.Unloaded:
                        colourBrush = Brushes.Green;
                        break;
                    case VFlashStationComponent.VFlashStatus.Flashing:
                        colourBrush = Brushes.Black;
                        break;
                    case VFlashStationComponent.VFlashStatus.Aborting:
                        colourBrush = Brushes.Red;
                        break;
                    case VFlashStationComponent.VFlashStatus.Flashed:
                        colourBrush = Brushes.Green;
                        break;
                    default:
                        colourBrush = Brushes.Red;
                        break;
                }
                _vFlashLabel.Dispatcher.BeginInvoke((new Action(delegate
                    {
                        _vFlashLabel.Content = "vFlash channel " + channel.ChannelId + ": " + channel.Status;
                        _vFlashLabel.Foreground = colourBrush;
                    })));
                Thread.Sleep(21);
            }
        }
    }
}
