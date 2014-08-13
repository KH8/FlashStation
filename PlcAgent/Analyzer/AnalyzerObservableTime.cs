using System.Windows.Media;
using OxyPlot;
using OxyPlot.Axes;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerObservableTime : AnalyzerObservableVariable
    {
        #region Constructors

        public AnalyzerObservableTime(Analyzer analyzer)
            : base(analyzer, new CiInteger("Time", 0, CommunicationInterfaceComponent.VariableType.Integer, 0))
        {
            MainViewModel.Model.Axes.Clear();

            MainViewModel.VerticalAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false
            };
            MainViewModel.HorizontalAxis = new TimeSpanAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                StringFormat = "hh:mm:ss",
                MajorStep = 1,
                MinorStep = 0.1,
                TextColor = OxyColors.Black,
            };

            MainViewModel.Model.Axes.Add(MainViewModel.VerticalAxis);
            MainViewModel.Model.Axes.Add(MainViewModel.HorizontalAxis);

            MainViewModel.Brush = Brushes.Black;

            ValueFactor = 1000.0;
        }

        #endregion

    }
}
