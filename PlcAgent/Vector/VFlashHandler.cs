using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.MainRegistry;
using _PlcAgent.Visual.Gui;

namespace _PlcAgent.Vector
{
    public class VFlashHandler : ExecutiveModule
    {
        #region Variables

        private readonly VFlashErrorCollector _vFlashErrorCollector;
        private VFlashTypeBank _vFlashTypeBank;

        private readonly Thread _vFlashThread;
        private VFlashChannel _vFlashChannel;

        #endregion


        #region Properties

        public VFlashErrorCollector ErrorCollector
        {
            get { return _vFlashErrorCollector; }
        }

        public VFlashTypeBank VFlashTypeBank
        {
            get { return _vFlashTypeBank; }
            set { _vFlashTypeBank = value; }
        }

        public VFlashHandlerInterfaceAssignmentFile VFlashHandlerInterfaceAssignmentFile { get; set; }

        public override string Description
        {
            get { return Header.Name + " ; assigned components: " + CommunicationInterfaceHandler.Header.Name + " ; " + VFlashTypeBank.Header.Name; }
        }

        #endregion


        #region Constructors

        public VFlashHandler(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, VFlashTypeBank vFlashTypeBank, VFlashHandlerInterfaceAssignmentFile vFlashHandlerInterfaceAssignmentFile)
            : base(id, name, communicationInterfaceHandler)
        {
            VFlashHandlerInterfaceAssignmentFile = vFlashHandlerInterfaceAssignmentFile;

            _vFlashErrorCollector = new VFlashErrorCollector();
            _vFlashTypeBank = vFlashTypeBank;

            _vFlashThread = new Thread(VFlashPlcCommunicationThread);
            _vFlashThread.SetApartmentState(ApartmentState.STA);
            _vFlashThread.IsBackground = true;

            if (VFlashHandlerInterfaceAssignmentFile.Assignment == null) VFlashHandlerInterfaceAssignmentFile.Assignment = new string[9][];
            Assignment = VFlashHandlerInterfaceAssignmentFile.Assignment[Header.Id];
            CreateInterfaceAssignment();
        }

        #endregion


        #region Methods

        public void InitializeVFlash()
        {
            if (VFlashStationControllerContext.VFlashStationController != null) return;
            VFlashStationControllerContext.VFlashStationController = new VFlashStationController(ReportError, Header.Id);
            try
            {
                VFlashStationControllerContext.VFlashStationController.Initialize();
            }
            catch (Exception)
            {
                MessageBox.Show("ID: " + Header.Id + " VFlash initialization failed", "VFlash Failed");
                throw new FlashHandlerException("VFlash initialization failed");
            }
        }

        public void DeinitializeVFlash()
        {
            if (VFlashStationControllerContext.VFlashStationController == null ||
                !VFlashStationControllerContext.VFlashStationController.IsEmpty) return;
            try
            {
                VFlashStationControllerContext.VFlashStationController.Deinitialize();
            }
            catch (Exception)
            {
                MessageBox.Show("ID: " + Header.Id + " VFlash deinitialization failed", "VFlash Failed");
                throw new FlashHandlerException("VFlash deinitialization failed");
            }
        }

        public override void Initialize()
        {
            InitializeVFlash();

            VFlashStationControllerContext.VFlashStationController.Add(_vFlashChannel = new VFlashChannel(ReportError, "", Header.Id));
            
            _vFlashThread.Start();
            Logger.Log("ID: " + Header.Id + " vFlash Initialized");
        }

        public override void Deinitialize()
        {
            VFlashStationControllerContext.VFlashStationController.Remove(_vFlashChannel);
            
            _vFlashThread.Abort();
            Logger.Log("ID: " + Header.Id + " vFlash Deinitialized");

            DeinitializeVFlash();
        }

        public override void TemplateGuiUpdate(TabControl mainTabControl, TabControl outputTabControl,
            TabControl connectionTabControl, Grid footerGrid)
        {
            var newtabItem = new TabItem { Header = Header.Name };
            outputTabControl.Items.Add(newtabItem);
            outputTabControl.SelectedItem = newtabItem;

            var newScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
            };
            newtabItem.Content = newScrollViewer;

            var newGrid = new Grid();
            newScrollViewer.Content = newGrid;

            var gridVFlashComponent = (GuiComponent)RegistryContext.Registry.GuiVFlashHandlerComponents.ReturnComponent(Header.Id);
            gridVFlashComponent.Initialize(0, 0, newGrid);

