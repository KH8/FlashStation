using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for DataCursor.xaml
    /// </summary>
    public partial class GuiAnalyzerDataCursor
    {
        private readonly Analyzer.Analyzer _analyzer;

        public GuiAnalyzerDataCursor(Analyzer.Analyzer analyzer)
        {
            _analyzer = analyzer;
            InitializeComponent();
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height - 43;
            Width = width - 242;

            Margin = new Thickness(217,18,0,0);
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            var grid = (Grid) sender;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var offset = grid.Width/2;
                var posX = e.GetPosition(this).X - offset;
                if (posX < 0) posX = 0;
                if (posX > Width - 80) posX = Width - 80;

                grid.Margin = new Thickness(posX, 0, 0, 0);
            }

        }
    }
}
