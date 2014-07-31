using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly Boolean _save;
        private readonly Analyzer.Analyzer _analyzer;
        private readonly AnalyzerChannel _analyzerChannel;

        private readonly Thread _updateThread;
        
        public uint Id;

        public GuiAnalyzerSingleFigure(uint id, Analyzer.Analyzer analyzer)
        {
            Id = id;
            _analyzer = analyzer;
            _analyzerChannel = analyzer.AnalyzerChannels.GetChannel(Id);

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

            if (_analyzerChannel.AnalyzerObservableVariable != null)
            {
                VariableComboBox.SelectedItem = _analyzerChannel.AnalyzerObservableVariable.CommunicationInterfaceVariable;
                foreach (var brush in colorsList.Where(brush => Equals(brush.ToString(), _analyzerChannel.AnalyzerObservableVariable.Brush.ToString())))
                {
                    BrushComboBox.SelectedItem = brush;
                }
                UnitTextBox.Text = _analyzerChannel.AnalyzerObservableVariable.Unit; 
            }

            _save = true;

            PlotArea.DataContext = new MainViewModel();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                if (_analyzerChannel.AnalyzerObservableVariable != null && _analyzer.Recording)
                {
                    PlotArea.Dispatcher.BeginInvoke((new Action(
                            () => PlotArea.DataContext = _analyzerChannel.AnalyzerObservableVariable.MainViewModel)));
                }
                UpdateLabels();
                Thread.Sleep(10);
            }
        }

        public void UpdateSizes(double height, double width)
        {
            Width = width;
            PlotCanvas.Width = width;
            GeneralGrid.Width = width;
            PlotGrid.Width = width - 225;
        }

        private void RemoveChannel(object sender, System.Windows.RoutedEventArgs e)
        {
            _analyzer.RemoveChannel(_analyzerChannel);
        }

        private void UpdateLabels()
        {
            if (_analyzerChannel.AnalyzerObservableVariable == null) return;

            if (TypeLabel == null) return;
            TypeLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                TypeLabel.Content = _analyzerChannel.AnalyzerObservableVariable.Type;
            })));
            if (VariableLabel == null) return;
            VariableLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                VariableLabel.Content = _analyzerChannel.AnalyzerObservableVariable.Name + ", " + _analyzerChannel.AnalyzerObservableVariable.Type + ", [" +
                                    _analyzerChannel.AnalyzerObservableVariable.Unit + "]";
            })));
            VariableComboBox.Dispatcher.BeginInvoke((new Action(delegate
            {
                VariableComboBox.IsEnabled = !_analyzer.Recording;
            })));
            DeleteButton.Dispatcher.BeginInvoke((new Action(delegate
            {
                DeleteButton.IsEnabled = !_analyzer.Recording;
            })));
        }

        private void BrushSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_save) return; 
            if (_analyzerChannel.AnalyzerObservableVariable == null) return;
            _analyzerChannel.AnalyzerObservableVariable.Brush = (Brush)BrushComboBox.SelectedItem;
            _analyzer.AnalyzerChannels.StoreConfiguration();
        }

        private void VariableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (ComboBox) sender;

            if (!_save) return; 
            try { _analyzerChannel.AnalyzerObservableVariable = new AnalyzerObservableVariable((CommunicationInterfaceVariable)selector.SelectedItem); }
            catch (Exception)
            {
                _analyzerChannel.AnalyzerObservableVariable = null;
                selector.SelectedItem = null;
                TypeLabel.Content = "no variable selected";
                return;
            }

            _analyzer.AnalyzerChannels.StoreConfiguration();
        }

        private void UnitBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;

            if (!_save) return; 
            if (_analyzerChannel.AnalyzerObservableVariable == null) return;
            try { _analyzerChannel.AnalyzerObservableVariable.Unit = box.Text; }
            catch (Exception) { _analyzerChannel.AnalyzerObservableVariable.Unit = "1"; }

            UpdateLabels();
            _analyzer.AnalyzerChannels.StoreConfiguration();
        }
    }
}
