using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using _PlcAgent.Analyzer;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiAnalyzerConfiguration : AnalyzerComponent
    {
        #region Variables

        private readonly Boolean _save;

        #endregion


        #region Constructors

        public GuiAnalyzerConfiguration(Analyzer.Analyzer analyzer) : base(analyzer)
        {
            InitializeComponent();

            SampleTimeSlider.Value = Analyzer.AnalyzerSetupFile.SampleTime[Analyzer.Header.Id];
            TimeRangeSlider.Value = Analyzer.AnalyzerSetupFile.TimeRange[Analyzer.Header.Id];

            ShowDataCursorsCheckBox.IsChecked = Analyzer.AnalyzerSetupFile.ShowDataCursors[Analyzer.Header.Id];

            _save = true;
        }

        #endregion


        #region Event Handlers

        protected override void OnRecordingChanged()
        {
            AnalyzerAddChannelButton.Dispatcher.BeginInvoke((new Action(delegate
            {
                AnalyzerAddChannelButton.IsEnabled = !Analyzer.Recording;
            })));
            AnalyzerExportButton.Dispatcher.BeginInvoke((new Action(delegate
            {
                AnalyzerExportButton.IsEnabled = !Analyzer.Recording;
            })));
        }

        protected override void OnRecordingTimeChanged()
        {}

        protected override void OnDataCursorsVisibilityChanged()
        {}

        private void AddNewChannel(object sender, RoutedEventArgs e)
        {
            Analyzer.AddNewChannel();
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "AnalysisExport",
                DefaultExt = ".csv",
                Filter = "CSV character-separated values file (.csv)|*.csv"
            };

            var result = dlg.ShowDialog();
            if (result == true) Analyzer.AnalyzerCsvHandler.ExportCsvFile(dlg.FileName);
        }

        private void SampleTimeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider) sender;
            SampleTimeLabel.Content = (int)slider.Value + " ms";
            
            if (!_save) return;
            Analyzer.AnalyzerSetupFile.SampleTime[Analyzer.Header.Id] = (int)slider.Value;
            Analyzer.AnalyzerSetupFile.Save();
        }

        private void TimeRangeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider) sender;
            var newTimeRange = (int)slider.Value;
            TimeRangeLabel.Content = newTimeRange + " ms";

            if (!_save) return;
            Analyzer.AnalyzerSetupFile.TimeRange[Analyzer.Header.Id] = newTimeRange;
            Analyzer.AnalyzerSetupFile.Save();

            Analyzer.TimeRange = newTimeRange;
        }

        private void ShowHideDataCursors(object sender, RoutedEventArgs e)
        {
            var showDataCursorsCheckBox = (CheckBox) sender;
            if (showDataCursorsCheckBox.IsChecked == null) return;

            Analyzer.AnalyzerSetupFile.ShowDataCursors[Analyzer.Header.Id] = (bool) showDataCursorsCheckBox.IsChecked;
            Analyzer.AnalyzerSetupFile.Save();

            Analyzer.DataCursorsVisibility = Analyzer.AnalyzerSetupFile.ShowDataCursors[Analyzer.Header.Id] ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion

    }
}
