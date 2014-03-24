using System;
using System.Threading;

namespace _3880_80_FlashStation.PLC
{
    class PlcCommunicator : PlcCommunicatorBase
    {
        #region Variables
        
        //Private
        //Status
        private int _connectionStatus;
        private int _configurationStatus;

        //Configuration
        private PlcConfig _plcConfiguration;

        //Data buffers
        private byte[] _readBytes;
        private byte[] _writeBytes;

        // Error for read and write
        private int _errorReadByteNoDave;
        private int _errorWriteByteNoDave;

        // NoDave variables
        private libnodave.daveOSserialType _daveOSserialType;
        private libnodave.daveInterface _daveInterface;
        private libnodave.daveConnection _daveConnection;

        //Threads
        private readonly Thread _communicationWatchDogThread;
        private readonly Thread _dataAquisitionThread;

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

        public byte[] ReadBytes
        {
            get
            {
                if (_readBytes != null)
                {
                    return _readBytes;
                }
                throw new PlcException("Error: Plc communication is not configured.");
            }
        }

        public byte[] WriteBytes
        {
            get
            {
                if (_writeBytes != null)
                {
                    return _writeBytes;
                }
                throw new PlcException("Error: Plc communication is not configured.");
            }
        }

        #endregion

        #region Constructor

        public PlcCommunicator()
        {
            _readBytes = null;
            _writeBytes = null;

            //Init Properties
            _plcConfiguration = new PlcConfig();
            _connectionStatus = -1;
            _configurationStatus = -1;

            //Threads
            _communicationWatchDogThread = new Thread(WatchDog);
            _dataAquisitionThread = new Thread(DataAquisition);

            _communicationWatchDogThread.SetApartmentState(ApartmentState.STA);
            _communicationWatchDogThread.IsBackground = false;
            _communicationWatchDogThread.Start();

            _dataAquisitionThread.SetApartmentState(ApartmentState.STA);
            _dataAquisitionThread.IsBackground = false;
            _dataAquisitionThread.Start();
        }

        #endregion

        #region Methods

        public void SetupConnection(PlcConfigurator configurator)
        {
            if (configurator.PlcConfiguration.PlcConfigurationStatus != 1)
            {
                _connectionStatus = -1;
            }
            else
            {
                _plcConfiguration = configurator.PlcConfiguration;
                _readBytes = new byte[_plcConfiguration.PlcReadLength];
                _writeBytes = new byte[_plcConfiguration.PlcWriteLength];
                _connectionStatus = 1;
            }
        }

        public void OpenConnection()
        {
            // Check if configuration is done
            if (_configurationStatus != 1)
            {
                throw new PlcException("Error: Plc communication is not configured.");
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
                        _connectionStatus = 1;
                    else
                        _connectionStatus = -1;
                }
                else
                {
                    throw new PlcException("Error: Can not open connection to PLC.");
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

        private void DataAquisition()
        {
            while (_dataAquisitionThread.IsAlive)
            {
                if (_connectionStatus == 1)
                {
                    // Reading...
                    _errorReadByteNoDave = _daveConnection.readManyBytes(libnodave.daveDB, _plcConfiguration.PlcReadDbNumber, _plcConfiguration.PlcReadStartAddress, _plcConfiguration.PlcReadLength, _readBytes);
                    if (_errorReadByteNoDave != 0)
                        throw new PlcException("Error: Can not read data from PLC.");
                    // Writeing...
                    _errorWriteByteNoDave = _daveConnection.writeManyBytes(libnodave.daveDB, _plcConfiguration.PlcWriteDbNumber, _plcConfiguration.PlcWriteStartAddress, _plcConfiguration.PlcWriteLength, _writeBytes);
                    if (_errorWriteByteNoDave != 0)
                        throw new PlcException("Error: Can not write data to PLC.");
                }

                Thread.Sleep(100);
            }
        }

        private void WatchDog()
        {
            while (_communicationWatchDogThread.IsAlive)
            {
                if (_daveConnection != null && _daveConnection.connectPLC() == 0)
                    _connectionStatus = 1;
                else
                    _connectionStatus = -1;

                Thread.Sleep(100);
            }
        }

        #endregion
    }

    #region Auxiliaries

    public class PlcException : ApplicationException
    {
        public PlcException(string info) : base(info) { }
    }

    #endregion
}
