using System;
using System.Drawing;
using _ttAgent.DataAquisition;

namespace _ttAgent.Analyzer
{
    abstract class AnalyzerObservableVariable
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


        protected AnalyzerObservableVariable(CommunicationInterfaceVariable communicationInterfaceVariable)
        {
            CommunicationInterfaceVariable = communicationInterfaceVariable;

            switch (communicationInterfaceVariable.Type)
            {
                    case CommunicationInterfaceComponent.VariableType.Bit:
                        Type = VariableType.Bit;
                        break;
                    case CommunicationInterfaceComponent.VariableType.Byte:
                        Type = VariableType.Byte;
                        break;
                    case CommunicationInterfaceComponent.VariableType.Integer:
                        Type = VariableType.Integer;
                        break;
                    case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                        Type = VariableType.DoubleInteger;
                        break;
                    case CommunicationInterfaceComponent.VariableType.Real:
                        Type = VariableType.Real;
                        break;
                    default:
                        throw new Exception("This type of CommunicationInterfaceVariable is not handled.");
            }
        }
    }
}
