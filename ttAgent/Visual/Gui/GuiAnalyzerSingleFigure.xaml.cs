using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using _ttAgent.Analyzer;
using _ttAgent.DataAquisition;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerSingleFigure.xaml
    /// </summary>
    public partial class GuiAnalyzerSingleFigure
    {
        private uint _id;
        private AnalyzerObservableVariable _analyzerObservableVariable;
        private Analyzer.Analyzer _analyzer;

        public GuiAnalyzerSingleFigure(uint id, Analyzer.Analyzer analyzer)
        {
            _id = id;
            _analyzer = analyzer;
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
            BrushComboBox.SelectedItem = Brushes.Green;
            BrushComboBox.DataContext = this;

            VariableComboBox.ItemsSource = _analyzer.CommunicationInterfaceHandler.ReadInterfaceComposite.Children;
        }

        private void Refresh(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _analyzerObservableVariable.StoreActualValue();
        }

        private void BrushSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_analyzerObservableVariable != null) _analyzerObservableVariable.MainViewModel.Brush = (Brush)BrushComboBox.SelectedItem;
        }

        private void VariableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (ComboBox) sender;
            _analyzerObservableVariable = new AnalyzerObservableVariable((CommunicationInterfaceVariable)selector.SelectedItem);
            _analyzer.AnalyzerObservableVariablesDictionary[_id] = _analyzerObservableVariable;
            PlotArea.DataContext = _analyzerObservableVariable.MainViewModel;
            TypeLabel.Content = _analyzerObservableVariable.Type;
        }
    }
}
