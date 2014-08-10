using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerMainFrame.xaml
    /// </summary>
    public partial class GuiAnalyzerMainFrame
    {
        private readonly Analyzer.Analyzer _analyzer;
        private readonly List<GuiComponent> _channelList;

        private readonly Thread _updateThread;

        public GuiAnalyzerDataCursor AnalyzerDataCursorRed;
        public GuiAnalyzerDataCursor AnalyzerDataCursorBlue;

        public GuiAnalyzerMainFrame(Analyzer.Analyzer analyzer)
        {
            _analyzer = analyzer;
            _channelList = new List<GuiComponent>();

            InitializeComponent();

            AnalyzerDataCursorRed = new GuiAnalyzerDataCursor(_analyzer)
            {
                ParentGrid = PlotGrid,
                Brush = Brushes.Red,
                ActualPosition = 246.0
            };
            AnalyzerDataCursorBlue = new GuiAnalyzerDataCursor(_analyzer)
            {
                ParentGrid = PlotGrid,
                Brush = Brushes.Blue,
                ActualPosition = 206.0
            };
            GeneralGrid.Children.Add(AnalyzerDataCursorRed);
            GeneralGrid.Children.Add(AnalyzerDataCursorBlue);

            if (!_analyzer.AnalyzerSetupFile.ShowDataCursors[_analyzer.Header.Id])
            {
                AnalyzerDataCursorRed.Visibility = Visibility.Hidden;
                AnalyzerDataCursorBlue.Visibility = Visibility.Hidden;
            }

            PlotArea.DataContext = _analyzer.TimeAxisViewModel;

            RefreshGui();

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
                AnalyzerTimeLabel.Dispatcher.BeginInvoke((new Action(delegate
                {
                    AnalyzerTimeLabel.Content = "Recording time: \n" + TimeSpan.FromMilliseconds(_analyzer.RecordingTime);
                })));
                Thread.Sleep(100);
            }
        }

        private void StartStopRecording(object sender, RoutedEventArgs e)
        {
            _analyzer.StartStopRecording();
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            MainScrollViewer.Height = height - 30;
            MainScrollViewer.Width = width;

            PlotGrid.Width = width - 30;

            TimePlotGrid.Width = width - 225;

            foreach (var analyzerSingleFigure in PlotGrid.Children.Cast<GuiAnalyzerSingleFigure>())
            {
                analyzerSingleFigure.UpdateSizes(height, width);
            }

            AnalyzerDataCursorRed.UpdateSizes(height, width);
            AnalyzerDataCursorBlue.UpdateSizes(height, width);
        }

        private void DrawChannel(uint id)
        {
            GuiComponent analyzerSingleFigure = null;

            foreach (var guiComponent in _channelList.Where(guiComponent => guiComponent.Header.Id == id)) { analyzerSingleFigure = guiComponent; }
            if (analyzerSingleFigure == null) _channelList.Add(analyzerSingleFigure = new GuiComponent(id, "", new GuiAnalyzerSingleFigure(id, _analyzer)));

            analyzerSingleFigure.Initialize(0, ((int)id - 1) * 130, PlotGrid);

            var userControl = (GuiAnalyzerSingleFigure)analyzerSingleFigure.UserControl;
            userControl.UpdateSizes(Height, Width);
        }

        public void RefreshGui()
        {
            PlotGrid.Children.Clear();

            var componentToBeRemoved = _channelList.Where(guiComponent => _analyzer.AnalyzerChannels.GetChannel(guiComponent.Header.Id) == null).ToList();
            foreach (var guiComponent in componentToBeRemoved) { _channelList.Remove(guiComponent); }
            foreach (var analyzerChannel in _analyzer.AnalyzerChannels.Children) { DrawChannel(analyzerChannel.Id); }
        }
    }
}
