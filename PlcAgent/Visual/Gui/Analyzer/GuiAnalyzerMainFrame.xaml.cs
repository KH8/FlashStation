using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using _PlcAgent.General;
using _PlcAgent.Visual.Interfaces;

namespace _PlcAgent.Visual.Gui.Analyzer
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerMainFrame.xaml
    /// </summary>
    public partial class GuiAnalyzerMainFrame : IRefreshableGui, IResizableGui
    {
        #region Variables

        private readonly List<GuiComponent> _channelList;

        private readonly Thread _updateThread;

        #endregion


        #region Constructors

        public GuiAnalyzerMainFrame(_PlcAgent.Analyzer.Analyzer analyzer) : base(analyzer)
        {
            InitializeComponent();

            Analyzer.AnalyzerChannels.OnChannelListModified += RefreshGui;

            _channelList = new List<GuiComponent>();

            Analyzer.AnalyzerDataCursorRed.ParentGrid = PlotGrid;
            Analyzer.AnalyzerDataCursorRed.ActualPosition = 246.0;
            Analyzer.AnalyzerDataCursorBlue.ParentGrid = PlotGrid;
            Analyzer.AnalyzerDataCursorBlue.ActualPosition = 206.0;

            GeneralGrid.Children.Add(Analyzer.AnalyzerDataCursorRed);
            GeneralGrid.Children.Add(Analyzer.AnalyzerDataCursorBlue);

            if (!Analyzer.AnalyzerSetupFile.ShowDataCursors[Analyzer.Header.Id])
            {
                Analyzer.AnalyzerDataCursorRed.Visibility = Visibility.Hidden;
                Analyzer.AnalyzerDataCursorBlue.Visibility = Visibility.Hidden;
            }

            PlotArea.Dispatcher.BeginInvoke((new Action(
                () => PlotArea.DataContext = Analyzer.TimeObservableVariable.MainViewModelClone)));

            RefreshGui();

            _updateThread = new Thread(AxisSynchronization) { IsBackground = true };
            _updateThread.Start();
        }

        #endregion


        #region Methods

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            MainScrollViewer.Height = Limiter.DoubleLimit(height - 30, 0);
            MainScrollViewer.Width = width;

            PlotGrid.Width = Limiter.DoubleLimit(width - 30, 0);

            TimePlotGrid.Width = Limiter.DoubleLimit(width - 225, 0);

            foreach (var analyzerSingleFigure in PlotGrid.Children.Cast<object>().Where(child => child.GetType() == typeof (GuiAnalyzerSingleFigure)).Cast<GuiAnalyzerSingleFigure>())
            {
                analyzerSingleFigure.UpdateSizes(height, width);
            }

            Analyzer.AnalyzerDataCursorRed.UpdateSizes(height, width);
            Analyzer.AnalyzerDataCursorBlue.UpdateSizes(height, width);
        }

        private void DrawChannel(uint id)
        {
            GuiComponent analyzerSingleFigure = null;

            foreach (var guiComponent in _channelList.Where(guiComponent => guiComponent.Header.Id == id)) { analyzerSingleFigure = guiComponent;}

            if (analyzerSingleFigure == null)
                _channelList.Add(
                    analyzerSingleFigure = new GuiComponent(id, "", new GuiAnalyzerSingleFigure(id, Analyzer)));

            PlotGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto, MinHeight = 138, MaxHeight = 350});
            PlotGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3) });

            analyzerSingleFigure.Initialize(0, 0, PlotGrid);
            Grid.SetRow(analyzerSingleFigure.UserControl, ((int)id - 1) * 2);

            GridSplitter newGridSplitter;

            PlotGrid.Children.Add(newGridSplitter = new GridSplitter { Height = 3, HorizontalAlignment = HorizontalAlignment.Stretch });
            Grid.SetRow(newGridSplitter, ((int)id * 2) - 1);

            var userControl = (GuiAnalyzerSingleFigure)analyzerSingleFigure.UserControl;
            userControl.UpdateSizes(Height, Width); 
        }

        public void RefreshGui()
        {
            PlotGrid.Children.Clear();

            var componentToBeRemoved =
                _channelList.Where(guiComponent => Analyzer.AnalyzerChannels.GetChannel(guiComponent.Header.Id) == null)
                    .ToList();

            PlotGrid.RowDefinitions.Clear();

            foreach (var guiComponent in componentToBeRemoved) { _channelList.Remove(guiComponent);}
            foreach (var analyzerChannel in Analyzer.AnalyzerChannels.Children) { DrawChannel(analyzerChannel.Id);}

            PlotGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3) });
        }

        public void AxisSynchronization()
        {
            while (_updateThread.IsAlive)
            {
                if (!Analyzer.Recording)
                {
                    var minimum = Analyzer.TimeObservableVariable.MainViewModel.HorizontalAxis.ActualMinimum;
                    var maximum = Analyzer.TimeObservableVariable.MainViewModel.HorizontalAxis.ActualMaximum;

                    foreach (var analyzerChannel in Analyzer.AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
                    {
                        analyzerChannel.AnalyzerObservableVariable.MainViewModel.SynchronizeView(minimum, maximum);
                    }
                }
                Thread.Sleep(200);
            }
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
                AnalyzerTimeLabel.Content = "Recording time: \n" + TimeSpan.FromMilliseconds(Analyzer.RecordingTime);
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
