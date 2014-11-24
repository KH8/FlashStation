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
    public abstract class MainViewModel : Observable, ICloneable
    {
        #region Variables

        private PlotModel _model;
        private LineSeries _series;

        private readonly DataPoint _emptyDataPoint;
        private DataPoint _newDataPoint;

        private readonly Dispatcher _dispatcher;
        private readonly Thread _updateThread;

        private Boolean _refreshPlot ;

        #endregion

        #region Properties

        public double TimeRange { get; set; }

        public abstract TimeSpanAxis HorizontalAxis { get; set; }
        public abstract LinearAxis VerticalAxis { get; set; }

        public LineSeries Series
        {
            get { return _series; }
            set { _series = value; }
        }

        public PlotModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public Brush Brush
        {
            get { return _series.Color.ToBrush(); }
            set
            {
                _series.Color = value.ToOxyColor();
                _model.InvalidatePlot(true);
            }
        }

        #endregion


        #region Constructors

        protected MainViewModel()
        {
            _model = new PlotModel
            {
                IsLegendVisible = false,
                DefaultFontSize = 0,
                PlotMargins = new OxyThickness(20, 0, 0, 10)
            };
            _series = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 1
            };
            _model.Series.Add(_series);

            _emptyDataPoint = new DataPoint(-1.0, -1.0);
            _newDataPoint = _emptyDataPoint;

            _dispatcher = Dispatcher.CurrentDispatcher;

            TimeRange = 10000;
            _refreshPlot = true;

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }

        #endregion

        #region Methods

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

        public void SynchronizeView()
        {
            var timeDiff = (HorizontalAxis.ActualMaximum - HorizontalAxis.ActualMinimum) / 2.0;
            var timeTick = HorizontalAxis.ActualMinimum + timeDiff;

            SynchronizeView(timeTick);
        }

        public void SynchronizeView(double timeTick)
        {
            HorizontalAxis.Reset();
            HorizontalAxis.Minimum = timeTick - TimeRange / 2000.0;
            HorizontalAxis.Maximum = timeTick + TimeRange / 2000.0;

            _refreshPlot = true;
        }

        public void SynchronizeView(double minimum, double maximum)
        {
            HorizontalAxis.Reset();
            HorizontalAxis.Minimum = minimum;
            HorizontalAxis.Maximum = maximum;

            _refreshPlot = true;
        }

        #endregion


        #region Background Methods

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                _dispatcher.Invoke(() =>
                {
                    if (Equals(_newDataPoint, _emptyDataPoint)) return;
                    _series.Points.Add(_newDataPoint);
                    SynchronizeView(_newDataPoint.X);
                    _newDataPoint = _emptyDataPoint;
                });
                Thread.Sleep(5);
            }
        }

        public void UpdateView()
        {
            _dispatcher.Invoke(() =>
            {
                if (!_refreshPlot) return;
                _model.InvalidatePlot(true);
                _refreshPlot = false;

                RaisePropertyChanged(() => _model);
            });
        }

        #endregion
    }

    public class DataMainViewModel : MainViewModel
    {
        #region Properties

        public override sealed TimeSpanAxis HorizontalAxis { get; set; }
        public override sealed LinearAxis VerticalAxis { get; set; }

        #endregion


        #region Constructors

        public DataMainViewModel()
        {
            HorizontalAxis = new TimeSpanAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsAxisVisible = true,
                IsPanEnabled = false,
                IsZoomEnabled = false,
                TextColor = OxyColor.Parse("#FFF6F6F6"),
                Position = AxisPosition.Bottom
            };
            VerticalAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left
            };

            Model.Axes.Add(HorizontalAxis);
            Model.Axes.Add(VerticalAxis);
        }

        #endregion

    }

    public class TimeMainViewModel : MainViewModel
    {
        #region Properties

        public override sealed TimeSpanAxis HorizontalAxis { get; set; }
        public override sealed LinearAxis VerticalAxis { get; set; }

        #endregion


        #region Constructors

        public TimeMainViewModel()
        {
            HorizontalAxis = new TimeSpanAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                StringFormat = "hh:mm:ss",
                MajorStep = 1,
                MinorStep = 0.1,
                TextColor = OxyColors.Black,
                Position = AxisPosition.Bottom
            };
            VerticalAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false
            };

            Model.Axes.Add(HorizontalAxis);
            Model.Axes.Add(VerticalAxis);
        }

        #endregion
    }
}