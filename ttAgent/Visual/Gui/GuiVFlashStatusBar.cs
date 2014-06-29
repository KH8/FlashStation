using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using _ttAgent.Vector;

namespace _ttAgent.Visual.Gui
{
    class GuiVFlashStatusBar : Gui
    {
        private Grid _generalGrid;

        private readonly VFlashHandler _vFlash;

        private Label _vFlashLabel = new Label();
        private Rectangle _statusRectangle = new Rectangle();

        private readonly Thread _updateThread;

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiVFlashStatusBar(uint id, string name, VFlashHandler vFlash) : base(id, name)
        {
            _vFlash = vFlash;

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

            grid.Children.Add(_vFlashLabel = GuiFactory.CreateLabel("VFlashChannelStatusLabel", "VFLASH__" + Header.Id + ":", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Center, HorizontalAlignment.Left, 25, 810));
            grid.Children.Add(_statusRectangle = new Rectangle
            {
                Width = 15,
                Height = 15,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Stroke = Brushes.DarkGray,
                Fill = Brushes.DarkGray,
                Margin = new Thickness(70, 3, 0, 0),
                ToolTip = "",
                RadiusX = 15,
                RadiusY = 15,
            });
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
                var channel = _vFlash.ReturnChannelSetup(Header.Id);
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
                _vFlashLabel.Dispatcher.BeginInvoke((new Action(delegate
                    {
                        _statusRectangle.ToolTip = "vFlash channel: " + channel.Status;
                        _statusRectangle.Fill = colourBrush;
                    })));
                Thread.Sleep(21);
            }
        }

        public override void UpdateAssignment()
        {
            throw new NotImplementedException();
        }
    }
}
