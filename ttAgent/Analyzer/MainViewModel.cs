// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   http://oxyplot.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot;
using OxyPlot.Series;

namespace AmpIdent.Visual
{
    public class MainViewModel : Observable
    {
        private PlotModel _model;
        readonly LineSeries[] _series;
        readonly PlotModel _tmp;

        public MainViewModel()
        {
            // Create the plot model
            _tmp = new PlotModel("Simple example", "using OxyPlot");
            // Create two line series (markers are hidden by default)

            _series = new LineSeries[10];

            for (var i = 1; i <= 9; i++)
            {
                _series[i] = new LineSeries("Series " +i) {MarkerType = MarkerType.Circle, MarkerSize = 1};
                // Add the series to the plot model
                _tmp.Series.Add(_series[i]);
            }
        }

        public PlotModel Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    RaisePropertyChanged(() => Model);
                }
            }
        }

        public void AddPoint(int i, DataPoint dataPoint)
        {
            _series[i].Points.Add(dataPoint);

            // Axes are created automatically if they are not defined
            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            Model = _tmp;
        }

        public void Clear()
        {
            for (var i = 1; i <= 9; i++)
            {
                _series[i].Points.Clear();
            }
        }
    }
}