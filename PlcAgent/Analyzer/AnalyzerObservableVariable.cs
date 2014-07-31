using System;
using System.Windows.Media;
using OxyPlot;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerObservableVariable
    {
        private MainViewModel _mainViewModel;

        public enum VariableType
        {
            Bit,
            Byte,
            Integer,
            DoubleInteger,
            Real
        }

        public CommunicationInterfaceVariable CommunicationInterfaceVariable { get; set; }

        public VariableType Type { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }

        public Brush Brush
        {
            get { return _mainViewModel.Brush; }
            set { _mainViewModel.Brush = value; }
        }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double ValueY { get; set; }
        public double ValueX { get; set; }

        public MainViewModel MainViewModel
        {
            get { return (MainViewModel) _mainViewModel.Clone(); }
            set { _mainViewModel = value; }
        }

        public AnalyzerObservableVariable(CommunicationInterfaceVariable communicationInterfaceVariable)
        {
            CommunicationInterfaceVariable = communicationInterfaceVariable;
            Name = communicationInterfaceVariable.Name;
            Type = GetType(CommunicationInterfaceVariable);
            Unit = "1";

            MinValue = 0.0;
            MaxValue = 0.0;

            _mainViewModel = new MainViewModel();
        }

        public void StoreActualValue(double valueX)
        {
            if (CommunicationInterfaceVariable == null) return;

            ValueY = GetValue(CommunicationInterfaceVariable);
            ValueX = valueX;

            if (ValueY > MaxValue) MaxValue = ValueY;
            if (ValueY < MinValue) MinValue = ValueY;

            _mainViewModel.AddPoint(new DataPoint(ValueX, ValueY));
        }

        public void Clear()
        {
            _mainViewModel.Clear();
        }

        private static double GetValue(CommunicationInterfaceVariable communicationInterfaceVariable)
        {
            switch (communicationInterfaceVariable.Type)
            {
                case CommunicationInterfaceComponent.VariableType.Bit:
                    var componentBit = (CiBit) communicationInterfaceVariable;
                    return Convert.ToDouble(componentBit.Value);
                case CommunicationInterfaceComponent.VariableType.Byte:
                    var componentByte = (CiByte) communicationInterfaceVariable;
                    return Convert.ToDouble(componentByte.Value);
                case CommunicationInterfaceComponent.VariableType.Integer:
                    var componentInteger = (CiInteger) communicationInterfaceVariable;
                    return Convert.ToDouble(componentInteger.Value);
                case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                    var componentDoubleInteger = (CiDoubleInteger) communicationInterfaceVariable;
                    return Convert.ToDouble(componentDoubleInteger.Value);
                case CommunicationInterfaceComponent.VariableType.Real:
                    var componentReal = (CiReal) communicationInterfaceVariable;
                    return Convert.ToDouble(componentReal.Value);
                default:
                    throw new Exception("This type of CommunicationInterfaceVariable is not handled");
            }
        }

        private static VariableType GetType(CommunicationInterfaceVariable communicationInterfaceVariable)
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
    }
}
