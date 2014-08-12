using System.Collections.ObjectModel;
using System.Windows;
using _PlcAgent.Analyzer;

namespace _PlcAgent.Visual.Gui
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

        public GuiAnalyzerDataCursorTable(Analyzer.Analyzer analyzer) : base(analyzer)
        {
            InitializeComponent();

            AnalyzerDataCursorPointCollection = Analyzer.AnalyzerDataCursorPointCollection.Children;
            CursorTableDataGrid.ItemsSource = AnalyzerDataCursorPointCollection;

            if (Analyzer.AnalyzerSetupFile.ShowDataCursors[Analyzer.Header.Id]) return;
            Visibility = Visibility.Hidden;
        }

        #endregion


        #region Event Handlers

        protected override void OnRecordingChanged()
        {}

        protected override void OnRecordingTimeChanged()
        {}

        #endregion

    }
}
