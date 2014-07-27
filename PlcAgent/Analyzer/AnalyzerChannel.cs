using System.Collections.Generic;

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

    public class AnalyzerChannelList : AnalyzerChannel
    {
        public List<AnalyzerChannel> Children { get; set; } 

        public AnalyzerChannelList(uint id) : base(id)
        {
            Children = new List<AnalyzerChannel>();
            RetriveConfiguration();
        }

        public void Add(AnalyzerChannel analyzerChannel)
        {
            Children.Add(analyzerChannel);
            StoreConfiguration();
        }

        public void Remove(AnalyzerChannel analyzerChannel)
        {
            Children.Remove(analyzerChannel);
            StoreConfiguration();
        }

        private void StoreConfiguration()
        {
            
        }

        private void RetriveConfiguration()
        {
            
        }
    }
}