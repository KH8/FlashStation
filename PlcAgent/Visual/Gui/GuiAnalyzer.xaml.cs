using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using _PlcAgent.Analyzer;
using _PlcAgent.General;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiAnalyzer
    {
        private readonly Boolean _save;
        private readonly Analyzer.Analyzer _analyzer;
        private readonly AnalyzerSetupFile _analyzerSetupFile;

        private readonly Thread _updateThread;

        public GuiAnalyzer(Module module)
        {
            _analyzer = (Analyzer.Analyzer) module;
            _analyzerSetupFile = _analyzer.AnalyzerSetupFile;

            InitializeComponent();

            SampleTimeSlider.Value = _analyzerSetupFile.SampleTime[_analyzer.Header.Id];
            TimeRangeSlider.Value = _analyzerSetupFile.TimeRange[_analyzer.Header.Id];

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();

            ShowDataCursorsCheckBox.IsChecked = _analyzerSetupFile.ShowDataCursors[_analyzer.Header.Id];

            _save = true;
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                AnalyzerStartStopButton.Dispatcher.BeginInvoke((new Action(delegate
                {
                    AnalyzerStartStopButton.Content = "Start";
                    if (_analyzer != null && _analyzer.Recording) AnalyzerStartStopButton.Content = "Stop";
                })));
                AnalyzerTimeLabel.Dispatcher.BeginInvoke((new Action(delegate
                {
                    AnalyzerTimeLabel.Content = "Recording time: " + TimeSpan.FromMilliseconds(_analyzer.RecordingTime);
                })));
                AnalyzerAddChannelButton.Dispatcher.BeginInvoke((new Action(delegate
                {
                    AnalyzerAddChannelButton.IsEnabled = !_analyzer.Recording;
                })));
                AnalyzerExportButton.Dispatcher.BeginInvoke((new Action(delegate
                {
                    AnalyzerExportButton.IsEnabled = !_analyzer.Recording;
                })));
                Thread.Sleep(100);
            }
        }

        private void StartStopRecording(object sender, RoutedEventArgs e)
        {
            _analyzer.StartStopRecording();
        }

        private void AddNewChannel(object sender, RoutedEventArgs e)
        {
            _analyzer.AddNewChannel();
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "AnalysisExport",
                DefaultExt = ".csv",
                Filter = "CSV character-separated values file (.csv)|*.csv"
            };

            var result = dlg.ShowDialog();
            if (result == true) _analyzer.ExportCsvFile(dlg.FileName);
        }

        private void SampleTimeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider) sender;
            SampleTimeLabel.Content = slider.Value + " ms";
            if (!_save) return; 
            _analyzerSetupFile.SampleTime[_analyzer.Header.Id] = (int) slider.Value;
            _analyzerSetupFile.Save();
        }

        private void TimeRangeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            TimeRangeLabel.Content = slider.Value + " ms";
            if (!_save) return; 
            _analyzerSetupFile.TimeRange[_analyzer.Header.Id] = slider.Value;
            _analyzerSetupFile.Save();
        }

        private void ShowHideDataCursors(object sender, RoutedEventArgs e)
        {
            var showDataCursorsCheckBox = (CheckBox)sender;
            if (showDataCursorsCheckBox.IsChecked == null) return;
            _analyzerSetupFile.ShowDataCursors[_analyzer.Header.Id] = (bool)showDataCursorsCheckBox.IsChecked;
            _analyzerSetupFile.Save();

            var visibility = Visibility.Hidden;
            if (_analyzer.AnalyzerSetupFile.ShowDataCursors[_analyzer.Header.Id]) visibility = Visibility.Visible;

            if (_analyzer.GuiAnalyzerMainFrame != null)
            {
                _analyzer.GuiAnalyzerMainFrame.AnalyzerDataCursorRed.Visibility = visibility;
                _analyzer.GuiAnalyzerMainFrame.AnalyzerDataCursorBlue.Visibility = visibility;
            }
            if (_analyzer.GuiAnalyzerDataCursorTable != null) _analyzer.GuiAnalyzerDataCursorTable.Visibility = visibility;
        }
    }
}
