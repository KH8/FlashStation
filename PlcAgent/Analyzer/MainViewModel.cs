// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   http://oxyplot.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using Brush = System.Windows.Media.Brush;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;

namespace _PlcAgent.Analyzer
{
    public class MainViewModel : Observable
    {
        private PlotModel _model;
        private readonly LineSeries _series;
        readonly PlotModel _tmp;

        public MainViewModel()
        {
            // Create the plot model
            _tmp = new PlotModel
            {
                IsLegendVisible = false,
                DefaultFontSize = 0,
                PlotMargins = new OxyThickness(20,0,0,10)
            };
            _tmp.Axes.Add(new LinearAxis
            {
                IsAxisVisible = true, //todo: to be changed to false later
                Position = AxisPosition.Bottom
            });
            _series = new LineSeries
            {
                MarkerType = MarkerType.Circle, 
                MarkerSize = 1
            };
            _tmp.Series.Add(_series);
        }

        public PlotModel Model
        {
            get { return _model; }
            set
            {
                if (_model == value) return;
                _model = value;
                RaisePropertyChanged(() => Model);
            }
        }

        public Brush Brush
        {
            get { return _series.Color.ToBrush(); }
            set { _series.Color = value.ToOxyColor(); }
        }

        public void AddPoint(DataPoint dataPoint)
        {
            _series.Points.Add(dataPoint);
            Model = _tmp;
        }

        public void Clear()
        {
            _series.Points.Clear();
        }
    }
}