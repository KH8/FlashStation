using System;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using _3880_80_FlashStation.Configuration;

namespace _3880_80_FlashStation.PLC
{
    class PlcCommunicator
    {
        #region Variables
        
        //Private
        //Status
        private int _connectionStatus;
        private int _configurationStatus;

        //Configuration
        private PlcConfig _plcConfiguration;

        //Data buffers
        private byte[] _readBufferBytes;
        private byte[] _writeBufferBytes;

        // NoDave variables
        private libnodave.daveOSserialType _daveOSserialType;
        private libnodave.daveInterface _daveInterface;
        private libnodave.daveConnection _daveConnection;

        #endregion

        #region Properties

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

        public void OpenConnection()
        {
            // Check if configuration is done
            if (_configurationStatus != 1)
            {
                throw new PlcException("Plc communication is not configured.");
            }
            // Open connection only if was closed
            if (_connectionStatus == -1)
            {
                _daveOSserialType.rfd = libnodave.openSocket(_plcConfiguration.PlcPortNumber, _plcConfiguration.PlcIpAddress);
                _daveOSserialType.wfd = _daveOSserialType.rfd;
                if (_daveOSserialType.rfd > 0)
                {
                    _daveInterface = new libnodave.daveInterface(_daveOSserialType, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);
                    _daveInterface.setTimeout(10000); // orginal was:1000000
                    _daveConnection = new libnodave.daveConnection(_daveInterface, 0, _plcConfiguration.PlcRackNumber, _plcConfiguration.PlcSlotNumber);

                    if (_daveConnection.connectPLC() == 0)
                        _connectionStatus = 0;
                    else
                        _connectionStatus = -1;
                }
                else
                {
                    throw new PlcException("Can not open connection to plc.");
                }
            }
        }
        public void CloseConnection()
        {
            // Close connection only if was open
            if (_daveConnection != null)
            {
                _daveConnection.disconnectPLC();
                libnodave.closeSocket(_daveOSserialType.rfd);
            }
            _connectionStatus = -1;
        }

        #endregion

        #region Auxiliaries

        public class PlcException : ApplicationException
        {
            public PlcException(string info) : base(info)
            {
            }
        }

        #endregion

    }
}
