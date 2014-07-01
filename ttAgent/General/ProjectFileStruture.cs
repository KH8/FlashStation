using System;
using _ttAgent.PLC;

namespace _ttAgent.General
{
    public abstract class ProjectFileStruture
    {
        [Serializable]
        public struct ProjectSavedData
        {
            public uint[][] PlcCommunicators;
            public uint[][] CommunicationInterfaceHandlers;
            public uint[][] OutputHandlers;
            public uint[][] VFlashTypeBanks;
            public uint[][] VFlashHandlers;

            public PlcCommunicator.PlcConfig[] Configuration;
            public Boolean[] ConnectAtStartUp;

            public string[] Path;
            public int[] ConfigurationStatus;

            public string[] FileNameSuffixes;
            public int[] StartAddress;
            public int[] EndAddress;
            public int[] SelectedIndex;
            public string[][] OutputHandlerAssignment;

            public string[][] TypeBank;
            public string[][] VFlashHandlerAssignment;
        }
    }
}
