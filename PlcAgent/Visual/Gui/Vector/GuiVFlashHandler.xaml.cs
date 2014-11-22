using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using _PlcAgent.Log;
using _PlcAgent.Vector;

namespace _PlcAgent.Visual.Gui.Vector
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public sealed partial class GuiVFlashHandler
    {
        #region Variables

        private FaultReportWindow _windowReport;
        private int _vFlashButtonEnables = 100;

        #endregion


        #region Constructors

        public GuiVFlashHandler(VFlashHandler vFlashHandler)
            : base(vFlashHandler)
        {
            InitializeComponent();

            HeaderGroupBox.Header = "Channel " + VFlashHandler.Header.Id;

            OnCommandChanged();
            OnStatusChanged();
            OnProjectHandleChanged();
            OnFlashProjectPathChanged();
            OnResultChanged();
            OnProgressPercentageChanged();
            OnRemainingTimeInSecsChanged();

        }

        #endregion


        #region Envent Handlers

        protected override void OnCommandChanged()
        {
        }

        protected override void OnStatusChanged()
        {
            string status;
            Brush colourBrush;

            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);
            if (channel == null) return;

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
                case VFlashStationComponent.VFlashStatus.SequenceDone:
                    status = "Flashing Sequence succeed";
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

            if (!VFlashHandler.PcControlMode)
            {
                VFlashLoadButton.Dispatcher.BeginInvoke(
                    (new Action(delegate { VFlashLoadButton.IsEnabled = false; })));
                VFlashUnloadButton.Dispatcher.BeginInvoke(
                    (new Action(delegate { VFlashUnloadButton.IsEnabled = false; })));
                VFlashFlashButton.Dispatcher.BeginInvoke(
                    (new Action(delegate { VFlashFlashButton.IsEnabled = false; })));
            }

            VFlashStatusLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                VFlashStatusLabel.Content = status;
                VFlashStatusLabel.Foreground = colourBrush;
            })));
        }

        protected override void OnProjectHandleChanged()
        {
        }

        protected override void OnFlashProjectPathChanged()
        {
            var path = "File path is not specified";

            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);
            if (channel == null) return;

            if (channel.FlashProjectPath != "")
            {
                var words = channel.FlashProjectPath.Split('\\');
                path = words[words.GetLength(0) - 1];
            }
            VFlashProjectPathLabel.Dispatcher.BeginInvoke(
                (new Action(delegate { VFlashProjectPathLabel.Content = path; })));
        }

        protected override void OnFlashingSequenceChanged()
        {
            var sequence = "no version selected";

            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);
            if (channel == null) return;

            if (channel.FlashingSequence != null)
            {
                sequence = channel.FlashingSequence.Version;
            }

            VFlashProjectVersionLabel.Dispatcher.BeginInvoke(
                (new Action(delegate { VFlashProjectVersionLabel.Content = sequence; })));
        }

        protected override void OnFlashingStepChanged()
        {
            var step = "-";

            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);
            if (channel == null) return;

            if (channel.FlashingSequence != null)
            {
                step = channel.FlashingStep.Id.ToString(CultureInfo.InvariantCulture);
            }

            VFlashProjectStepLabel.Dispatcher.BeginInvoke(
                (new Action(delegate { VFlashProjectStepLabel.Content = step + " / " + channel.FlashingSequence.Steps.Count; })));
        }

        protected override void OnResultChanged()
        {
        }

        protected override void OnProgressPercentageChanged()
        {
            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);
   
            VFlashProgressBar.Dispatcher.BeginInvoke((new Action(delegate
            {
                VFlashProgressBar.Foreground = Brushes.MidnightBlue;
                VFlashProgressBar.Value = channel.ProgressPercentage;
            })));
        }

        protected override void OnRemainingTimeInSecsChanged()
        {
            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);

            var remainingTimeInMinutes = new TimeSpan(0, 0, 0, (int)channel.RemainingTimeInSecs);
            VFlashTimeLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                VFlashTimeLabel.Content = "Remaining time: " + remainingTimeInMinutes;
            })));
        }

        private void VFlashControlModeChanged(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox) sender;
            VFlashHandler.PcControlMode = !VFlashHandler.PcControlMode;
            box.IsChecked = VFlashHandler.PcControlMode;
            Logger.Log("VFlash: PC Control mode changed to " + VFlashHandler.PcControlMode);

            OnStatusChanged();
        }

        private void LoadVFlashProject(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".vflashpack",
                Filter = "Flash Path (.vflashpack)|*.vflashpack"
            };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                try
                {
                    VFlashHandler.SetProjectPath(VFlashHandler.Header.Id, dlg.FileName);
                    VFlashHandler.LoadProject(VFlashHandler.Header.Id);
                    Logger.Log("Path load requested by the user");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Path Loading Failed");
                }
            }
        }

        private void UnloadVFlashProject(object sender, RoutedEventArgs e)
        {
            try
            {
                VFlashHandler.UnloadProject(VFlashHandler.Header.Id);
                Logger.Log("Path unload requested by the user");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Path Unloading Failed");
            }
        }

        private void FlashVFlashProject(object sender, RoutedEventArgs e)
        {
            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);
            if (channel.Status == VFlashStationComponent.VFlashStatus.Flashing)
            {
                try
                {
                    VFlashHandler.AbortFlashing(VFlashHandler.Header.Id);
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
                    VFlashHandler.StartFlashing(VFlashHandler.Header.Id);
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
            _windowReport.FaultListBox.Items.Add(VFlashHandler.ErrorCollector.CreateReport());
            VFlashHandler.ErrorCollector.CreateReport();
        }

        #endregion


        #region Methods

        private void ClearFaults()
        {
            VFlashHandler.ErrorCollector.Clear();
            _windowReport.FaultListBox.Items.Clear();
            _windowReport.FaultListBox.Items.Add(VFlashHandler.ErrorCollector.CreateReport());
            Logger.Log("VFlash: Fault list ereased by te user");
        }

        #endregion

    }
}
