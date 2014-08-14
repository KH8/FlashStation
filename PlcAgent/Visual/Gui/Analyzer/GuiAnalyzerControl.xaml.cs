using System;
using System.Windows;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiAnalyzerControl
    {
        #region Constructors

        public GuiAnalyzerControl(Analyzer.Analyzer analyzer) : base(analyzer)
        {
            InitializeComponent();
        }

        #endregion


        #region Event Handlers

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

        protected override void OnDataCursorsVisibilityChanged()
        {}

        private void StartStopRecording(object sender, RoutedEventArgs e)
        {
            Analyzer.StartStopRecording();
        }

        #endregion

    }
}
