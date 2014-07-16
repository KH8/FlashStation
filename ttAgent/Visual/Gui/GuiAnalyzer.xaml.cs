using System;
using System.Threading;
using _ttAgent.General;

namespace _ttAgent.Visual.Gui
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
    }
}
