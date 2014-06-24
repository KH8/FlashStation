using System;
using System.Linq;
using System.Threading;
using System.Windows;
using _ttAgent.DataAquisition;
using _ttAgent.Log;
using _ttAgent.MainRegistry;

namespace _ttAgent.Vector
{
    class VFlashHandler : RegistryComponent
    {
        #region Variables

        private Boolean _pcControlMode;
        private Boolean _pcControlModeChangeAllowed;

        private readonly VFlashStationController _vFlashStationController;
        private readonly VFlashErrorCollector _vFlashErrorCollector;
        private VFlashTypeBank _vFlashTypeBank;

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
            set { _vFlashTypeBank = value; }
        }

        public CommunicationInterfaceHandler CommunicationInterfaceHandler { get; set; }

        #endregion

        #region Constructor

        public VFlashHandler(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler) : base(id, name)
        {
            CommunicationInterfaceHandler = communicationInterfaceHandler;

            _vFlashErrorCollector = new VFlashErrorCollector();
            _vFlashStationController = new VFlashStationController(ReportError, Header.Id);
            _vFlashStationController.Add(new VFlashChannel(ReportError, "", Header.Id));

            _vFlashTypeBank = new VFlashTypeBank(id, "VFLASH_BANK__" + id, VFlashTypeBankFile.Default);
            _vFlashErrorCollector = new VFlashErrorCollector();

            _vFlashThread = new Thread(VFlashPlcCommunicationThread);
            _vFlashThread.SetApartmentState(ApartmentState.STA);
            _vFlashThread.IsBackground = true;
        }

        #endregion

        #region Methods

        public void InitializeVFlash()
        {
            try
            {
                if (!CheckInterface()) throw new FlashHandlerException("Interface check failed");
                _vFlashStationController.Initialize();
            }
            catch (Exception)
            {
                MessageBox.Show("ID: " + Header.Id + " VFlash initialization failed", "VFlash Failed");
                throw new FlashHandlerException("VFlash initialization failed");
            }

            _vFlashThread.Start();
            Logger.Log("ID: " + Header.Id + " vFlash Initialized");
        }