            var gridGuiVFlashStatusBar = (GuiComponent)RegistryContext.Registry.GuiVFlashStatusBars.ReturnComponent(Header.Id);
            gridGuiVFlashStatusBar.Initialize(95 * ((int)Header.Id - 1), 18, footerGrid);

            var gridGuiInterfaceAssignment = (GuiComponent)RegistryContext.Registry.GuiVFlashHandlerInterfaceAssignmentComponents.ReturnComponent(Header.Id);
            gridGuiInterfaceAssignment.Initialize(402, 0, newGrid);
        }

        public override void TemplateRegistryComponentUpdateRegistryFile()
        {
            MainRegistryFile.Default.VFlashHandlers[Header.Id] = new uint[9];
            MainRegistryFile.Default.VFlashHandlers[Header.Id][0] = Header.Id;
            MainRegistryFile.Default.VFlashHandlers[Header.Id][1] = 0;
            MainRegistryFile.Default.VFlashHandlers[Header.Id][2] = CommunicationInterfaceHandler.Header.Id;
            MainRegistryFile.Default.VFlashHandlers[Header.Id][3] = VFlashTypeBank.Header.Id;
            MainRegistryFile.Default.VFlashHandlers[Header.Id][4] = 0;
        }

        public override void TemplateRegistryComponentCheckAssignment(RegistryComponent component)
        {
            if (MainRegistryFile.Default.VFlashHandlers[Header.Id][component.ReferencePosition] == component.Header.Id) throw new Exception("The component is still assigned to another one");
        }

