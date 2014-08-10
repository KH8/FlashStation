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
    public partial class GuiAnalyzerControl
    {
        private readonly Boolean _save;
        private readonly Analyzer.Analyzer _analyzer;
        private readonly AnalyzerSetupFile _analyzerSetupFile;

        private readonly Thread _updateThread;

        public GuiAnalyzerControl(Module module)
        {
            _analyzer = (Analyzer.Analyzer) module;
            _analyzerSetupFile = _analyzer.AnalyzerSetupFile;

            InitializeComponent();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();

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
                Thread.Sleep(100);
            }
        }

        private void StartStopRecording(object sender, RoutedEventArgs e)
        {
            _analyzer.StartStopRecording();
        }
    }
}
