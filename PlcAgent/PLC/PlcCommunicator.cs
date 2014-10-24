using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using _PlcAgent.Log;
using _PlcAgent.MainRegistry;

namespace _PlcAgent.PLC
{
    public class PlcCommunicator : Module
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

        #region Variables
        
        //Private
        //Status
        private int _connectionStatus;
        private int _configurationStatus;

        //Serialization
        private int _serialize;
        private string _serializeFileName;

        //Configuration

        //Data buffers
        private byte[] _readBytes;
        private byte[] _writeBytes;
        private byte[] _readBytesBuffer;
        private byte[] _writeBytesBuffer;

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
            set
            {
                if (value == _connectionStatus) return;
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }

        public int ConfigurationStatus
        {
            get { return _configurationStatus; }
            set
            {
                _configurationStatus = value;
                OnPropertyChanged();
            }
        }

        //Configuration
        public PlcConfig PlcConfiguration { get; set; }

        public PlcConfigurationFile PlcConfigurationFile { get; set; }
        
        //Methods
        public byte[] ReadBytes
        {
            get
            {
                if (_readBytes != null)
                {
                    return _readBytes;
                }
                Logger.Log("ID: " + Header.Id + " Connection failed: Plc communication is not configured.");
                throw new PlcException("ID: " + Header.Id + " Error: Plc communication is not configured.");
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
                Logger.Log("ID: " + Header.Id + " Connection failed: Plc communication is not configured.");
                throw new PlcException("ID: " + Header.Id + " Error: Plc communication is not configured.");
            }
            set { _writeBytes = value; }
        }

        public override string Description
        {
            get { return Header.Name; }
        }

        #endregion


        #region Constructor

        public PlcCommunicator(uint id, string name, PlcConfigurationFile plcConfigurationFile) : base(id, name)
        {
            ReferencePosition = 1;

            _readBytes = null;
            _writeBytes = null;
            _readBytesBuffer = null;
            _writeBytesBuffer = null;

            //Init Properties
            PlcConfigurationFile = plcConfigurationFile;
            PlcConfiguration = plcConfigurationFile.Configuration[Header.Id];
            ConnectionStatus = -1;
            ConfigurationStatus = -1;

            //Threads
            _communicationWatchDogThread = new Thread(WatchDog);
            _dataAquisitionThread = new Thread(DataAquisition);

            _communicationWatchDogThread.SetApartmentState(ApartmentState.STA);
            _communicationWatchDogThread.IsBackground = true;

            _dataAquisitionThread.SetApartmentState(ApartmentState.STA);
            _dataAquisitionThread.IsBackground = true;
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            _serialize = 0;
        }

        public override void Deinitialize()
        {
            _serialize = 0;

            CloseConnection();
            _communicationWatchDogThread.Abort();
            _dataAquisitionThread.Abort();

            Logger.Log("ID: " + Header.Id + " PLC communication Deinitialized");
        }

        public void InitializeConnection()
        {
            _communicationWatchDogThread.Start();
            _dataAquisitionThread.Start();

            if (!PlcConfigurationFile.ConnectAtStartUp[Header.Id] || ConnectionStatus == 1)
            {
                Logger.Log("ID: " + Header.Id + " PLC communication Initialized");
                return;
            }
            ConnectionStatus = -2;
            Logger.Log("ID: " + Header.Id + " Connected with IP address " + PlcConfiguration.PlcIpAddress + " at start up");//*/
            Logger.Log("ID: " + Header.Id + " PLC communication initialized");
        }

        public void SetupConnection(PlcConfig configuration)
        {
            if (configuration.PlcConfigurationStatus != 1)
            {
                ConfigurationStatus = -1;
            }
            else
            {
                PlcConfiguration = configuration;
                _readBytesBuffer = new byte[PlcConfiguration.PlcReadLength];
                _writeBytesBuffer = new byte[PlcConfiguration.PlcWriteLength];
                _readBytes = new byte[PlcConfiguration.PlcReadLength];
                _writeBytes = new byte[PlcConfiguration.PlcWriteLength];
                ConfigurationStatus = 1;
            }
        }

