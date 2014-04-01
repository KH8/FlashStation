using System;
using System.Linq;
using System.Threading;
using Vector.vFlash.Automation;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;

namespace _3880_80_FlashStation.Vector
{
    class VFlashHandler
    {
        #region Variables

        private Boolean _pcControlMode;
        private Boolean _pcControlModeChangeAllowed;

        private readonly VFlashStationController _vFlashStationController;
        private readonly VFlashErrorCollector _vFlashErrorCollector;
        private readonly VFlashTypeBank _vFlashTypeBank;

        private readonly CommunicationInterfaceComposite _inputComposite;
        private readonly CommunicationInterfaceComposite _outputComposite;

        private readonly Thread _vFlashThread;

        #endregion

        #region Properties

        public Boolean PcControlMode
        {
            get { return _pcControlMode; }
            set { if (_pcControlModeChangeAllowed) { _pcControlMode = value;}}
        }

        public VFlashErrorCollector ErrorCollector
        {
            get { return _vFlashErrorCollector; }
        }

        public VFlashTypeBank VFlashTypeBank
        {
            get { return _vFlashTypeBank; }
        }

        #endregion

        #region Constructor

        public VFlashHandler(CommunicationInterfaceComposite inputComposite, CommunicationInterfaceComposite outputComposite, CallbackProgressDelegate updateProgressDelegate, CallbackStatusDelegate updateStatusDelegate)
        {
            _inputComposite = inputComposite;
            _outputComposite = outputComposite;

            _vFlashErrorCollector = new VFlashErrorCollector();
            _vFlashStationController = new VFlashStationController(ReportError, 0);
            _vFlashStationController.Add(new VFlashChannel(ReportError, "", 1, updateProgressDelegate, updateStatusDelegate));
            _vFlashStationController.Initialize();

            _vFlashTypeBank = new VFlashTypeBank();
            _vFlashErrorCollector = new VFlashErrorCollector();

            _vFlashThread = new Thread(VFlashPlcCommunicationThread);
            _vFlashThread.SetApartmentState(ApartmentState.STA);
            _vFlashThread.IsBackground = true;
            _vFlashThread.Start();
        }

        #endregion

        #region Methods

        public void LoadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be loaded was not found");
            channelFound.ExecuteCommand("Load"); 
        }

