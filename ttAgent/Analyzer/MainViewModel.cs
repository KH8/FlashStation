// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   http://oxyplot.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing;
using System.Web.UI.WebControls;
using AmpIdent.Visual;
using OxyPlot;
using OxyPlot.Wpf;
using Axis = OxyPlot.Axes.Axis;
using Brush = System.Windows.Media.Brush;
using LineSeries = OxyPlot.Series.LineSeries;

namespace _ttAgent.Analyzer
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
            };
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