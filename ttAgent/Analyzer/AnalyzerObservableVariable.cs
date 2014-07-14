using System;
using System.Drawing;
using OxyPlot;
using _ttAgent.DataAquisition;

namespace _ttAgent.Analyzer
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
            Type = AnalyzerObservableVariableBuilder.GetType(CommunicationInterfaceVariable);
            MainViewModel = new MainViewModel();
        }

        public void StoreActualValue()
        {
            var rand = new Random();
            Value = AnalyzerObservableVariableBuilder.GetValue(CommunicationInterfaceVariable);
            //MainViewModel.AddPoint(new DataPoint(DateTime.Now.TimeOfDay.TotalMilliseconds, Value));
            MainViewModel.AddPoint(new DataPoint(DateTime.Now.TimeOfDay.TotalMilliseconds, rand.NextDouble()));
            MainViewModel.Model.InvalidatePlot(true);
        }

        public void Clear()
        {
            MainViewModel.Clear();
        }
    }

    public static class AnalyzerObservableVariableBuilder
    {
        public static double GetValue(CommunicationInterfaceVariable communicationInterfaceVariable)
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

        public static AnalyzerObservableVariable.VariableType GetType(CommunicationInterfaceVariable communicationInterfaceVariable)
        {
            switch (communicationInterfaceVariable.Type)
            {
                case CommunicationInterfaceComponent.VariableType.Bit:
                    return AnalyzerObservableVariable.VariableType.Bit;
                case CommunicationInterfaceComponent.VariableType.Byte:
                    return AnalyzerObservableVariable.VariableType.Byte;
                case CommunicationInterfaceComponent.VariableType.Integer:
                    return AnalyzerObservableVariable.VariableType.Integer;
                case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                    return AnalyzerObservableVariable.VariableType.DoubleInteger;
                case CommunicationInterfaceComponent.VariableType.Real:
                    return AnalyzerObservableVariable.VariableType.Real;
                default:
                    throw new Exception("This type of CommunicationInterfaceVariable is not handled");
            }
        }
    }
}
