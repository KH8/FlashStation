using System.Linq;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerMainFrame.xaml
    /// </summary>
    public partial class GuiAnalyzerMainFrame : UserControl
    {
        public GuiAnalyzerMainFrame()
        {
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
    }
}
