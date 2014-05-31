using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.Vector;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiVFlash : Gui
    {
        private Grid _generalGrid;

        private readonly VFlashHandler _vFlash;
        private int _vFlashButtonEnables = 100;

        private ProgressBar _vFlashProgressBar = new ProgressBar();
        private Label _vFlashProjectPathLabel = new Label();
        private Label _vFlashStatusLabel = new Label();
        private Label _vFlashTimeLabel = new Label();
        private Button _vFlashLoadButton = new Button();
        private Button _vFlashUnloadButton = new Button();
        private Button _vFlashFlashButton = new Button();

        private FaultReport _windowReport;

        private readonly Thread _updateThread;

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiVFlash(uint id, VFlashHandler vFlash)
        {
            Id = id;
            
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

            var guiVFlashGroupBox = GuiFactory.CreateGroupBox("Channel " + Id, 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 120, 775);
            _generalGrid.Children.Add(guiVFlashGroupBox);

            var guiVFlashGrid = GuiFactory.CreateGrid();
            guiVFlashGroupBox.Content = guiVFlashGrid;

            guiVFlashGrid.Children.Add(_vFlashProgressBar = GuiFactory.CreateProgressBar("VFlashProgressBar", 0, 37, HorizontalAlignment.Right, VerticalAlignment.Top, 16, 700));

            guiVFlashGrid.Children.Add(GuiFactory.CreateLabel("Actual Path Path: ", 0, 5, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 112));
            guiVFlashGrid.Children.Add(GuiFactory.CreateLabel("Progress: ", 0, 31, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 61));

            guiVFlashGrid.Children.Add(_vFlashProjectPathLabel = GuiFactory.CreateLabel("VFlashProjectPathLabel", "Channel is not activated", 0, 7, HorizontalAlignment.Right, VerticalAlignment.Top, HorizontalAlignment.Left, 22, 660));
            var converter = new BrushConverter();
            _vFlashProjectPathLabel.BorderBrush = (Brush)converter.ConvertFromString("#FFCFCFCF");
            _vFlashProjectPathLabel.BorderThickness = new Thickness(1);
            _vFlashProjectPathLabel.Padding = new Thickness(5, 1, 2, 2);
            _vFlashProjectPathLabel.Background = Brushes.White;
            
            guiVFlashGrid.Children.Add(_vFlashStatusLabel = GuiFactory.CreateLabel("VFlashStatusLabel", "No project loaded.", 4, 51, HorizontalAlignment.Right, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 562));
            _vFlashStatusLabel.Foreground = Brushes.Red;
            _vFlashStatusLabel.FontSize = 10;

            guiVFlashGrid.Children.Add(_vFlashTimeLabel = GuiFactory.CreateLabel("VFlashTimeLabel", "Remaining time: 00:00:00", 4, 33, HorizontalAlignment.Right, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 483));
            _vFlashTimeLabel.Foreground = Brushes.Black;
            _vFlashTimeLabel.FontSize = 10;

            guiVFlashGrid.Children.Add(_vFlashLoadButton = GuiFactory.CreateButton("VFlashLoadButton", "Load Path", 5, 65, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 90, LoadVFlashProject));
            guiVFlashGrid.Children.Add(_vFlashUnloadButton = GuiFactory.CreateButton("VFlashUnloadButton", "Unload Path", 100, 65, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 90, UnloadVFlashProject));
            guiVFlashGrid.Children.Add(_vFlashFlashButton = GuiFactory.CreateButton("VFlashFlashButton", "Flash", 195, 65, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 90, FlashVFlashProject));
            _vFlashFlashButton.FontWeight = FontWeights.Bold;
            guiVFlashGrid.Children.Add(GuiFactory.CreateButton("VFlashFaultsButton", "Faults", 290, 65, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 90, VFlashShowFaults));

            guiVFlashGrid.Children.Add(GuiFactory.CreateCheckBox("VFlashControlBox", "PC Control", 5, 80, HorizontalAlignment.Right, VerticalAlignment.Top, 77, VFlashControlModeChanged));
        }

        private void VFlashControlModeChanged(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox)sender;
            _vFlash.PcControlMode = !_vFlash.PcControlMode;
            box.IsChecked = _vFlash.PcControlMode;
            Logger.Log("VFlash: PC Control mode changed to " + _vFlash.PcControlMode);
        }

        private void LoadVFlashProject(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".vflashpack", Filter = "Flash Path (.vflashpack)|*.vflashpack" };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                try
                {
                    _vFlash.SetProjectPath(Id, dlg.FileName);
                    _vFlash.LoadProject(Id);
                    Logger.Log("Path load requested by the operator");
                }
                catch (Exception exception) { MessageBox.Show(exception.Message, "Path Loading Failed"); }
            }
        }

        private void UnloadVFlashProject(object sender, RoutedEventArgs e)
        {
            try
            {
                _vFlash.UnloadProject(Id);
                Logger.Log("Path unload requested by the operator");
            }
            catch (Exception exception) { MessageBox.Show(exception.Message, "Path Unloading Failed"); }
        }

        private void FlashVFlashProject(object sender, RoutedEventArgs e)
        {
            var channel = _vFlash.ReturnChannelSetup(Id);
            if (channel.Status == VFlashStationComponent.VFlashStatus.Flashing)
            {
                try
                {
                    _vFlash.AbortFlashing(Id);
                    Logger.Log("Flash abort requested by the operator");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Flash Abort Failed");
                }
            }
            if (channel.Status != VFlashStationComponent.VFlashStatus.Flashing)
            {
                try
                {
                    _vFlash.StartFlashing(Id);
                    Logger.Log("Path start requested by the operator");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Path Flashing Failed");
                }
            }
        }

        private void VFlashShowFaults(object sender, RoutedEventArgs e)
        {
            _windowReport = new FaultReport(ClearFaults);
            _windowReport.Show();
            _windowReport.FaultListBox.Items.Add(_vFlash.ErrorCollector.CreateReport());
            _vFlash.ErrorCollector.CreateReport();
        }

        private void ClearFaults()
        {
            _vFlash.ErrorCollector.Clear();
            _windowReport.FaultListBox.Items.Clear();
            _windowReport.FaultListBox.Items.Add(_vFlash.ErrorCollector.CreateReport());
            Logger.Log("VFlash: Fault list ereased");
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
                string path = "File path is not specified";
                string status;
                Brush colourBrush;

                var channel = _vFlash.ReturnChannelSetup(Id);
                if (channel == null) { return; }
                switch (channel.Status)
                {
                    case VFlashStationComponent.VFlashStatus.Created:
                        status = "Channel created";
                        colourBrush = Brushes.Black;
                        _vFlashButtonEnables = 100;
                        break;
                    case VFlashStationComponent.VFlashStatus.Loading:
                        status = "Loading ...";
                        colourBrush = Brushes.Black;
                        break;
                    case VFlashStationComponent.VFlashStatus.Loaded:
                        status = "Path was loaded succesfully";
                        colourBrush = Brushes.Green;
                        _vFlashButtonEnables = 11;
                        break;
                    case VFlashStationComponent.VFlashStatus.Unloading:
                        status = "Unloading ...";
                        colourBrush = Brushes.Black;
                        break;
                    case VFlashStationComponent.VFlashStatus.Unloaded:
                        status = "Path was unloaded succesfully";
                        colourBrush = Brushes.Green;
                        _vFlashButtonEnables = 100;
                        break;
                    case VFlashStationComponent.VFlashStatus.Flashing:
                        status = "Flashing ...";
                        colourBrush = Brushes.Black;
                        _vFlashFlashButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashFlashButton.Content = "Abort"; })));
                        break;
                    case VFlashStationComponent.VFlashStatus.Aborting:
                        status = "Flash Aborting ...";
                        colourBrush = Brushes.Red;
                        _vFlashFlashButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashFlashButton.Content = "Abort"; })));
                        break;
                    case VFlashStationComponent.VFlashStatus.Flashed:
                        status = "Flashing succeed";
                        colourBrush = Brushes.Green;
                        _vFlashFlashButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashFlashButton.Content = "Flash"; })));
                        _vFlashButtonEnables = 11;
                        break;
                    default:
                        status = channel.Status.ToString();
                        colourBrush = Brushes.Red;
                        _vFlashFlashButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashFlashButton.Content = "Flash"; })));
                        break;
                }

                if (_vFlashButtonEnables == 11)
                {
                    _vFlashLoadButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashLoadButton.IsEnabled = false; })));
                    _vFlashUnloadButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashUnloadButton.IsEnabled = true; })));
                    _vFlashFlashButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashFlashButton.IsEnabled = true; })));
                }
                if (_vFlashButtonEnables == 100)
                {
                    _vFlashLoadButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashLoadButton.IsEnabled = true; })));
                    _vFlashUnloadButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashUnloadButton.IsEnabled = false; })));
                    _vFlashFlashButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashFlashButton.IsEnabled = false; })));
                }
                if (!_vFlash.PcControlMode)
                {
                    _vFlashLoadButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashLoadButton.IsEnabled = false; })));
                    _vFlashUnloadButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashUnloadButton.IsEnabled = false; })));
                    _vFlashFlashButton.Dispatcher.BeginInvoke((new Action(delegate { _vFlashFlashButton.IsEnabled = false; })));
                }

                if (channel.FlashProjectPath != "")
                {
                    string[] words = channel.FlashProjectPath.Split('\\');
                    path = words[words.GetLength(0) - 1];
                }
                _vFlashProjectPathLabel.Dispatcher.BeginInvoke((new Action(delegate { _vFlashProjectPathLabel.Content = path; })));
                _vFlashStatusLabel.Dispatcher.BeginInvoke((new Action(delegate
                {
                    _vFlashStatusLabel.Content = status;
                    _vFlashStatusLabel.Foreground = colourBrush;
                })));

                VFlashStatusHandler(Id);
                Thread.Sleep(21);
            }
        }

        private void VFlashStatusHandler(uint chanId)
        {
            var channel = _vFlash.ReturnChannelSetup(chanId);

            var remainingTimeInMinutes = new TimeSpan(0, 0, 0, (int)channel.RemainingTimeInSecs);
            _vFlashTimeLabel.Dispatcher.BeginInvoke((new Action(delegate { _vFlashTimeLabel.Content = "Remaining time: " + remainingTimeInMinutes; })));
            _vFlashProgressBar.Dispatcher.BeginInvoke((new Action(delegate
            {
                _vFlashProgressBar.Foreground = Brushes.MidnightBlue;
                _vFlashProgressBar.Value = channel.ProgressPercentage;
            })));
        }
    }
}
