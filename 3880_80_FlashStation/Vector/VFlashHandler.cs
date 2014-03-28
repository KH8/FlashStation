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
        
        public VFlashErrorCollector ErrorCollector
        {
            get { return _errorCollector; }
        }

        #endregion

        #region Constructor

        public VFlashHandler()
        {
            _channelsConfigurators = new List<VFlashChannelConfigurator>();

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
                return;
            }
            _channelsConfigurators.Add(new VFlashChannelConfigurator(projectPath, chanId, UpdateProgress, UpdateStatus));
        }

        public VFlashChannelConfigurator ReturnChannelSetup(uint chanId)
        {
            foreach (VFlashChannelConfigurator channel in _channelsConfigurators)
            {
                if (channel.ChannelId == chanId)
                    return channel;
            }
            throw new FlashHandlerException("Error: Channel was not found");
        }

        public void LoadProject(uint chanId)
        {
            foreach (VFlashChannelConfigurator channel in _channelsConfigurators)
            {
                if (channel.ChannelId == chanId)
                    channel.Command = "Load";
                return;
            }
            throw new FlashHandlerException("Error: Channel to be loaded was not found");
        }

        public void UnloadProject(uint chanId)
        {
            foreach (VFlashChannelConfigurator channel in _channelsConfigurators)
            {
                if (channel.ChannelId == chanId)
                    channel.Command = "Unload";
                return;
            }
            throw new FlashHandlerException("Error: Channel to be unloaded was not found");
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

                foreach (VFlashChannelConfigurator channel in _channelsConfigurators)
                {
                    switch (channel.Command)
                    {
                        default:
                            channel.Command = "";
                            break;
                        case "Load":
                            channel.Status = "Loading";
                            channel.Result = _vFlashStationController.InitializeAndLoadProjects(_channelsConfigurators);
                            if (channel.Result)
                            {
                                channel.Command = "";
                                channel.Result = false;
                                channel.Status = "Loaded";
                            }
                            break;
                        case "Unload":
                            channel.Status = "Unloading";
                            channel.Result = _vFlashStationController.UnloadProjectsAndDeinitialize(_channelsConfigurators);
                            if (channel.Result)
                            {
                                channel.Command = "";
                                channel.Result = false;
                                channel.Status = "Unloaded";
                            }
                            break;
                    }
                }
                Thread.Sleep(50);
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

        internal void ReportError(uint channelId, long handle, string errorMessage)
        {
            ErrorCollector.AddMessage(DateTime.Now + "Handle {0}: {1}", handle, errorMessage);
            foreach (VFlashChannelConfigurator channel in _channelsConfigurators)
            {
                if (channel.ChannelId == channelId)
                    channel.Command = "";
                    channel.Status = errorMessage;
                return;
            }
            throw new FlashHandlerException("Error: Channel to be unloaded was not found");
        }

        #endregion
    }
}
