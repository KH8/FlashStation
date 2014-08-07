using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerDataCursorPoint
    {
        public string Name { get; set; }
        public string BlueValue { get; set; }
        public string RedValue { get; set; }
        public string Difference { get; set; }
    }

    public class AnalyzerDataCursorPointCollection
    {
        public ObservableCollection<AnalyzerDataCursorPoint> Children { get; set; }
        public Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        public AnalyzerDataCursorPointCollection()
        {
            Children = new ObservableCollection<AnalyzerDataCursorPoint>();
        }

        public AnalyzerDataCursorPoint GetAssignment(string name)
        {
            return Children.Where(assignment => assignment.Name == name).Select(point => point).FirstOrDefault();
        }
    }
}
