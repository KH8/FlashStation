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
        private readonly Analyzer.Analyzer _analyzer;

        public ObservableCollection<AnalyzerDataCursorPoint> AnalyzerDataCursorPointCollection;

        public GuiAnalyzerDataCursorTable(Analyzer.Analyzer analyzer)
        {
            _analyzer = analyzer;

            InitializeComponent();

            AnalyzerDataCursorPointCollection = _analyzer.AnalyzerDataCursorPointCollection.Children;
            DataCursorTableGrid.ItemsSource = AnalyzerDataCursorPointCollection;

            if (_analyzer.AnalyzerSetupFile.ShowDataCursors[_analyzer.Header.Id]) return;
            Visibility = Visibility.Hidden;
        }
    }
}
