using System;
using System.Threading;
using System.Windows;
using _PlcAgent.General;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiAnalyzerControl
    {
        public GuiAnalyzerControl(Analyzer.Analyzer analyzer) : base(analyzer)
        {
            InitializeComponent();
        }

        protected override void OnRecordingChanged()
        {
            AnalyzerStartStopButton.Dispatcher.BeginInvoke((new Action(delegate
            {
                AnalyzerStartStopButton.Content = "Start";
                if (Analyzer != null && Analyzer.Recording) AnalyzerStartStopButton.Content = "Stop";
            })));
        }

        protected override void OnRecordingTimeChanged()
        {
            AnalyzerTimeLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                AnalyzerTimeLabel.Content = "Recording time: " + TimeSpan.FromMilliseconds(Analyzer.RecordingTime);
            })));
        }

        private void StartStopRecording(object sender, RoutedEventArgs e)
        {
            Analyzer.StartStopRecording();
        }
    }
}
