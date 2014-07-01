using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _ttAgent.Log;
using _ttAgent.MainRegistry;
using _ttAgent.Vector;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiVFlashHandler
    {
        private int _xPosition;
        private int _yPosition;
        private Grid _generalGridMemory = new Grid();

        private FaultReportWindow _windowReport;

        public int XPosition
        {
            get { return _xPosition; }
            set { _xPosition = value; }
        }

        public int YPosition
        {
            get { return _yPosition; }
            set { _yPosition = value; }
        }

        public RegistryComponent.RegistryComponentHeader Header;

        private readonly VFlashHandler _vFlash;
        private int _vFlashButtonEnables = 100;

        private readonly Thread _updateThread;

        public GuiVFlashHandler(uint id, string name, VFlashHandler vFlash)
        {
            Header = new RegistryComponent.RegistryComponentHeader
            {
                Id = id,
                Name = name
            };

            _vFlash = vFlash;

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
        }

        public void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            InitializeComponent();
            GeneralGrid.Margin = new Thickness(xPosition, yPosition, 0, 0);
            try
            {
                if (_generalGridMemory != null) _generalGridMemory.Children.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
            generalGrid.Children.Add(this);
            _generalGridMemory = generalGrid;

            HeaderGroupBox.Header = "Channel " + Header.Id;

            _updateThread.Start();
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
                    _vFlash.SetProjectPath(Header.Id, dlg.FileName);
                    _vFlash.LoadProject(Header.Id);
                    Logger.Log("Path load requested by the user");
                }
                catch (Exception exception) { MessageBox.Show(exception.Message, "Path Loading Failed"); }
            }
        }

        private void UnloadVFlashProject(object sender, RoutedEventArgs e)
        {
            try
            {
                _vFlash.UnloadProject(Header.Id);
                Logger.Log("Path unload requested by the user");
            }
            catch (Exception exception) { MessageBox.Show(exception.Message, "Path Unloading Failed"); }
        }

        private void FlashVFlashProject(object sender, RoutedEventArgs e)
        {
            var channel = _vFlash.ReturnChannelSetup(Header.Id);
            if (channel.Status == VFlashStationComponent.VFlashStatus.Flashing)
            {
                try
                {
                    _vFlash.AbortFlashing(Header.Id);
                    Logger.Log("Flash abort requested by the user");
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
                    _vFlash.StartFlashing(Header.Id);
                    Logger.Log("Path start requested by the user");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Path Flashing Failed");
                }
            }
        }

        private void VFlashShowFaults(object sender, RoutedEventArgs e)
        {
            _windowReport = new FaultReportWindow(ClearFaults);
            _windowReport.Show();
            _windowReport.FaultListBox.Items.Add(_vFlash.ErrorCollector.CreateReport());
            _vFlash.ErrorCollector.CreateReport();
        }

        private void ClearFaults()
        {
            _vFlash.ErrorCollector.Clear();
            _windowReport.FaultListBox.Items.Clear();
            _windowReport.FaultListBox.Items.Add(_vFlash.ErrorCollector.CreateReport());
            Logger.Log("VFlash: Fault list ereased by te user");
        }


        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                string path = "File path is not specified";
                string status;
                Brush colourBrush;

                var channel = _vFlash.ReturnChannelSetup(Header.Id);
                if (channel == null)
                {
                    return;
                }
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
                        VFlashFlashButton.Dispatcher.BeginInvoke(
                            (new Action(delegate { VFlashFlashButton.Content = "Abort"; })));
                        break;
                    case VFlashStationComponent.VFlashStatus.Aborting:
                        status = "Flash Aborting ...";
                        colourBrush = Brushes.Red;
                        VFlashFlashButton.Dispatcher.BeginInvoke(
                            (new Action(delegate { VFlashFlashButton.Content = "Abort"; })));
                        break;
                    case VFlashStationComponent.VFlashStatus.Flashed:
                        status = "Flashing succeed";
                        colourBrush = Brushes.Green;
                        VFlashFlashButton.Dispatcher.BeginInvoke(
                            (new Action(delegate { VFlashFlashButton.Content = "Flash"; })));
                        _vFlashButtonEnables = 11;
                        break;
                    default:
                        status = channel.Status.ToString();
                        colourBrush = Brushes.Red;
                        VFlashFlashButton.Dispatcher.BeginInvoke(
                            (new Action(delegate { VFlashFlashButton.Content = "Flash"; })));
                        break;
                }

                if (_vFlashButtonEnables == 11)
                {
                    VFlashLoadButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashLoadButton.IsEnabled = false; })));
                    VFlashUnloadButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashUnloadButton.IsEnabled = true; })));
                    VFlashFlashButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashFlashButton.IsEnabled = true; })));
                }
                if (_vFlashButtonEnables == 100)
                {
                    VFlashLoadButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashLoadButton.IsEnabled = true; })));
                    VFlashUnloadButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashUnloadButton.IsEnabled = false; })));
                    VFlashFlashButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashFlashButton.IsEnabled = false; })));
                }
                if (!_vFlash.PcControlMode)
                {
                    VFlashLoadButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashLoadButton.IsEnabled = false; })));
                    VFlashUnloadButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashUnloadButton.IsEnabled = false; })));
                    VFlashFlashButton.Dispatcher.BeginInvoke(
                        (new Action(delegate { VFlashFlashButton.IsEnabled = false; })));
                }

                if (channel.FlashProjectPath != "")
                {
                    string[] words = channel.FlashProjectPath.Split('\\');
                    path = words[words.GetLength(0) - 1];
                }
                VFlashProjectPathLabel.Dispatcher.BeginInvoke(
                    (new Action(delegate { VFlashProjectPathLabel.Content = path; })));
                VFlashStatusLabel.Dispatcher.BeginInvoke((new Action(delegate
                {
                    VFlashStatusLabel.Content = status;
                    VFlashStatusLabel.Foreground = colourBrush;
                })));

                VFlashStatusHandler(Header.Id);
                Thread.Sleep(21);
            }
        }

        private void VFlashStatusHandler(uint chanId)
        {
            var channel = _vFlash.ReturnChannelSetup(chanId);

            var remainingTimeInMinutes = new TimeSpan(0, 0, 0, (int)channel.RemainingTimeInSecs);
            VFlashTimeLabel.Dispatcher.BeginInvoke((new Action(delegate { VFlashTimeLabel.Content = "Remaining time: " + remainingTimeInMinutes; })));
            VFlashProgressBar.Dispatcher.BeginInvoke((new Action(delegate
            {
                VFlashProgressBar.Foreground = Brushes.MidnightBlue;
                VFlashProgressBar.Value = channel.ProgressPercentage;
            })));
        }
    }
}
