// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   http://oxyplot.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using Brush = System.Windows.Media.Brush;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using TimeSpanAxis = OxyPlot.Axes.TimeSpanAxis;

namespace _PlcAgent.Analyzer
{
    public class MainViewModel : Observable, ICloneable
    {
        private PlotModel _model;
        private readonly LineSeries _series;

        public LinearAxis HorizontalAxis;
        public LinearAxis VerticalAxis;

        private readonly DataPoint _emptyDataPoint;
        private DataPoint _newDataPoint;

        private readonly Dispatcher _dispatcher;
        private readonly Thread _updateThread;

        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel
            {
                IsLegendVisible = false,
                DefaultFontSize = 0,
                PlotMargins = new OxyThickness(20,0,0,10)
            };
            tmp.Axes.Add(HorizontalAxis = new TimeSpanAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsAxisVisible = true,
                IsPanEnabled = false,
                IsZoomEnabled = false,
                TextColor = OxyColor.Parse("#FFF6F6F6"),
                Position = AxisPosition.Bottom
            });
            tmp.Axes.Add(VerticalAxis = new LinearAxis
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
            tmp.Series.Add(_series);
            _model = tmp;

            _emptyDataPoint = new DataPoint(-1.0,-1.0);
            _newDataPoint = _emptyDataPoint;

            _dispatcher = Dispatcher.CurrentDispatcher;

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }

        public PlotModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public Brush Brush
        {
            get { return _series.Color.ToBrush(); }
            set { _series.Color = value.ToOxyColor(); }
        }

        public void AddPoint(DataPoint dataPoint)
        {
            _newDataPoint = dataPoint;
        }

        public void Clear()
        {
            _series.Points.Clear();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                _dispatcher.Invoke(() =>
                {
                    if (Equals(_newDataPoint, _emptyDataPoint)) return;

                    _series.Points.Add(_newDataPoint);
                    _model.InvalidatePlot(true);
                    RaisePropertyChanged(() => _model);

                    _newDataPoint = _emptyDataPoint;
                });
                Thread.Sleep(5);
            }
        }
    }
}