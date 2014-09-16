using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _PlcAgent.Analyzer;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Visual.Interfaces;
using _PlcAgent.Visual.TreeListView;

namespace _PlcAgent.Visual.Gui.Analyzer
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerSingleFigure.xaml
    /// </summary>
    public partial class GuiAnalyzerSingleFigure : IResizableGui
    {
        #region Variables

        private readonly Boolean _save;
        private readonly AnalyzerChannel _analyzerChannel;

        private readonly List<Brush> _colorsList = new List<Brush>
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

        #endregion


        #region Properties

        public AnalyzerChannel AnalyzerChannel { get { return _analyzerChannel;}}

        #endregion


        #region Constructors

        public GuiAnalyzerSingleFigure(uint id, _PlcAgent.Analyzer.Analyzer analyzer) : base(analyzer)
        {
            InitializeComponent();

            BrushComboBox.ItemsSource = _colorsList;
            BrushComboBox.SelectedItem = Brushes.Green;
            BrushComboBox.DataContext = this;

            Analyzer.CommunicationInterfaceHandler.OnInterfaceUpdatedDelegate += OnInterfaceUpdated;
            OnInterfaceUpdated();

            _analyzerChannel = Analyzer.AnalyzerChannels.GetChannel(id);
            ChannelGroupBox.Header = "Channel " + _analyzerChannel.Id;

            PlotArea.Dispatcher.BeginInvoke((new Action(
                () => PlotArea.DataContext = new DataMainViewModel())));

            if (_analyzerChannel.AnalyzerObservableVariable != null)
            {
                UpdateControls(this, new PropertyChangedEventArgs("CommunicationInterfaceVariable"));
                UpdateControls(this, new PropertyChangedEventArgs("Brush"));
                UpdateControls(this, new PropertyChangedEventArgs("Unit"));
                _analyzerChannel.AnalyzerObservableVariable.PropertyChanged += UpdateControls;

                PlotArea.Dispatcher.BeginInvoke((new Action(
                () => PlotArea.DataContext = _analyzerChannel.AnalyzerObservableVariable.MainViewModelClone)));
            }

            _save = true;
        }

        #endregion


        #region Mathods

        public void UpdateSizes(double height, double width)
        {
            Width = width;
            GeneralGrid.Width = width;
            PlotGrid.Width = Limiter.DoubleLimit(width - 225, 0);
        }

        #endregion


        #region Event Handlers

        private void RemoveChannel(object sender, RoutedEventArgs e)
        {
            Analyzer.RemoveChannel(_analyzerChannel);
        }

        protected override void OnRecordingChanged()
        {
            VariableComboBox.Dispatcher.BeginInvoke((new Action(delegate
            {
                VariableComboBox.IsEnabled = !Analyzer.Recording;
            })));
            DeleteButton.Dispatcher.BeginInvoke((new Action(delegate
            {
                DeleteButton.IsEnabled = !Analyzer.Recording;
            })));
        }

        protected override void OnRecordingTimeChanged()
        {}

        protected override void OnDataCursorsVisibilityChanged()
        {}

        protected void OnInterfaceUpdated()
        {
            var observableReadCollection = new ObservableCollection<object>();
            var observableWriteCollection = new ObservableCollection<object>();

            new DisplayDataSimpleBuilder().Build(observableReadCollection, observableWriteCollection, Analyzer.CommunicationInterfaceHandler);
            VariableComboBox.ItemsSource = observableReadCollection;
        }

        private void UpdateControls(object sender, PropertyChangedEventArgs e)
        {
            if (_analyzerChannel.AnalyzerObservableVariable == null) return;

            switch (e.PropertyName)
            {
                case "CommunicationInterfaceVariable":
                    VariableComboBox.SelectedItem = _analyzerChannel.AnalyzerObservableVariable.CommunicationInterfaceVariable;
                    TypeLabel.Content = _analyzerChannel.AnalyzerObservableVariable.Type;
                    break;
                case "Brush":
                    foreach (var brush in _colorsList.Where(brush => Equals(brush.ToString(), _analyzerChannel.AnalyzerObservableVariable.Brush.ToString())))
                    { BrushComboBox.SelectedItem = brush; }
                    break;
                case "Unit":
                    UnitTextBox.Text = _analyzerChannel.AnalyzerObservableVariable.Unit;
                    break;
            }
            VariableLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                VariableLabel.Content = _analyzerChannel.AnalyzerObservableVariable.Name
                                        + ", " + _analyzerChannel.AnalyzerObservableVariable.Type
                                        + ", [" + _analyzerChannel.AnalyzerObservableVariable.Unit + "]";
            })));
            
            MinMaxLabel.Dispatcher.BeginInvoke((new Action(delegate
            {
                MinMaxLabel.Content = "ACTUAL: " + _analyzerChannel.AnalyzerObservableVariable.ValueY
                                     + " MIN: " + _analyzerChannel.AnalyzerObservableVariable.MinValue
                                     + " MAX: " + _analyzerChannel.AnalyzerObservableVariable.MaxValue;
            })));
            

        }

        private void BrushSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_save) return;

            if (_analyzerChannel.AnalyzerObservableVariable == null) return;

            _analyzerChannel.AnalyzerObservableVariable.Brush = (Brush) BrushComboBox.SelectedItem;

            Analyzer.AnalyzerChannels.StoreConfiguration();
        }

        private void VariableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (ComboBox) sender;

            if (!_save) return;

            try
            {
                var displayData = (DisplayDataBuilder.DisplayData) VariableComboBox.SelectedItem;
                _analyzerChannel.AnalyzerObservableVariable = new AnalyzerObservableVariable(Analyzer, (CommunicationInterfaceVariable)displayData.Component)
                {
                    Brush = (Brush) BrushComboBox.SelectedItem,
                    Unit = UnitTextBox.Text
                };

                UpdateControls(this, new PropertyChangedEventArgs("CommunicationInterfaceVariable"));
                UpdateControls(this, new PropertyChangedEventArgs("Brush"));
                UpdateControls(this, new PropertyChangedEventArgs("Unit"));
                _analyzerChannel.AnalyzerObservableVariable.PropertyChanged += UpdateControls;

                PlotArea.Dispatcher.BeginInvoke((new Action(
                () => PlotArea.DataContext = _analyzerChannel.AnalyzerObservableVariable.MainViewModelClone)));
            }
            catch (Exception)
            {
                _analyzerChannel.AnalyzerObservableVariable = null;
                selector.SelectedItem = null;
                TypeLabel.Content = "no variable selected";
                return;
            }

            Analyzer.AnalyzerChannels.StoreConfiguration();
        }

        private void UnitBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox) sender;

            if (!_save) return;

            if (_analyzerChannel.AnalyzerObservableVariable == null) return;

            try { _analyzerChannel.AnalyzerObservableVariable.Unit = box.Text; }
            catch (Exception) { _analyzerChannel.AnalyzerObservableVariable.Unit = "1"; }

            Analyzer.AnalyzerChannels.StoreConfiguration();
        }

        #endregion

    }
}
