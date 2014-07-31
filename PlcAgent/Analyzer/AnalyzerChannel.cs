using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

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

        public uint HighestId
        {
            get
            {
                uint[] highestId = {0};
                foreach (var analyzerChannel in Children.Where(analyzerChannel => analyzerChannel.Id > highestId[0]))
                {
                    highestId[0] = analyzerChannel.Id;
                }
                return highestId[0];
            }
        }

        public AnalyzerSetupFile AnalyzerSetupFile { get; set; }

        public AnalyzerChannelList(uint id, Analyzer analyzer) : base(id, analyzer)
        {
            Children = new List<AnalyzerChannel>();
            AnalyzerSetupFile = analyzer.AnalyzerSetupFile;
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

        public AnalyzerChannel GetChannel(uint id)
        {
            return Children.FirstOrDefault(analyzerChannel => analyzerChannel.Id == id);
        }

        public void StoreConfiguration()
        {
            AnalyzerSetupFile.Channels[Analyzer.Header.Id] = new string[1];
            AnalyzerSetupFile.Channels[Analyzer.Header.Id] = new string[HighestId + 1];
            foreach (var analyzerChannel in Children)
            {
                if (analyzerChannel.AnalyzerObservableVariable == null)
                {
                    AnalyzerSetupFile.Channels[Analyzer.Header.Id][analyzerChannel.Id] =
                        analyzerChannel.Id + "%" + "Empty";
                }
                else
                {
                    AnalyzerSetupFile.Channels[Analyzer.Header.Id][analyzerChannel.Id] = 
                    analyzerChannel.Id + "%" +
                    analyzerChannel.AnalyzerObservableVariable.CommunicationInterfaceVariable.Name + "%" +
                    analyzerChannel.AnalyzerObservableVariable.Name + "%" +
                    analyzerChannel.AnalyzerObservableVariable.Type + "%" +
                    analyzerChannel.AnalyzerObservableVariable.Unit + "%" +
                    analyzerChannel.AnalyzerObservableVariable.Brush;
                }
            }
            AnalyzerSetupFile.Save();
        }

        public void RetriveConfiguration()
        {
            foreach (var channelStrings in AnalyzerSetupFile.Channels[Analyzer.Header.Id].Where(channel => channel != null).Select(channel => channel.Split('%')))
            {
                if (channelStrings[1] != "Empty")
                {
                    var newChannel = new AnalyzerChannel(Convert.ToUInt32(channelStrings[0]), Analyzer)
                    {
                        AnalyzerObservableVariable =
                            new AnalyzerObservableVariable(
                                Analyzer.CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(
                                    channelStrings[1]))
                            {
                                Name = channelStrings[2],
                            }
                    };
                    newChannel.AnalyzerObservableVariable.Unit = channelStrings[4];
                    newChannel.AnalyzerObservableVariable.Brush =
                        (Brush)new BrushConverter().ConvertFromString(channelStrings[5]);

                    Children.Add(newChannel);
                }
                else
                {
                    Children.Add(new AnalyzerChannel(Convert.ToUInt32(channelStrings[0]), Analyzer));
                }
            }
        }
    }
}