// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   http://oxyplot.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using AmpIdent.Visual;
using OxyPlot;
using OxyPlot.Series;

namespace _ttAgent.Analyzer
{
    public class MainViewModel : Observable
    {
        private PlotModel _model;
        readonly LineSeries _series;
        readonly PlotModel _tmp;

        public MainViewModel()
        {
            // Create the plot model
            _tmp = new PlotModel("Test Plot", "powered by OxyPlot")
            {
                IsLegendVisible = false,
                TitleFontSize = 0,
                Title = "",
                SubtitleFontSize = 0
            };

            _series = new LineSeries("Data Series") { MarkerType = MarkerType.Circle, MarkerSize = 1 };
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