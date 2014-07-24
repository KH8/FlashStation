using System;
using System.Threading;
using System.Windows.Controls;
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

            _analyzer.InitrializeChannels();

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
                Thread.Sleep(20);
            }
        }

        private void StartStopRecording(object sender, System.Windows.RoutedEventArgs e)
        {
            _analyzer.StartStopRecording();
        }

        private void AddNewChannel(object sender, System.Windows.RoutedEventArgs e)
        {
            _analyzer.AddNewChannel();
        }

        private void SampleTimeChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider) sender;
            SampleTimeLabel.Content = slider.Value + " ms";
            if (!_save) return; 
            _analyzerSetupFile.SampleTime[_analyzer.Header.Id] = (int) slider.Value;
            _analyzerSetupFile.Save();
        }

        private void TimeRangeChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            TimeRangeLabel.Content = slider.Value + " ms";
            if (!_save) return; 
            _analyzerSetupFile.TimeRange[_analyzer.Header.Id] = slider.Value;
            _analyzerSetupFile.Save();
        }
    }
}
