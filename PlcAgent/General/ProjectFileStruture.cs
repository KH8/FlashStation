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
            public uint[][] OutputFileCreators;
            public uint[][] OutputHandlers;
            public uint[][] DbConnectionHandlers;
            public uint[][] VFlashTypeBanks;
            public uint[][] VFlashHandlers;
            public uint[][] Analyzers;

            public PLC.PlcCommunicator.PlcConfig[] Configuration;
            public Boolean[] ConnectAtStartUp;

            public string[] Path;
            public int[] ConfigurationStatus;

            public string[] TemplatePaths;

            public string[] OutputFileCreatorFileNameSuffixes;
            public int[] OutputFileCreatorSelectedIndex;
            public string[] OutputFileCreatorDirectoryPaths;
            public string[][] OutputFileCreatorAssignment;

            public string[] OutputHandlerFileNameSuffixes;
            public int[] OutputHandlerStartAddress;
            public int[] OutputHandlerEndAddress;
            public int[] OutputHandlerSelectedIndex;
            public string[] OutputHandlerDirectoryPaths;
            public string[][] OutputHandlerAssignment;

            public string[] DbConnectionHandlerFileDbInstances;
            public string[] DbConnectionHandlerFileInitialCatalogs;
            public string[] DbConnectionHandlerFileConfigurationFileNames;
            public string[][] DbConnectionHandlerAssignment;

            public string[][] TypeBank;
            public string[][][] Steps;
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
