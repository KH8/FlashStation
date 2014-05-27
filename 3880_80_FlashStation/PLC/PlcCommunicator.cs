using System;
using System.Threading;
using _3880_80_FlashStation.Log;

namespace _3880_80_FlashStation.PLC
{
    public class PlcCommunicator : PlcCommunicatorBase
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
                Logger.Log("Connection failed: Plc communication is not configured.");
                throw new PlcException("Error: Plc communication is not configured.");
            }
            set { _writeBytes = value; }
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
            _communicationWatchDogThread.IsBackground = true;
            _communicationWatchDogThread.Start();

            _dataAquisitionThread.SetApartmentState(ApartmentState.STA);
            _dataAquisitionThread.IsBackground = true;
            _dataAquisitionThread.Start();
        }

        #endregion

        #region Methods

        public void SetupConnection(PlcConfig configuration)
        {
            if (configuration.PlcConfigurationStatus != 1)
            {
                _configurationStatus = -1;
            }
            else
            {
                _plcConfiguration = configuration;
                _readBytes = new byte[_plcConfiguration.PlcReadLength];
                _writeBytes = new byte[_plcConfiguration.PlcWriteLength];
                _configurationStatus = 1;
            }
        }

        public void OpenConnection()
        {
            // Check if configuration is done
            if (_configurationStatus != 1)
            {
                Logger.Log("Connection failed: Plc communication is not configured.");
                throw new PlcException("Error: Plc communication is not configured.");
            }
            // Open connection only if was closed
            if (_connectionStatus != 1)
            {
                _daveOSserialType.rfd = libnodave.openSocket(_plcConfiguration.PlcPortNumber, _plcConfiguration.PlcIpAddress);
                _daveOSserialType.wfd = _daveOSserialType.rfd;
                if (_daveOSserialType.rfd > 0)
                {
                    _daveInterface = new libnodave.daveInterface(_daveOSserialType, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);
                    _daveInterface.setTimeout(10000); // orginal was:1000000
                    _daveConnection = new libnodave.daveConnection(_daveInterface, 0, _plcConfiguration.PlcRackNumber, _plcConfiguration.PlcSlotNumber);

                    if (_daveConnection.connectPLC() == 0)
                    {
                        _connectionStatus = 1;
                        Logger.Log("Communication with PLC IP Address : " +
                                   _plcConfiguration.PlcIpAddress + " established");
                    }
                    else
                        _connectionStatus = -1;
                }
                else
                {
                    Logger.Log("Connection failed: Can not open connection to PLC.");
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
            Logger.Log("Communication with PLC IP Address : " + _plcConfiguration.PlcIpAddress + " was closed");
        }

        private void DataAquisition()
        {
            while (_dataAquisitionThread.IsAlive)
            {
                if (_connectionStatus == 1)
                {
                    // Reading...
                    _errorReadByteNoDave = _daveConnection.readManyBytes(libnodave.daveDB, _plcConfiguration.PlcReadDbNumber, _plcConfiguration.PlcReadStartAddress, _plcConfiguration.PlcReadLength, _readBytes);   
                    //todo buffer
                    // Writeing...
                    _errorWriteByteNoDave = _daveConnection.writeManyBytes(libnodave.daveDB, _plcConfiguration.PlcWriteDbNumber, _plcConfiguration.PlcWriteStartAddress, _plcConfiguration.PlcWriteLength, _writeBytes);
                    //todo buffer
                }
                Thread.Sleep(100);
            }
        }

        private void WatchDog()
        {
            while (_communicationWatchDogThread.IsAlive)
            {
                // Reading...
                if (_errorReadByteNoDave != 0)
                {
                    CloseConnection();
                    _connectionStatus = -2;
                    Logger.Log("Communication with PLC IP Address : " + _plcConfiguration.PlcIpAddress + " was broken");
                }
                // Writeing...
                if (_errorWriteByteNoDave != 0)
                {
                    CloseConnection();
                    _connectionStatus = -2;
                    //throw new PlcException("Error: Can not write data to PLC.");
                    Logger.Log("Communication with PLC IP Address : " + _plcConfiguration.PlcIpAddress + " was broken");
                }
                if (_connectionStatus == -2)
                {
                    try { OpenConnection();}
                    catch (Exception e) { Console.WriteLine(e);}
                }
                Thread.Sleep(2000);
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
