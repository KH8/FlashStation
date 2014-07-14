using System.Windows;
using System.Windows.Controls;
using _ttAgent.Analyzer;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerSingleFigure.xaml
    /// </summary>
    public partial class GuiAnalyzerSingleFigure
    {
        private AnalyzerObservableVariable _analyzerObservableVariable;

        public GuiAnalyzerSingleFigure(AnalyzerObservableVariable analyzerObservableVariable)
        {
            _analyzerObservableVariable = analyzerObservableVariable;
            InitializeComponent();

            DataContext = _analyzerObservableVariable.MainViewModel;
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            _analyzerObservableVariable.StoreActualValue();
        }

    }
}
