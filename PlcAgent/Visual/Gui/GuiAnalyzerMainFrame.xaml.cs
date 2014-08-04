using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using _PlcAgent.Analyzer;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerMainFrame.xaml
    /// </summary>
    public partial class GuiAnalyzerMainFrame
    {
        private readonly Analyzer.Analyzer _analyzer;
        private readonly List<GuiComponent> _channelList;

        public GuiAnalyzerMainFrame(Analyzer.Analyzer analyzer)
        {
            _analyzer = analyzer;
            _channelList = new List<GuiComponent>();

            InitializeComponent();

            PlotArea.DataContext = _analyzer.TimeAxisViewModel;

            RefreshGui();
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            MainScrollViewer.Height = height - 30;
            MainScrollViewer.Width = width;

            GeneralGrid.Width = width - 30;

            PlotGrid.Width = width - 225;

            foreach (var analyzerSingleFigure in GeneralGrid.Children.Cast<GuiAnalyzerSingleFigure>())
            {
                analyzerSingleFigure.UpdateSizes(height, width);
            }
        }

        private void DrawChannel(uint id)
        {
            GuiComponent analyzerSingleFigure = null;

            foreach (var guiComponent in _channelList.Where(guiComponent => guiComponent.Header.Id == id)) { analyzerSingleFigure = guiComponent; }
            if (analyzerSingleFigure == null) _channelList.Add(analyzerSingleFigure = new GuiComponent(id, "", new GuiAnalyzerSingleFigure(id, _analyzer)));

            analyzerSingleFigure.Initialize(0, ((int)id - 1) * 130, GeneralGrid);

            var userControl = (GuiAnalyzerSingleFigure)analyzerSingleFigure.UserControl;
            userControl.UpdateSizes(Height, Width);
        }

        public void RefreshGui()
        {
            GeneralGrid.Children.Clear();

            var componentToBeRemoved = _channelList.Where(guiComponent => _analyzer.AnalyzerChannels.GetChannel(guiComponent.Header.Id) == null).ToList();
            foreach (var guiComponent in componentToBeRemoved) { _channelList.Remove(guiComponent); }
            foreach (var analyzerChannel in _analyzer.AnalyzerChannels.Children) { DrawChannel(analyzerChannel.Id); }
        }

        private void PlotAreaOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _analyzer.AnalyzerSetupFile.TimeRange[_analyzer.Header.Id] += e.Delta;
        }
    }
}
