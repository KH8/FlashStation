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

        //public MainModelView

        protected AnalyzerObservableVariable(CommunicationInterfaceVariable communicationInterfaceVariable)
        {
            CommunicationInterfaceVariable = communicationInterfaceVariable;
        }

        public abstract void StoreActualValue();
        public abstract void Clear();
    }
}
