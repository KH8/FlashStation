using System;

namespace ttAgent.PLC
{
    public abstract class PlcCommunicatorBase
    {
        [Serializable]
        public struct PlcConfig
        {
            public string PlcIpAddress;
            public int PlcPortNumber;
            public int PlcRackNumber;
            public int PlcSlotNumber;
            public int PlcReadDbNumber;
            public int PlcReadStartAddress;
            public int PlcReadLength;
            public int PlcWriteDbNumber;
            public int PlcWriteStartAddress;
            public int PlcWriteLength;
            public int PlcConfigurationStatus;
        }
    }
}