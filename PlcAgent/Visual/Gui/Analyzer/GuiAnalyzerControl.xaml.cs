using System;
using System.ComponentModel;
using System.Windows;

namespace _PlcAgent.Visual.Gui.Analyzer
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiAnalyzerControl
    {
        #region Constructors

        public GuiAnalyzerControl(_PlcAgent.Analyzer.Analyzer analyzer) : base(analyzer)
        {
            InitializeComponent();

            Analyzer.CommunicationInterfaceHandler.PlcCommunicator.PropertyChanged += OnConnectionStatusChanged;
            //if (Analyzer.CommunicationInterfaceHandler.PlcCommunicator.ConnectionStatus != 1) AnalyzerStartStopButton.IsEnabled = false;
        }

        #endregion


        #region Event Handlers

        protected void OnConnectionStatusChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            AnalyzerStartStopButton.IsEnabled = true;
            if (Analyzer.CommunicationInterfaceHandler.PlcCommunicator.ConnectionStatus != 1) AnalyzerStartStopButton.IsEnabled = false;
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

        protected override void OnDataCursorsVisibilityChanged()
        {}

        private void StartStopRecording(object sender, RoutedEventArgs e)
        {
            Analyzer.StartStopRecording();
        }

        #endregion

    }
}
