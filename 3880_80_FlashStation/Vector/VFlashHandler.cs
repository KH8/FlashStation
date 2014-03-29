using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vector.vFlash.Automation;
using _3880_80_FlashStation.DataAquisition;

namespace _3880_80_FlashStation.Vector
{
    class VFlashHandler
    {
        #region Variables

        private readonly VFlashStationController _vFlashStationController;
        private readonly VFlashErrorCollector _errorCollector;

        private readonly CommunicationInterfaceComposite _inputComposite;
        private readonly CommunicationInterfaceComposite _outputComposite;

        private readonly Thread _vectorThread;

        #endregion

        #region Properties

        public VFlashErrorCollector ErrorCollector
        {
            get { return _errorCollector; }
        }

        #endregion

        #region Constructor

        public VFlashHandler(CommunicationInterfaceComposite inputComposite, CommunicationInterfaceComposite outputComposite)
        {
            _inputComposite = inputComposite;
            _outputComposite = outputComposite;

            _errorCollector = new VFlashErrorCollector();
            _vFlashStationController = new VFlashStationController(ReportError, 0);
            _vFlashStationController.Initialize();

            _vectorThread = new Thread(VectorBackgroundThread);
            _vectorThread.SetApartmentState(ApartmentState.STA);
            _vectorThread.IsBackground = true;
            _vectorThread.Start();
        }

        #endregion

        #region Methods

        public void LoadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be loaded was not found");
            if (channelFound.Command == "")
            { channelFound.Command = "Load"; }
            else ReportError(chanId, channelFound.ProjectHandle, "Error: An attempt to call two commands at the same time");   
            //todo: other conditions
        }

        public void UnloadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be unloaded was not found");
            if (channelFound.Command == "")
            { channelFound.Command = "Unload"; }
            else ReportError(chanId, channelFound.ProjectHandle, "Error: An attempt to call two commands at the same time");
            //todo: other conditions
        }

        public void StartFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be flashed was not found");
            if (channelFound.Command == "")
            { channelFound.Command = "Start"; }
            else ReportError(chanId, channelFound.ProjectHandle, "Error: An attempt to call two commands at the same time");
            //todo: other conditions
        }

        public void AbortFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be aborted was not found");
            if (channelFound.Command == "")
            { channelFound.Command = "Stop"; }
            else ReportError(chanId, channelFound.ProjectHandle, "Error: An attempt to call two commands at the same time");
            //todo: other conditions
        }

        public void Deinitialize()
        {
            _vFlashStationController.Deinitialize();
        }

        #endregion

        #region Background methods

        private void VectorBackgroundThread()
        {
            Int16 val = 0;
            while (_vectorThread.IsAlive)
            {
                if (_outputComposite != null)
                {
                    val += 1;
                    _outputComposite.ModifyValue("ANTWORT", val);
                    _outputComposite.ModifyValue("FEHLERCODE", (Int16)(val - 2 * val));
                }

                foreach (VFlashChannel channel in _channelsConfigurators)
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
                        case "Start":
                            channel.Status = "Flashing";
                            channel.Result = _vFlashStationController.StartFlashing(_channelsConfigurators);
                            if (channel.Result)
                            {
                                channel.Command = "";
                                channel.Result = false;
                                channel.Status = "Loaded";
                            }
                            break;
                        case "Stop":
                            channel.Status = "Aborting";
                            channel.Result = _vFlashStationController.AbortFlashing(_channelsConfigurators);
                            if (channel.Result)
                            {
                                channel.Command = "";
                                channel.Result = false;
                                channel.Status = "Loaded";
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
            foreach (VFlashChannel channel in _channelsConfigurators)
            {
                if (channel.ChannelId == channelId)
                    channel.Command = "";
                    channel.Status = errorMessage;
                return;
            }
        }

        #endregion
    }
}
