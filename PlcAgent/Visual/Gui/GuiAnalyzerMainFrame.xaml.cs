using System.Linq;
using UserControl = System.Windows.Controls.UserControl;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerMainFrame.xaml
    /// </summary>
    public partial class GuiAnalyzerMainFrame
    {
        private readonly Analyzer.Analyzer _analyzer;

        public GuiAnalyzerMainFrame(Analyzer.Analyzer analyzer)
        {
            _analyzer = analyzer;
            InitializeComponent();
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            MainScrollViewer.Height = height;
            MainScrollViewer.Width = width;

            GeneralGrid.Width = width - 30;

            foreach (var analyzerSingleFigure in GeneralGrid.Children.Cast<GuiAnalyzerSingleFigure>())
            {
                analyzerSingleFigure.UpdateSizes(height, width);
            }
        }

        private void DrawChannel(uint id)
        {
            var analyzerSingleFigure = new GuiComponent(id, "", new GuiAnalyzerSingleFigure(id, _analyzer));
            analyzerSingleFigure.Initialize(0, ((int)id - 1) * 130, GeneralGrid);

            var userControl = (GuiAnalyzerSingleFigure)analyzerSingleFigure.UserControl;
            userControl.UpdateSizes(Height, Width);
        }

        public void RefreshGui()
        {
            GeneralGrid.Children.Clear();
            foreach (var analyzerChannel in _analyzer.AnalyzerChannels.Children) { DrawChannel(analyzerChannel.Id); }
        }
    }
}
