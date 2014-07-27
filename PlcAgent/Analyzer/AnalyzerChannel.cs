using System.Windows.Media;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerChannel
    {
        private uint _id;
        private AnalyzerObservableVariable _analyzerObservableVariable;

        public AnalyzerChannel(uint id)
        {
            _id = id;
        }

        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public AnalyzerObservableVariable AnalyzerObservableVariable
        {
            get { return _analyzerObservableVariable; }
            set { _analyzerObservableVariable = value; }
        }
    }
}