        public void LoadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be loaded was not found");
            channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Load); 
        }

        public void UnloadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be unloaded was not found");
            channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Unload);

            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
                CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue("PROGRAMMTYPAKTIV", (Int16)0);
        }

        public void StartFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be flashed was not found");
            channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Start); 
        }

        public void AbortFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be aborted was not found");
            channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Abort); 
        }

        public void SetProjectPath(uint chanId, string projectPath)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be aborted was not found");
            channelFound.FlashProjectPath = projectPath;
            Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + chanId + " : New path assigned : \n" + channelFound.FlashProjectPath);

            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
                CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue("PROGRAMMTYPAKTIV", (Int16)0);
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
            Int16 caseAuxiliary = 0;
            Int16 programActive = 0;

            var version = "N/L";

            while (_vFlashThread.IsAlive)
            {
                var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == 1);
                _pcControlModeChangeAllowed = false;

                if (channelFound != null && !_pcControlMode && CheckInterface())
                {
                    var inputCompositeCommand = (CiInteger)CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable("BEFEHL");
                    var inputCompositeProgrammTyp = (CiInteger)CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable("PROGRAMMTYP");

                    switch (inputCompositeCommand.Value)
                    {
                        case 100:
                            if (caseAuxiliary != 100)
                            {
                                Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                           " : Path change requested from PLC");
                                var returnedPath =
                                    _vFlashTypeBank.ReturnPath(Convert.ToUInt16(inputCompositeProgrammTyp.Value));
                                if (returnedPath != null)
                                {
                                    SetProjectPath(1,
                                        _vFlashTypeBank.ReturnPath(Convert.ToUInt16(inputCompositeProgrammTyp.Value)));
                                    programActive = inputCompositeProgrammTyp.Value;
                                    antwort = 100;
                                }
                                else
                                {
                                    Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                               " : Path change requested failed");
                                    programActive = 0;
                                    antwort = 999;
                                }
                            }
                            caseAuxiliary = 100;
                            break;
                        case 200:
                            if (caseAuxiliary != 200)
                            {
                                Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                           " : Path load requested from PLC");
                                channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Load);
                                Thread.Sleep(200);
                            }
                            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
                            {
                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.Loaded) antwort = 200;
                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.Fault) antwort = 999;
                            }
                            caseAuxiliary = 200;
                            break;
                        case 300:
                            if (caseAuxiliary != 300)
                            {
                                Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                           " : Path unload requested from PLC");
                                channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Unload);
                                Thread.Sleep(200);
                            }
                            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
                            {
                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.Unloaded)
                                {
                                    programActive = 0;
                                    antwort = 300;
                                }
                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.Fault) antwort = 999;
                            }
                            caseAuxiliary = 300;
                            break;
                        case 400:
                            if (caseAuxiliary != 400)
                            {
                                Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                           " : Flashing requested from PLC");
                                channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Start);
                                Thread.Sleep(200);
                            }
                            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
                            {
                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.Flashed) antwort = 400;
                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.Fault) antwort = 999;
                            }
                            caseAuxiliary = 400;
                            break;
                        case 500:
                            if (caseAuxiliary != 500)
                            {
                                Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                           " : Flashing abort requested from PLC");
                                channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Abort);
                                Thread.Sleep(200);
                            }
                            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
                            {
                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.Aborted) antwort = 500;
                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.Fault) antwort = 999;
                            }
                            caseAuxiliary = 500;
                            break;
                        default:
                            antwort = 0;
                            caseAuxiliary = 0;
                            break;
                    }
                }
                else
                {
                    antwort = 999;
                    _pcControlModeChangeAllowed = true;
                }

                Int16 statusInt = 0;

                if (channelFound != null)
                    switch (channelFound.Status)
                    {
                        case VFlashStationComponent.VFlashStatus.Created:
                            statusInt = 100;
                            _pcControlModeChangeAllowed = true;
                            version = "N/L";
                            break;
                        case VFlashStationComponent.VFlashStatus.Loading:
                            statusInt = 299;
                            break;
                        case VFlashStationComponent.VFlashStatus.Loaded:
                            statusInt = 200;
                            version = VFlashTypeBank.ReturnVersion((uint)programActive) ?? "N/L";
                            _pcControlModeChangeAllowed = true;
                            break;
                        case VFlashStationComponent.VFlashStatus.Unloading:
                            statusInt = 399;
                            break;
                        case VFlashStationComponent.VFlashStatus.Unloaded:
                            statusInt = 300;
                            version = "N/L";
                            _pcControlModeChangeAllowed = true;
                            break;
                        case VFlashStationComponent.VFlashStatus.Flashing:
                            statusInt = 499;
                            break;
                        case VFlashStationComponent.VFlashStatus.Flashed:
                            statusInt = 400;
                            _pcControlModeChangeAllowed = true;
                            break;
                        case VFlashStationComponent.VFlashStatus.Aborting:
                            statusInt = 599;
                            break;
                        case VFlashStationComponent.VFlashStatus.Fault:
                            _pcControlModeChangeAllowed = true;
                            statusInt = 999;
                            break;
                        default:
                            statusInt = 0;
                            _pcControlModeChangeAllowed = true;
                            break;
                    }
                if (CommunicationInterfaceHandler.WriteInterfaceComposite != null && CheckInterface())
                {
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue("LEBENSZAECHLER", counter);
                    counter++;
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue("ANTWORT", antwort);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue("STATUS", statusInt);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue("PROGRAMMTYPAKTIV", programActive);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue("VERSION", version);
                }
                Thread.Sleep(200);
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
            Logger.Log("ID: " + Header.Id + " VFlash: Fault on Channel nr. " + channelId + " : " + errorMessage);
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == channelId);
            if (channelFound != null)
            {
                channelFound.Command = VFlashStationComponent.VFlashCommand.NoCommand;
                channelFound.Status = VFlashStationComponent.VFlashStatus.Fault;
            }
        }

        private Boolean CheckInterface()
        {
            CommunicationInterfaceComponent component = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable("BEFEHL");
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable("PROGRAMMTYP");
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable("LEBENSZAECHLER");
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable("ANTWORT");
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable("STATUS");
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable("PROGRAMMTYPAKTIV");
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable("VERSION");
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.String)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable("FEHLERCODE");
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;

            return true;
        }

        #endregion
    }
}