        public void OpenConnection()
        {
            // Check if configuration is done
            if (ConfigurationStatus != 1)
            {
                Logger.Log("ID: " + Header.Id + " Connection failed: Plc communication is not configured.");
                throw new PlcException("ID: " + Header.Id + " Error: Plc communication is not configured.");
            }
            // Open connection only if was closed
            if (ConnectionStatus != 1)
            {
                _daveOSserialType.rfd = libnodave.openSocket(PlcConfiguration.PlcPortNumber, PlcConfiguration.PlcIpAddress);
                _daveOSserialType.wfd = _daveOSserialType.rfd;
                if (_daveOSserialType.rfd > 0)
                {
                    _daveInterface = new libnodave.daveInterface(_daveOSserialType, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);
                    _daveInterface.setTimeout(10000); // orginal was:1000000
                    _daveConnection = new libnodave.daveConnection(_daveInterface, 0, PlcConfiguration.PlcRackNumber, PlcConfiguration.PlcSlotNumber);

                    if (_daveConnection.connectPLC() == 0)
                    {
                        ConnectionStatus = 1;
                        Logger.Log("ID: " + Header.Id + " Communication with PLC IP Address : " +
                                   PlcConfiguration.PlcIpAddress + " established");
                    }
                    else
                        ConnectionStatus = -1;
                }
                else
                {
                    Logger.Log("ID: " + Header.Id + " Connection failed: Can not open connection to PLC.");
                    throw new PlcException("ID: " + Header.Id + " Error: Can not open connection to PLC.");
                }
            }
        }

        public void CloseConnection()
        {
            ConnectionStatus = -1;
            if (_daveConnection != null)
            {
                _daveConnection.disconnectPLC();
                libnodave.closeSocket(_daveOSserialType.rfd);
                _errorReadByteNoDave = 0;
                _errorWriteByteNoDave = 0;
            }
            Logger.Log("ID: " + Header.Id + " Communication with PLC IP Address : " + PlcConfiguration.PlcIpAddress + " was closed");
        }

        public void MakeABinary(string fileName)
        {
            _serializeFileName = fileName;
            _serialize = 1;
        }

        #endregion


        #region Background Methods

        private void DataAquisition()
        {
            while (_dataAquisitionThread.IsAlive)
            {
                if (ConnectionStatus == 1)
                {
                    // Reading...
                    _errorReadByteNoDave = _daveConnection.readManyBytes(libnodave.daveDB, PlcConfiguration.PlcReadDbNumber, PlcConfiguration.PlcReadStartAddress, PlcConfiguration.PlcReadLength, _readBytesBuffer);
                    if (_errorReadByteNoDave == 0) _readBytes = _readBytesBuffer;

                    if (_serialize == 1)
                    {
                        IFormatter formatter = new BinaryFormatter();

                        Stream stream = new FileStream(_serializeFileName,
                            FileMode.Create,
                            FileAccess.Write, FileShare.None);
                        formatter.Serialize(stream, _readBytesBuffer);
                        stream.Close();

                        Logger.Log("ID: " + Header.Id + " A binary file " + _serializeFileName + " was created.");
                        _serialize = 0;
                    }

                    Thread.Sleep(5);
                    // Writeing...
                    _errorWriteByteNoDave = _daveConnection.writeManyBytes(libnodave.daveDB, PlcConfiguration.PlcWriteDbNumber, PlcConfiguration.PlcWriteStartAddress, PlcConfiguration.PlcWriteLength, _writeBytesBuffer);
                    if (_errorWriteByteNoDave == 0) _writeBytes = _writeBytesBuffer;
                }
                Thread.Sleep(5);
            }
        }

        private void WatchDog()
        {
            while (_communicationWatchDogThread.IsAlive)
            {
                if (ConnectionStatus == -2)
                {
                    try { OpenConnection(); }
                    catch (Exception e) { Console.WriteLine(e); }
                }
                // Reading...
                if (_errorReadByteNoDave != 0 && ConnectionStatus != -1)
                {
                    CloseConnection();
                    if (ConnectionStatus != -2) Logger.Log("ID: " + Header.Id + " Communication with PLC IP Address : " + PlcConfiguration.PlcIpAddress + " was broken");
                    ConnectionStatus = -2;
                }
                // Writeing...
                if (_errorWriteByteNoDave != 0 && ConnectionStatus != -1)
                {
                    CloseConnection();
                    if (ConnectionStatus != -2) Logger.Log("ID: " + Header.Id + " Communication with PLC IP Address : " + PlcConfiguration.PlcIpAddress + " was broken");
                    ConnectionStatus = -2;
                }
                Thread.Sleep(1000);
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
