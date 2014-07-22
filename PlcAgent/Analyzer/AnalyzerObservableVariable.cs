using System;
using System.Drawing;
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
        public Brush Brush { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public double Value { get; set; }

        public MainViewModel MainViewModel { get; set; }

        public AnalyzerObservableVariable(CommunicationInterfaceVariable communicationInterfaceVariable)
        {
            CommunicationInterfaceVariable = communicationInterfaceVariable;
            Name = communicationInterfaceVariable.Name;
            Type = GetType(CommunicationInterfaceVariable);
            Unit = "1";

            MainViewModel = new MainViewModel();
            //StoreActualValue();
        }

        public void StoreActualValue()
        {
            if (CommunicationInterfaceVariable == null) return;
            Value = GetValue(CommunicationInterfaceVariable);
            MainViewModel.AddPoint(new DataPoint(DateTime.Now.TimeOfDay.TotalMilliseconds, Value));
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