        public void UnloadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be unloaded was not found");
            channelFound.ExecuteCommand("Unload"); 
        }

        public void StartFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be flashed was not found");
            channelFound.ExecuteCommand("Start"); 
        }

        public void AbortFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be aborted was not found");
            channelFound.ExecuteCommand("Abort"); 
        }

        public void SetProjectPath(uint chanId, string projectPath)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be aborted was not found");
            channelFound.FlashProjectPath = projectPath;
            Logger.Log("VFlash: Channel nr. " + chanId + " : New path assigned : \n" + channelFound.FlashProjectPath);
        }

        public VFlashChannel ReturnChannelSetup(uint chanId)
        {
            return (VFlashChannel)_vFlashStationController.ReturnChannelSetup(chanId);
        }

        public void Deinitialize()
        {
            _vFlashStationController.Deinitialize();
        }

        #endregion

        #region Background methods

        private void VFlashPlcCommunicationThread()
        {
            Int16 counter = 0;
            Int16 antwort = 0;
            Int16 caseAuxiliary = 0; //for commands working on the rising edge only

            while (_vFlashThread.IsAlive)
            {
                var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == 1);
                var inputComposite = (CiInteger)_inputComposite.ReturnVariable("BEFEHL");
                _pcControlModeChangeAllowed = false;

                if (channelFound != null && !_pcControlMode)
                    switch (inputComposite.Value)
                    {
                        case 100:
                            if (caseAuxiliary != 100)
                            {
                                Logger.Log("VFlash: Channel nr. " + channelFound.ChannelId + " : Path change requested from PLC");
                                SetProjectPath(1, VFlashTypeBank.ReturnPath(1));
                            }
                            if (_outputComposite != null) antwort = 100;
                            caseAuxiliary = 100;
                            break;
                        case 200:
                            if (caseAuxiliary != 200)
                            {
                                Logger.Log("VFlash: Channel nr. " + channelFound.ChannelId + " : Project load requested from PLC");
                                channelFound.ExecuteCommand("Load");
                            }
                            if (_outputComposite != null)
                            {
                                if (channelFound.Status == "Loaded") antwort = 200;
                                if (channelFound.Status == "Fault occured!") antwort = 999;   
                            }
                            caseAuxiliary = 200;
                            break;
                        case 300:
                            if (caseAuxiliary != 300)
                            {
                                Logger.Log("VFlash: Channel nr. " + channelFound.ChannelId + " : Project unload requested from PLC");
                                channelFound.ExecuteCommand("Unload");
                            }
                            if (_outputComposite != null)
                            {
                                if (channelFound.Status == "Unloaded") antwort = 300;
                                if (channelFound.Status == "Fault occured!") antwort = 999;
                            }
                            caseAuxiliary = 300;
                            break;
                        case 400:
                            if (caseAuxiliary != 400)
                            {
                                Logger.Log("VFlash: Channel nr. " + channelFound.ChannelId + " : Flashing requested from PLC");
                                channelFound.ExecuteCommand("Start");
                            }
                            if (_outputComposite != null)
                            {
                                if (channelFound.Status == "Flashed") antwort = 400;
                                if (channelFound.Status == "Fault occured!") antwort = 999;
                            }
                            caseAuxiliary = 400;
                            break;
                        case 500:
                            if (caseAuxiliary != 500)
                            {
                                Logger.Log("VFlash: Channel nr. " + channelFound.ChannelId + " : Flashing abort requested from PLC");
                                channelFound.ExecuteCommand("Abort");
                            }
                            if (_outputComposite != null)
                            {
                                if (channelFound.Status == "Loaded") antwort = 500;
                                if (channelFound.Status == "Fault occured!") antwort = 999;
                            }
                            caseAuxiliary = 500;
                            break;
                        default:
                            antwort = 0;
                            break;
                    }

                if (_pcControlMode)
                {
                    antwort = 998;
                    _pcControlModeChangeAllowed = true;
                }
                if (_outputComposite != null) _outputComposite.ModifyValue("ANTWORT", antwort);
                
                    Int16 statusInt = 0;
                if (channelFound != null)
                    switch (channelFound.Status)
                    {
                        case "Created":
                            statusInt = 100;
                            _pcControlModeChangeAllowed = true;
                            break;
                        case "Loading":
                            statusInt = 299;
                            break;
                        case "Loaded":
                            statusInt = 200;
                            _pcControlModeChangeAllowed = true;
                            break;
                        case "Unloading":
                            statusInt = 399;
                            break;
                        case "Unloaded":
                            statusInt = 300;
                            _pcControlModeChangeAllowed = true;
                            break;
                        case "Flashing":
                            statusInt = 499;
                            break;
                        case "Flashed":
                            statusInt = 400;
                            _pcControlModeChangeAllowed = true;
                            break;
                        case "Aborting":
                            statusInt = 599;
                            break;
                        case "Fault occured!":
                            _pcControlModeChangeAllowed = true;
                            statusInt = 999;
                            break;
                        default:
                            statusInt = 0;
                            _pcControlModeChangeAllowed = true;
                            break;
                    }
                if (_outputComposite != null)
                {
                    _outputComposite.ModifyValue("STATUS", statusInt);
                    _outputComposite.ModifyValue("LEBENSZAECHLER", counter);
                    counter++;
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

        internal void ReportError(uint channelId, long handle, string errorMessage)
        {
            ErrorCollector.AddMessage(DateTime.Now + "Handle {0}: {1}", handle, errorMessage);
            Logger.Log("VFlash: Fault on Channel nr. " + channelId + " : " + errorMessage);
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == channelId);
            if (channelFound != null)
            {
                channelFound.Command = "";
                channelFound.Status = "Fault occured!";
            }
        }

        #endregion
    }
}
