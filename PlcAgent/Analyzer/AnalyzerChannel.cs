using System;
using System.Collections.Generic;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerChannel
    {
        private uint _id;
        private Analyzer _analyzer;
        private AnalyzerObservableVariable _analyzerObservableVariable;

        public AnalyzerChannel(uint id, Analyzer analyzer)
        {
            _id = id;
            _analyzer = analyzer;
        }

        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Analyzer Analyzer
        {
            get { return _analyzer; }
            set { _analyzer = value; }
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

        public AnalyzerSetupFile AnalyzerSetupFile { get; set; }

        public AnalyzerChannelList(uint id, Analyzer analyzer) : base(id, analyzer)
        {
            Children = new List<AnalyzerChannel>();
            AnalyzerSetupFile = analyzer.AnalyzerSetupFile;

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

        public void StoreConfiguration()
        {
            AnalyzerSetupFile.Channels = new string[1];
            AnalyzerSetupFile.Channels = new string[AnalyzerSetupFile.NumberOfChannels[Analyzer.Header.Id] + 1];
            foreach (var analyzerChannel in Children)
            {
                if (analyzerChannel.AnalyzerObservableVariable == null)
                {
                    AnalyzerSetupFile.Channels[analyzerChannel.Id] = "Empty Channel";
                }
                else
                {
                    AnalyzerSetupFile.Channels[analyzerChannel.Id] =
                    analyzerChannel.AnalyzerObservableVariable.CommunicationInterfaceVariable.Name + "%" +
                    analyzerChannel.AnalyzerObservableVariable.Name + "%" +
                    analyzerChannel.AnalyzerObservableVariable.Type + "%" +
                    analyzerChannel.AnalyzerObservableVariable.Unit + "%" +
                    analyzerChannel.AnalyzerObservableVariable.Brush;
                }
            }
            AnalyzerSetupFile.Save();
        }

        private void RetriveConfiguration()
        {
            
        }
    }
}