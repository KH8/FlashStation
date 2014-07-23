using System;
using System.Threading;
using System.Windows.Controls;
using _PlcAgent.General;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiAnalyzer
    {
        private readonly Analyzer.Analyzer _analyzer;

        private readonly Thread _updateThread;

        public GuiAnalyzer(Module module)
        {
            _analyzer = (Analyzer.Analyzer) module;

            InitializeComponent();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
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
            _analyzer.SampleTime = (int) slider.Value;
        }

        private void TiemRandeChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            TimeRandeLabel.Content = slider.Value + " ms";
            _analyzer.TimeRange = slider.Value;
        }
    }
}
