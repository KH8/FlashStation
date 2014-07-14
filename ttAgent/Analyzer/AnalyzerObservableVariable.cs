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

        public MainViewModel MainViewModel { get; set; }

        public AnalyzerObservableVariable()
        {
            //CommunicationInterfaceVariable = communicationInterfaceVariable;
            MainViewModel = new MainViewModel();
        }

        public void StoreActualValue()
        {
            var rand = new Random();
            MainViewModel.AddPoint(new DataPoint(DateTime.Now.TimeOfDay.TotalMilliseconds, rand.NextDouble()));
            MainViewModel.Model.InvalidatePlot(true);
        }

        public void Clear()
        {
            MainViewModel.Clear();
        }
    }
}
