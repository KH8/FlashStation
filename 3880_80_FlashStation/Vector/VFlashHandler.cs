using System;
using System.Collections.Generic;
using System.Threading;
using Vector.vFlash.Automation;
using _3880_80_FlashStation.DataAquisition;

namespace _3880_80_FlashStation.Vector
{
    class VFlashHandler
    {
        #region Variables

        public struct Channel
        {
            public string Command;
            public Boolean Result;
            public string Status;
            public string Path;
        }

        private readonly Channel[] _channels;

        private CommunicationInterfaceComposite _inputInterface;
        private CommunicationInterfaceComposite _outputInterface;

        private readonly List<VFlashChannelConfigurator> _channelsConfigurators;
        private readonly VFlashStationController _vFlashStationController;
        private readonly VFlashErrorCollector _errorCollector;

        private readonly Thread _vectorThread;

        #endregion

        #region Properties

        public CommunicationInterfaceComposite InputInterface
        {
            set { _inputInterface = value; }
        }

        public CommunicationInterfaceComposite OutputInterface
        {
            get { return _outputInterface; }
            set { _outputInterface = value; }
        }

        public Channel[] Channels
        {
            get { return _channels; }
        }

        public VFlashErrorCollector ErrorCollector
        {
            get { return _errorCollector; }
        }

        #endregion

        #region Constructor

        public VFlashHandler()
        {
            _channelsConfigurators = new List<VFlashChannelConfigurator>();
            _channels = new Channel[5];

            _errorCollector = new VFlashErrorCollector();
            _vFlashStationController = new VFlashStationController(ReportError);

            _vectorThread = new Thread(VectorBackgroundThread);
            _vectorThread.SetApartmentState(ApartmentState.STA);
            _vectorThread.IsBackground = true;
            _vectorThread.Start();
        }

        #endregion

        #region Methods

        public void AddChannelSetup(string projectPath, uint chanId)
        {
            foreach (VFlashChannelConfigurator channel in _channelsConfigurators)
            {
                if (channel.ChannelId == chanId)
                    channel.FlashProjectPath = projectPath;
                    _channels[chanId].Path = projectPath;
                return;
            }
            _channelsConfigurators.Add(new VFlashChannelConfigurator(projectPath, chanId, UpdateProgress, UpdateStatus));
            _channels[chanId].Path = projectPath;
            _channels[chanId].Command = "";
            _channels[chanId].Status = "Created";
        }

        public void LoadProject(int channel)
        {
            switch (channel)
            {
                case 1:
                    _channels[1].Command = "Load";
                    break;
                    //todo: 
            }
        }

        public void UnloadProject(int channel)
        {
            switch (channel)
            {
                case 1:
                    _channels[1].Command = "Unload";
                    break;
                    //todo: 
            }
        }

        #endregion

        #region Background methods

        private void VectorBackgroundThread()
        {
            Int16 val = 0;
            while (_vectorThread.IsAlive)
            {
                if (_outputInterface != null)
                {
                    val += 1;
                    _outputInterface.ModifyValue("ANTWORT", val);
                    _outputInterface.ModifyValue("FEHLERCODE", (Int16) (val - 2*val));
                }

                switch (_channels[1].Command)
                {
                    default:
                        _channels[1].Command = "";
                        break;
                    case "Load":
                        _channels[1].Status = "Loading";
                        _channels[1].Result = _vFlashStationController.InitializeAndLoadProjects(_channelsConfigurators);
                        if (_channels[1].Result)
                        {
                            _channels[1].Command = "";
                            _channels[1].Result = false;
                            _channels[1].Status = "Loaded";
                        }
                        break;
                    case "Unload":
                        _channels[1].Status = "Unloading";
                        _channels[1].Result = _vFlashStationController.UnloadProjectsAndDeinitialize(_channelsConfigurators);
                        if (_channels[1].Result)
                        {
                            _channels[1].Command = "";
                            _channels[1].Result = false;
                            _channels[1].Status = "Unloaded";
                        }
                        break;
                }
                Thread.Sleep(10);
            }
        }

        #endregion

        #region Auxiliaries

        public class FlashHandlerException : ApplicationException
        {
            public FlashHandlerException(string info) : base(info) { }
        }

        internal void UpdateProgress(long handle, uint progressInPercent, uint remainingTimeInSecs)
        {
        }

        internal void UpdateStatus(long handle, VFlashStationStatus status)
        {
        }

        internal void ReportError(long handle, string errorMessage)
        {
            ErrorCollector.AddMessage(DateTime.Now + "Handle {0}: {1}", handle, errorMessage);
            _channels[1].Command = "";
            _channels[1].Status = errorMessage;
        }

        #endregion
    }
}
