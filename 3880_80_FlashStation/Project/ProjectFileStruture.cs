using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using _3880_80_FlashStation.MainRegistry;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Project
{
    public abstract class ProjectFileStruture
    {
        [Serializable]
        public struct ProjectSavedData
        {
            public uint[][] PlcCommunicators;
            public uint[][] CommunicationInterfaceHandlers;
            public uint[][] OutputWriters;
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
