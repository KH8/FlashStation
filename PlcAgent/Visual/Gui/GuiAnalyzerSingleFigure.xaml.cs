using System;
using System.Collections.Generic;
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
        private readonly Analyzer.Analyzer _analyzer;
        private readonly AnalyzerChannel _analyzerChannel;
        
        public uint Id;

        public GuiAnalyzerSingleFigure(uint id, Analyzer.Analyzer analyzer)
        {
            Id = id;
            _analyzer = analyzer;
            _analyzerChannel = analyzer.GetChannel(Id);

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

            ChannelGroupBox.Header = "Channel " + _analyzerChannel.Id;

            VariableComboBox.ItemsSource = _analyzer.CommunicationInterfaceHandler.ReadInterfaceComposite.Children;
        }

        public void UpdateSizes(double height, double width)
        {
            Width = width;
            PlotCanvas.Width = width;
            GeneralGrid.Width = width;
            PlotGrid.Width = width - 225;
        }

        public void UpdateFigure()
        {
            UpdatePlotArea();
            UpdateLabels();
        }

        private void BrushSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_analyzerChannel.AnalyzerObservableVariable == null) return;
            _analyzerChannel.AnalyzerObservableVariable.MainViewModel.Brush = (Brush)BrushComboBox.SelectedItem;
            _analyzerChannel.AnalyzerObservableVariable.Brush = (Brush)BrushComboBox.SelectedItem;
        }

        private void VariableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (ComboBox) sender;
            try { _analyzerChannel.AnalyzerObservableVariable = new AnalyzerObservableVariable((CommunicationInterfaceVariable)selector.SelectedItem); }
            catch (Exception)
            {
                selector.SelectedItem = null;
                TypeLabel.Content = "no variable selected";
                return;
            }
            _analyzer.AnalyzerChannels.StoreConfiguration();
            UpdateFigure();
        }

        private void UnitBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;

            if (_analyzerChannel.AnalyzerObservableVariable == null) return;
            try { _analyzerChannel.AnalyzerObservableVariable.Unit = box.Text; }
            catch (Exception) { _analyzerChannel.AnalyzerObservableVariable.Unit = "1"; }
            UpdateLabels();
        }

        private void UpdatePlotArea()
        {
            if (_analyzerChannel.AnalyzerObservableVariable == null) return;
            PlotArea.DataContext = _analyzerChannel.AnalyzerObservableVariable.MainViewModel;
        }


        private void UpdateLabels()
        {
            if (_analyzerChannel.AnalyzerObservableVariable == null) return;
            TypeLabel.Content = _analyzerChannel.AnalyzerObservableVariable.Type;
            if (VariableLabel == null) return;
            VariableLabel.Content = _analyzerChannel.AnalyzerObservableVariable.Name + ", " + _analyzerChannel.AnalyzerObservableVariable.Type + ", [" +
                                    _analyzerChannel.AnalyzerObservableVariable.Unit + "]";
        }

        private void RemoveChannel(object sender, System.Windows.RoutedEventArgs e)
        {
            _analyzer.RemoveChannel(_analyzerChannel);
        }
    }
}
