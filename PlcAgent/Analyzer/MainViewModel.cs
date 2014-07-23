// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   http://oxyplot.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing;
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

        public LinearAxis HorizontalAxis;
        public LinearAxis VerticalAxis;

        public MainViewModel()
        {
            // Create the plot model
            _tmp = new PlotModel
            {
                IsLegendVisible = false,
                DefaultFontSize = 0,
                PlotMargins = new OxyThickness(20,0,0,10)
            };
            _tmp.Axes.Add(HorizontalAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsAxisVisible = true,
                IsPanEnabled = false,
                IsZoomEnabled = false,
                TextColor = OxyColor.Parse("#FFF6F6F6"),
                Position = AxisPosition.Bottom
            });
            _tmp.Axes.Add(VerticalAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left
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