        public void LoadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)VFlashStationControllerContext.VFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be loaded was not found");
            channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Load);

            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
            {
                var version = channelFound.FlashProjectPath;
                if (channelFound.FlashingSequence != null) version = channelFound.FlashingSequence.Version;

                CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Version"), version);
            }
        }

        public void UnloadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)VFlashStationControllerContext.VFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be unloaded was not found");
            channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Unload);

            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
            {
                CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Version"), "N/L     ");
            }
        }

        public void StartFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)VFlashStationControllerContext.VFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be flashed was not found");
            channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Start); 
        }

        public void AbortFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)VFlashStationControllerContext.VFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be aborted was not found");
            channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Abort); 
        }

        public void SetProjectPath(uint chanId, string projectPath)
        {
            var channelFound = (VFlashChannel)VFlashStationControllerContext.VFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be set was not found");
            channelFound.FlashProjectPath = projectPath;
            Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + chanId + " : New path assigned : \n" + channelFound.FlashProjectPath);

            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
            {
                CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Version"), "N/L     ");
            }
        }

        public void SetProjectSequence(uint chanId, VFlashTypeBank.VFlashTypeComponent component)
        {
            var channelFound = (VFlashChannel)VFlashStationControllerContext.VFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be set was not found");
            channelFound.FlashingSequence = component;
            Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + chanId + " : New sequence assigned : \n" + component.Version);

            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
            {
                CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Version"), "N/L     ");
            }  
        }

        public VFlashChannel ReturnChannelSetup(uint chanId)
        {
            return (VFlashChannel)VFlashStationControllerContext.VFlashStationController.ReturnChannelSetup(chanId);
        }

        #endregion


        #region Background methods

        private void VFlashPlcCommunicationThread()
        {
            Int16 counter = 0;
            Int16 antwort = 0;
            Int16 caseAuxiliary = 0;
            Int16 numberOfSteps = 0;
            Int16 actualStep = 0;

            var version = "N/L ";

            while (_vFlashThread.IsAlive)
            {
                var channelFound = (VFlashChannel)VFlashStationControllerContext.VFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == Header.Id);
                PcControlModeChangeAllowed = false;

                if (channelFound != null && !PcControlMode && CheckInterface())
                {
                    var inputCompositeCommand = (Int16)CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Command")).Value;
                    var inputCompositeProgrammVersion = (string)CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Program Version")).Value;

                    switch (inputCompositeCommand)
                    {
                        default:
                            numberOfSteps = 0;
                            actualStep = 0;
                            antwort = 0;
                            caseAuxiliary = 0;
                            break;
                        case 100:
                            if (caseAuxiliary != 100)
                            {
                                Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                           " : Version requested from PLC");
                                
                                var versionFound = VFlashTypeBank.Children.FirstOrDefault(child => child.Version == inputCompositeProgrammVersion);

                                if (versionFound != null)
                                {
                                    SetProjectSequence(Header.Id, versionFound);
                                    version = inputCompositeProgrammVersion;
                                    numberOfSteps = (short) channelFound.FlashingSequence.Steps.Count;
                                    antwort = 100;
                                }
                                else
                                {
                                    Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                               " : Version change requested failed");
                                    version = "N/L ";
                                    antwort = 999;
                                }
                            }
                            caseAuxiliary = 100;
                            break;
                        case 200:
                            if (caseAuxiliary != 200)
                            {
                                Logger.Log("ID: " + Header.Id + " VFlash: Channel nr. " + channelFound.ChannelId +
                                           " : Sequence execution requested from PLC");
                                channelFound.ExecuteCommand(VFlashStationComponent.VFlashCommand.Sequence);
                                Thread.Sleep(200);
                            }
                            if (CommunicationInterfaceHandler.WriteInterfaceComposite != null)
                            {
                                numberOfSteps = (short) channelFound.FlashingSequence.Steps.Count;
                                if (channelFound.FlashingStep != null) actualStep = (short)channelFound.FlashingStep.Id;

                                if (channelFound.Status == VFlashStationComponent.VFlashStatus.SequenceDone) antwort = 200;
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
                                    version = "N/L ";
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
                    }
                }
                else
                {
                    antwort = 999;
                    PcControlModeChangeAllowed = true;
                }

                Int16 statusInt = 0;
                Int16 programPercentage = 0;

                if (channelFound != null)
                    switch (channelFound.Status)
                    {
                        case VFlashStationComponent.VFlashStatus.Created:
                            statusInt = 100;
                            PcControlModeChangeAllowed = true;
                            break;
                        case VFlashStationComponent.VFlashStatus.Loading:
                            statusInt = 299;
                            break;
                        case VFlashStationComponent.VFlashStatus.Loaded:
                            statusInt = 200;
                            PcControlModeChangeAllowed = true;
                            break;
                        case VFlashStationComponent.VFlashStatus.Unloading:
                            statusInt = 399;
                            version = "N/L ";
                            break;
                        case VFlashStationComponent.VFlashStatus.Unloaded:
                            statusInt = 300;
                            PcControlModeChangeAllowed = true;
                            break;
                        case VFlashStationComponent.VFlashStatus.Flashing:
                            statusInt = 499;
                            programPercentage = Convert.ToInt16(channelFound.ProgressPercentage);
                            break;
                        case VFlashStationComponent.VFlashStatus.Flashed:
                            statusInt = 400;
                            programPercentage = 100;
                            PcControlModeChangeAllowed = true;
                            break;
                        case VFlashStationComponent.VFlashStatus.Aborting:
                            statusInt = 599;
                            break;
                        case VFlashStationComponent.VFlashStatus.Fault:
                            PcControlModeChangeAllowed = true;
                            statusInt = 999;
                            break;
                        default:
                            statusInt = 0;
                            PcControlModeChangeAllowed = true;
                            break;
                    }
                if (CommunicationInterfaceHandler.WriteInterfaceComposite != null && CheckInterface())
                {
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Life Counter"), counter);
                    counter++;
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Reply"), antwort);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Status"), statusInt);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Version"), version);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Progress Percentage"), programPercentage);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Number Of Steps"), numberOfSteps);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Actual Step"), actualStep);
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
            var channelFound = (VFlashChannel)VFlashStationControllerContext.VFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == channelId);

            if (channelFound == null) return;
            channelFound.Command = VFlashStationComponent.VFlashCommand.NoCommand;
            channelFound.Status = VFlashStationComponent.VFlashStatus.Fault;
        }

        protected override void AssignmentFileUpdate()
        {
            VFlashHandlerInterfaceAssignmentFile.Assignment[Header.Id] = Assignment;
            VFlashHandlerInterfaceAssignmentFile.Save();
        }

        protected override sealed void CreateInterfaceAssignment()
        {
            if (Assignment.Length == 0) Assignment = new string[10];
            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();

            InterfaceAssignmentCollection.Add(0, "Command", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.In, Assignment);
            InterfaceAssignmentCollection.Add(1, "Program Version", CommunicationInterfaceComponent.VariableType.String, InterfaceAssignment.Direction.In, Assignment);
            InterfaceAssignmentCollection.Add(2, "Life Counter", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(3, "Reply", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(4, "Status", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(5, "Version", CommunicationInterfaceComponent.VariableType.String, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(6, "Fault Code", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(7, "Progress Percentage", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(8, "Actual Step", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(9, "Number Of Steps", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
        }

        #endregion

    }
}
