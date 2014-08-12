using System.Windows.Media;
using OxyPlot;
using OxyPlot.Axes;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerObservableTime : AnalyzerObservableVariable
    {
        public TimeSpanAxis TimeSpanAxis;

        public AnalyzerObservableTime(Analyzer analyzer) : base(analyzer, new CiInteger("Time", 0, CommunicationInterfaceComponent.VariableType.Integer, 0))
        {
            MainViewModel.Model.Axes.Clear();
            MainViewModel.Model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false
            });
            MainViewModel.Model.Axes.Add(TimeSpanAxis = new TimeSpanAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                StringFormat = "hh:mm:ss",
                MajorStep = 1,
                MinorStep = 0.1
            });
            MainViewModel.Brush = Brushes.Black;

            ValueFactor = 1000.0;
        }
    }
}
