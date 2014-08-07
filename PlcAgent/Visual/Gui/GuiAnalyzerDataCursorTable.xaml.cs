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
        public ObservableCollection<AnalyzerDataCursorPoint> AnalyzerDataCursorPointCollection;

        public GuiAnalyzerDataCursorTable(Analyzer.Analyzer analyzer)
        {
            InitializeComponent();

            AnalyzerDataCursorPointCollection = analyzer.AnalyzerDataCursorPointCollection.Children;
            CursorTableDataGrid.ItemsSource = AnalyzerDataCursorPointCollection;

            if (analyzer.AnalyzerSetupFile.ShowDataCursors[analyzer.Header.Id]) return;
            Visibility = Visibility.Hidden;
        }
    }
}
