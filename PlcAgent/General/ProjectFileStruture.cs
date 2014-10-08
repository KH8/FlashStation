using System;

namespace _PlcAgent.General
{
    public abstract class ProjectFileStruture
    {
        [Serializable]
        public struct ProjectSavedData
        {
            public uint[][] PlcCommunicators;
            public uint[][] CommunicationInterfaceHandlers;
            public uint[][] OutputDataTemplates;
            public uint[][] OutputHandlers;
            public uint[][] VFlashTypeBanks;
            public uint[][] VFlashHandlers;
            public uint[][] Analyzers;

            public PLC.PlcCommunicator.PlcConfig[] Configuration;
            public Boolean[] ConnectAtStartUp;

            public string[] Path;
            public int[] ConfigurationStatus;

            public string[] TemplatePaths;

            public string[] FileNameSuffixes;
            public int[] StartAddress;
            public int[] EndAddress;
            public int[] SelectedIndex;
            public string[] DirectoryPaths;
            public string[][] OutputHandlerAssignment;

            public string[][] TypeBank;
            public string[][] VFlashHandlerAssignment;

            public string[][] AnalyzerAssignment;

            public int[] SampleTime;
            public double[] TimeRange;
            public int[] NumberOfChannels;
            public string[][] Channels;
            public Boolean[] ShowDataCursors;

        }
    }
}
