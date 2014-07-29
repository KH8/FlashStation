using System;
using System.Windows.Media;
using OxyPlot;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerObservableVariable
    {
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
            get { return MainViewModel.Brush; }
            set { MainViewModel.Brush = value; }
        }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double ValueY { get; set; }
        public double ValueX { get; set; }

        public MainViewModel MainViewModel { get; set; }

        public AnalyzerObservableVariable(CommunicationInterfaceVariable communicationInterfaceVariable)
        {
            CommunicationInterfaceVariable = communicationInterfaceVariable;
            Name = communicationInterfaceVariable.Name;
            Type = GetType(CommunicationInterfaceVariable);
            Unit = "1";

            MinValue = 0.0;
            MaxValue = 0.0;

            MainViewModel = new MainViewModel();
        }

        public void StoreActualValue()
        {
            if (CommunicationInterfaceVariable == null) return;

            ValueY = GetValue(CommunicationInterfaceVariable);
            ValueX = DateTime.Now.TimeOfDay.TotalMilliseconds;

            if (ValueY > MaxValue) MaxValue = ValueY;
            if (ValueY < MinValue) MinValue = ValueY;

            MainViewModel.AddPoint(new DataPoint(ValueX, ValueY));
            MainViewModel.Model.InvalidatePlot(true);
        }

        public void Clear()
        {
            MainViewModel.Clear();
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
