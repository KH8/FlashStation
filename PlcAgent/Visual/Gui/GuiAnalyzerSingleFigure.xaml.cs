using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _PlcAgent.Analyzer;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerSingleFigure.xaml
    /// </summary>
    public partial class GuiAnalyzerSingleFigure
    {
        private AnalyzerObservableVariable _analyzerObservableVariable;
        private readonly Analyzer.Analyzer _analyzer;

        public uint Id;

        public GuiAnalyzerSingleFigure(uint id, Analyzer.Analyzer analyzer)
        {
            Id = id;
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

            ChannelGroupBox.Header = "Channel " + Id;

            VariableComboBox.ItemsSource = _analyzer.CommunicationInterfaceHandler.ReadInterfaceComposite.Children;
        }

        public void UpdateSizes(double height, double width)
        {
            Width = width;
            PlotCanvas.Width = width;
            GeneralGrid.Width = width;
            PlotGrid.Width = width - 225;
        }

        private void BrushSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_analyzerObservableVariable != null) _analyzerObservableVariable.MainViewModel.Brush = (Brush)BrushComboBox.SelectedItem;
        }

        private void VariableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (ComboBox) sender;
            try
            {
                _analyzerObservableVariable = new AnalyzerObservableVariable((CommunicationInterfaceVariable)selector.SelectedItem);
            }
            catch (Exception)
            {
                selector.SelectedItem = null;
                TypeLabel.Content = "no variable selected";
                return;
            }

            _analyzer.AnalyzerObservableVariablesDictionary[Id] = _analyzerObservableVariable;
            PlotArea.DataContext = _analyzerObservableVariable.MainViewModel;
            TypeLabel.Content = _analyzerObservableVariable.Type;
            UpdateLabels();
        }

        private void UnitBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;

            if (_analyzerObservableVariable == null) return;
            try { _analyzerObservableVariable.Unit = box.Text; }
            catch (Exception) { _analyzerObservableVariable.Unit = "1"; }
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            if (_analyzerObservableVariable == null) return;
            VariableLabel.Content = _analyzerObservableVariable.Name + ", " + _analyzerObservableVariable.Type + ", [" +
                                    _analyzerObservableVariable.Unit + "]";
        }
    }
}
