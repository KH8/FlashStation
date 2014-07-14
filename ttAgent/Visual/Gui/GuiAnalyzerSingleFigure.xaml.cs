using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using _ttAgent.Analyzer;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerSingleFigure.xaml
    /// </summary>
    public partial class GuiAnalyzerSingleFigure
    {
        private readonly AnalyzerObservableVariable _analyzerObservableVariable;

        public GuiAnalyzerSingleFigure(AnalyzerObservableVariable analyzerObservableVariable)
        {
            _analyzerObservableVariable = analyzerObservableVariable;
            InitializeComponent();

            var colorsList = new List<Brush>
            {
                Brushes.Red,
                Brushes.DarkRed,
                Brushes.Green,
                Brushes.DarkGreen,
                Brushes.Blue,
                Brushes.DarkBlue,
                Brushes.Yellow,
                Brushes.Orange,
                Brushes.DarkOrange,
                Brushes.Black
            };

            BrushComboBox.ItemsSource = colorsList;
            BrushComboBox.SelectedItem = Brushes.Black;
            BrushComboBox.DataContext = this;
            PlotArea.DataContext = _analyzerObservableVariable.MainViewModel;
            TypeLabel.Content = _analyzerObservableVariable.Type;
        }

        private void Refresh(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _analyzerObservableVariable.StoreActualValue();
        }

        private void BrushSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _analyzerObservableVariable.MainViewModel.Brush = (Brush)BrushComboBox.SelectedItem;
        }
    }
}
