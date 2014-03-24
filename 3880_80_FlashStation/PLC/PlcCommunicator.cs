using _3880_80_FlashStation.Configuration;

namespace _3880_80_FlashStation.PLC
{
    class PlcCommunicator
    {
        #region Properties
        
        //Private
        //Status
        private int _connectionStatus;
        private int _configurationStatus;

        //Configuration
        private PlcConfig _plcConfiguration;

        //Data buffers
        private byte[] _readBufferBytes;
        private byte[] _writeBufferBytes;

        //Public
        //Status
        public int ConnectionStatus
        {
            get { return _connectionStatus; }
            set { _connectionStatus = value; }
        }

        public int ConfigurationStatus
        {
            get { return _configurationStatus; }
            set { _configurationStatus = value; }
        }

        public PlcConfig PlcConfiguration
        {
            get { return _plcConfiguration; }
            set { _plcConfiguration = value; }
        }

        #endregion

        #region Structures

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
            public int PlcwriteStartAddress;
            public int PlcWriteLength;
            public int PlcConfigurationStatus;
        }

        #endregion

        #region Constructor

        public PlcCommunicator()
        {
            //Init Properties
            _plcConfiguration = new PlcConfig();
            _connectionStatus = -1;
            _configurationStatus = -1;
        }

        #endregion

        #region Methods

        public void SetupConnection(PlcConfig config)
        {
            _plcConfiguration = config;
            //to do: be done verification
            _connectionStatus = 1;
        }



        #endregion


    }
}
