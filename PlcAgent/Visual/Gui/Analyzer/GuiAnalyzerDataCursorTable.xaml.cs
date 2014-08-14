using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using _PlcAgent.Analyzer;

namespace _PlcAgent.Visual.Gui.Analyzer
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiAnalyzerDataCursorTable
    {
        #region Properties

        public ObservableCollection<AnalyzerDataCursorPoint> AnalyzerDataCursorPointCollection;

        #endregion


        #region Constructors

        public GuiAnalyzerDataCursorTable(_PlcAgent.Analyzer.Analyzer analyzer) : base(analyzer)
        {
            InitializeComponent();

            AnalyzerDataCursorPointCollection = Analyzer.AnalyzerDataCursorPointCollection.Children;
            CursorTableDataGrid.ItemsSource = AnalyzerDataCursorPointCollection;

            Analyzer.AnalyzerDataCursorRed.PropertyChanged += OnDataCursorPositionChanged;
            Analyzer.AnalyzerDataCursorBlue.PropertyChanged += OnDataCursorPositionChanged;
            Analyzer.OnUpdatePlotsDelegate += OnUpdatePlots;

            if (Analyzer.AnalyzerSetupFile.ShowDataCursors[Analyzer.Header.Id]) return;
            Visibility = Visibility.Hidden;
        }

        #endregion

        #region Methods

        private static string FromMilliseconds(double value)
        {
            return Double.IsNaN(value) ? "N/A" : TimeSpan.FromMilliseconds(value).ToString();
        }

        public string FormatNumber(double number, int length)
        {
            var stringRepresentation = number.ToString(CultureInfo.InvariantCulture);

            if (stringRepresentation.Length > length)
                stringRepresentation = stringRepresentation.Substring(0, length);

            if (stringRepresentation.Length == length && stringRepresentation.EndsWith("."))
                stringRepresentation = stringRepresentation.Substring(0, length - 1);

            return stringRepresentation.PadLeft(length);
        }

        #endregion


        #region Event Handlers

        protected override void OnRecordingChanged()
        {
            if (Analyzer.Recording)
            {
                foreach (var dataCursorPoint in Analyzer.AnalyzerDataCursorPointCollection.Children)
                {
                    dataCursorPoint.BlueValue = "---";
                    dataCursorPoint.RedValue = "---";
                    dataCursorPoint.Difference = "---";
                }
                CursorTableDataGrid.Items.Refresh();
            }
            else { OnDataCursorPositionChanged(this, new PropertyChangedEventArgs(""));}
        }

        protected override void OnRecordingTimeChanged()
        {}

        protected override void OnDataCursorsVisibilityChanged()
        {
            Visibility = Analyzer.AnalyzerSetupFile.ShowDataCursors[Analyzer.Header.Id] ? Visibility.Visible : Visibility.Hidden;
        }

        protected void OnUpdatePlots()
        {
            OnDataCursorPositionChanged(this, new PropertyChangedEventArgs(""));
        }

        protected void OnDataCursorPositionChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            double timePointBlue;
            double timePointRed;
            double timeDifference;

            try { timePointBlue = Analyzer.TimeObservableVariable.GetTimePosition(Analyzer.AnalyzerDataCursorBlue.PercentageActualPosition); }
            catch (Exception) { timePointBlue = Double.NaN; }
            try { timePointRed = Analyzer.TimeObservableVariable.GetTimePosition(Analyzer.AnalyzerDataCursorRed.PercentageActualPosition); }
            catch (Exception) { timePointRed = Double.NaN; }
            try { timeDifference = timePointRed - timePointBlue; }
            catch (Exception) { timeDifference = Double.NaN; }

            Analyzer.AnalyzerDataCursorPointCollection.Dispatcher.BeginInvoke((new Action(delegate
            {
                Analyzer.AnalyzerDataCursorPointCollection.Children.Clear();
                Analyzer.AnalyzerDataCursorPointCollection.Children.Add(new AnalyzerDataCursorPoint
                {
                    Name = "Time base",
                    BlueValue = FromMilliseconds(timePointBlue),
                    RedValue = FromMilliseconds(timePointRed),
                    Difference = FromMilliseconds(timeDifference)
                });

                foreach (var analyzerChannel in Analyzer.AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
                {
                    var tolerance = Analyzer.AnalyzerSetupFile.SampleTime[Analyzer.Header.Id] / 1000.0;
                    var pointBlue = analyzerChannel.AnalyzerObservableVariable.GetValue(timePointBlue / 1000.0, tolerance);
                    var pointRed = analyzerChannel.AnalyzerObservableVariable.GetValue(timePointRed / 1000.0, tolerance);
                    var pDifference = pointRed - pointBlue;

                    Analyzer.AnalyzerDataCursorPointCollection.Children.Add(new AnalyzerDataCursorPoint
                    {
                        Name = analyzerChannel.AnalyzerObservableVariable.Name,
                        BlueValue = FormatNumber(pointBlue, 15),
                        RedValue = FormatNumber(pointRed, 15),
                        Difference = FormatNumber(pDifference, 15)
                    });
                }
            })));
        }

        #endregion

    }
}
