using System;
using _ttAgent.PLC;

namespace _ttAgent.Project
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

            public PlcCommunicatorBase.PlcConfig[] Configuration;
            public Boolean[] ConnectAtStartUp;

            public string[] Path;
            public int[] ConfigurationStatus;

            public int[] StartAddress;
            public int[] EndAddress;
            public int[] SelectedIndex;

            public string[][] TypeBank;
        }
    }
}
