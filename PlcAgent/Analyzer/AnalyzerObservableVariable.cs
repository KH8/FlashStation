using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Series;
using _PlcAgent.Annotations;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerObservableVariable : AnalyzerComponent, INotifyPropertyChanged
    {
        #region Variables

        private CommunicationInterfaceVariable _communicationInterfaceVariable;

        private VariableType _type;
        private string _name;
        private string _unit;

        private double _minValue;
        private double _maxValue;

        private MainViewModel _mainViewModel;

        #endregion


        #region Properties

        public enum VariableType
        {
            Bit,
            Byte,
            Integer,
            DoubleInteger,
            Real
        }

        public CommunicationInterfaceVariable CommunicationInterfaceVariable
        {
            get { return _communicationInterfaceVariable; }
            set
            {
                if (Equals(value, _communicationInterfaceVariable)) return;
                _communicationInterfaceVariable = value;
                OnPropertyChanged();
            }
        }

        public VariableType Type
        {
            get { return _type; }
            set
            {
                if (Equals(value, _type)) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public new string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Unit
        {
            get { return _unit; }
            set
            {
                if (value == _unit) return;
                _unit = value;
                OnPropertyChanged();
            }
        }

        public Brush Brush
        {
            get { return _mainViewModel.Brush; }
            set
            {
                if (Equals(value, _mainViewModel.Brush)) return;
                _mainViewModel.Brush = value;
                OnPropertyChanged();
            }
        }

        public double TimeRange
        {
            get { return _mainViewModel.TimeRange; }
            set
            {
                _mainViewModel.TimeRange = value;
                _mainViewModel.SynchronizeView();
            }
        }

        public double MinValue
        {
            get { return _minValue; }
            set
            {
                if (Equals(value, _minValue)) return;
                _minValue = value;
                OnPropertyChanged();
            }
        }
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (Equals(value, _maxValue)) return;
                _maxValue = value;
                OnPropertyChanged();
            }
        }

        public double ValueY { get; set; }
        public double ValueX { get; set; }

        public MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
            set { _mainViewModel = value; }
        }

        public MainViewModel MainViewModelClone
        {
            get { return (MainViewModel)_mainViewModel.Clone(); }
            set { _mainViewModel = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        #region Constructors

        public AnalyzerObservableVariable(Analyzer analyzer,
            CommunicationInterfaceVariable communicationInterfaceVariable)
            : base(analyzer)
        {
            CommunicationInterfaceVariable = communicationInterfaceVariable;
            Name = communicationInterfaceVariable.Name;
            Type = GetType(CommunicationInterfaceVariable);
            Unit = "1";

            MinValue = 0.0;
            MaxValue = 0.0;

            _mainViewModel = new DataMainViewModel();
        }

        #endregion


        #region Methods

        public void Clear()
        {
            _mainViewModel.Clear();
        }

        public void StoreActualValue(double valueX)
        {
            if (CommunicationInterfaceVariable == null) return;

            ValueY = new Random().NextDouble(); //Convert.ToDouble(CommunicationInterfaceVariable.Value);
            ValueX = valueX;

            if (ValueY > MaxValue) MaxValue = ValueY;
            if (ValueY < MinValue) MinValue = ValueY;

            _mainViewModel.AddPoint(new DataPoint(ValueX, ValueY));
        }

        public double GetValue(double valueX, double tolerance)
        {
            var result = Double.NaN;

            if (CommunicationInterfaceVariable == null) return result;

            foreach (var lineSerie in _mainViewModel.Model.Series.Cast<LineSeries>())
            {
                try
                {
                    foreach ( var point in lineSerie.Points.Where( point => point.X >= valueX - tolerance && point.X <= valueX + tolerance))
                    { result = point.Y; }
                }
                catch (Exception) { result = Double.NaN;}
            }
            return result;
        }

        public double GetTimePosition(double positionPercentage)
        {
            return TimeRange * positionPercentage + _mainViewModel.HorizontalAxis.Minimum * 1000.0;
        }

        private static VariableType GetType(CommunicationInterfaceComponent communicationInterfaceVariable)
        {
            switch (communicationInterfaceVariable.Type)
            {
                case CommunicationInterfaceComponent.VariableType.Bit:
                    return VariableType.Bit;
                case CommunicationInterfaceComponent.VariableType.Byte:
                    return VariableType.Byte;
                case CommunicationInterfaceComponent.VariableType.Integer:
                    return VariableType.Integer;
                case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                    return VariableType.DoubleInteger;
                case CommunicationInterfaceComponent.VariableType.Real:
                    return VariableType.Real;
                default:
                    throw new Exception("This type of CommunicationInterfaceVariable is not handled");
            }
        }

        #endregion


        #region Event Handlers

        protected override void OnRecordingChanged()
        {}

        protected override void OnRecordingTimeChanged()
        {}

        protected override void OnDataCursorsVisibilityChanged()
        {}

        #endregion
    }
}
