using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using _PlcAgent.Annotations;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerChannel : AnalyzerComponent
    {
        #region Constructors

        public AnalyzerChannel(uint id, Analyzer analyzer)
            : base(analyzer)
        {
            Id = id;
        }

        public uint Id { get; set; }

        #endregion


        #region Properties

        public AnalyzerObservableVariable AnalyzerObservableVariable { get; set; }

        #endregion


        #region Event Handlers

        protected override void OnRecordingChanged()
        {}

        protected override void OnRecordingTimeChanged()
        {}

        #endregion

    }

    public class AnalyzerChannelList : AnalyzerChannel
    {
        #region Properties

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

        public delegate void ChannelListModifiedDelegate();
        public ChannelListModifiedDelegate OnChannelListModified;

        #endregion

        #region Constructors

        #region Methods

        public AnalyzerChannelList(uint id, Analyzer analyzer) : base(id, analyzer)
        {
            Children = new List<AnalyzerChannel>();
            AnalyzerSetupFile = analyzer.AnalyzerSetupFile;
        }

        #endregion


        public void Add(AnalyzerChannel analyzerChannel)
        {
            Children.Add(analyzerChannel);
            StoreConfiguration();
            if (OnChannelListModified != null) OnChannelListModified();
        }

        public void Remove(AnalyzerChannel analyzerChannel)
        {
            Children.Remove(analyzerChannel);
            StoreConfiguration();
            if (OnChannelListModified != null) OnChannelListModified();
        }

        public void Clear()
        {
            foreach (var analyzerChannel in Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
            {
                analyzerChannel.AnalyzerObservableVariable.Clear();
            }
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
            foreach (
                var channelStrings in
                    AnalyzerSetupFile.Channels[Analyzer.Header.Id].Where(channel => channel != null)
                        .Select(channel => channel.Split('%')))
            {
                if (channelStrings[1] != "Empty")
                {
                    var newChannel = new AnalyzerChannel(Convert.ToUInt32(channelStrings[0]), Analyzer)
                    {
                        AnalyzerObservableVariable =
                            new AnalyzerObservableVariable(Analyzer,
                                Analyzer.CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(
                                    channelStrings[1]))
                            {
                                Name = channelStrings[2],
                            }
                    };
                    newChannel.AnalyzerObservableVariable.Unit = channelStrings[4];
                    newChannel.AnalyzerObservableVariable.Brush =
                        (Brush) new BrushConverter().ConvertFromString(channelStrings[5]);

                    Children.Add(newChannel);
                }
                else
                {
                    Children.Add(new AnalyzerChannel(Convert.ToUInt32(channelStrings[0]), Analyzer));
                }
            }
        }

        #endregion

    }
}