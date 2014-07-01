using System;
using System.Threading;
using System.Windows;
using _ttAgent.DataAquisition;
using _ttAgent.General;
using _ttAgent.Log;

namespace _ttAgent.Output
{
    public class OutputHandler : Module
    {
        #region Variables

        private Boolean _pcControlMode;
        private Boolean _pcControlModeChangeAllowed;

        private OutputWriter _outputWriter;

        private readonly Thread _outputThread;

        #endregion

        #region Properties

        public Boolean PcControlMode
        {
            get { return _pcControlMode; }
            set { if (_pcControlModeChangeAllowed) { _pcControlMode = value;}}
        }

        public OutputWriter OutputWriter
        {
            get { return _outputWriter; }
            set { _outputWriter = value; }
        }

        public CommunicationInterfaceHandler CommunicationInterfaceHandler { get; set; }
        public OutputHandlerFile OutputHandlerFile { get; set; }
        public OutputHandlerInterfaceAssignmentFile OutputHandlerInterfaceAssignmentFile { get; set; }

        #endregion

        #region Constructor

        public OutputHandler(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, OutputHandlerFile outputHandlerFile, OutputHandlerInterfaceAssignmentFile outputHandlerInterfaceAssignmentFile) : base(id, name)
        {
            CommunicationInterfaceHandler = communicationInterfaceHandler;
            OutputHandlerFile = outputHandlerFile;

            _outputThread = new Thread(OutputCommunicationThread);
            _outputThread.SetApartmentState(ApartmentState.STA);
            _outputThread.IsBackground = true;

            CreateInterfaceAssignment(id, outputHandlerInterfaceAssignmentFile);
        }

        #endregion

        #region Methods

        public void InitializeOutputHandler()
        {
            _outputThread.Start();
            Logger.Log("ID: " + Header.Id + " Output Handler Initialized");
        }

        public void CreateOutput()
        {
            string fileName = OutputHandlerFile.Default.FileNameSuffixes[Header.Id];
            var interfaceVariable = fileName.Split('%');
            if (interfaceVariable.Length > 1)
            {
                var interfaceInputComponent = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(interfaceVariable[1]);
                if (interfaceInputComponent != null)
                {
                    if (interfaceInputComponent.Type == CommunicationInterfaceComponent.VariableType.String)
                    {
                        var ciString = (CiString)interfaceInputComponent;
                        fileName = ciString.Value;
                    }
                }
                var interfaceOutputComponent = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(interfaceVariable[1]);
                if (interfaceOutputComponent != null)
                {
                    if (interfaceOutputComponent.Type == CommunicationInterfaceComponent.VariableType.String)
                    {
                        var ciString = (CiString)interfaceOutputComponent;
                        fileName = ciString.Value;
                    }
                }
            }

            if (_outputWriter != null)
            {
                try{
                    _outputWriter.CreateOutput(fileName,
                        _outputWriter.InterfaceToStrings(CommunicationInterfaceHandler.ReadInterfaceComposite,
                            OutputHandlerFile.Default.StartAddress[Header.Id],
                            OutputHandlerFile.Default.EndAddress[Header.Id]));
                }
                catch (Exception)
                {
                    MessageBox.Show("ID: " + Header.Id + " : Output File creation Failed!", "Error");
                    Logger.Log("ID: " + Header.Id + " : Output File creation Failed");
                }
            }
        }

        #endregion

        #region Background methods

        private void OutputCommunicationThread()
        {
            Int16 counter = 0;
            Int16 caseAuxiliary = 0;

            while (_outputThread.IsAlive)
            {
                _pcControlModeChangeAllowed = false;

                Int16 antwort;
                Int16 status;

                if (!_pcControlMode && CheckInterface())
                {
                    var inputCompositeCommand = (CiInteger)CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Command"));

                    switch (inputCompositeCommand.Value)
                    {
                        case 100:
                            if (caseAuxiliary != 100)
                            {
                                Logger.Log("ID: " + Header.Id + " : Output file creation requested from PLC");
                                CreateOutput();
                            }
                            caseAuxiliary = 100;
                            antwort = 100;
                            status = 100;
                            break;
                        default:
                            caseAuxiliary = 0;
                            antwort = 0;
                            status = 0;
                            break;
                    }
                }
                else
                {
                    antwort = 999;
                    status = 999;
                    _pcControlModeChangeAllowed = true;
                }

                if (CommunicationInterfaceHandler.WriteInterfaceComposite != null && CheckInterface())
                {
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Life Counter"), counter);
                    counter++;
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Reply"), antwort);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Status"), status);
                }
                Thread.Sleep(200);
            }
        }

        #endregion

        #region Auxiliaries

        public class OutputHandlerException : ApplicationException
        {
            public OutputHandlerException(string info) : base(info) { }
        }

        private Boolean CheckInterface()
        {
            CommunicationInterfaceComponent component = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Command"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer) return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Life Counter"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer) return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Reply"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer) return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Status"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer) return false;
            return true;
        }

        public void CreateInterfaceAssignment(uint id,
            OutputHandlerInterfaceAssignmentFile outputHandlerInterfaceAssignmentFile)
        {
            OutputHandlerInterfaceAssignmentFile = outputHandlerInterfaceAssignmentFile;
            if (OutputHandlerInterfaceAssignmentFile.Assignment[id].Length == 0)
            {
                OutputHandlerInterfaceAssignmentFile.Assignment[id] = new string[4];
            }

            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.In,
                Name = "Command",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = OutputHandlerInterfaceAssignmentFile.Assignment[id][0]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Life Counter",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = OutputHandlerInterfaceAssignmentFile.Assignment[id][1]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Reply",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = OutputHandlerInterfaceAssignmentFile.Assignment[id][2]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Status",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = OutputHandlerInterfaceAssignmentFile.Assignment[id][3]
            });
        }

        public override void UpdateAssignment()
        {
            OutputHandlerInterfaceAssignmentFile.Assignment[Header.Id][0] =
                InterfaceAssignmentCollection.GetAssignment("Command");
            OutputHandlerInterfaceAssignmentFile.Assignment[Header.Id][1] =
                InterfaceAssignmentCollection.GetAssignment("Life Counter");
            OutputHandlerInterfaceAssignmentFile.Assignment[Header.Id][2] =
                InterfaceAssignmentCollection.GetAssignment("Reply");
            OutputHandlerInterfaceAssignmentFile.Assignment[Header.Id][3] =
                InterfaceAssignmentCollection.GetAssignment("Status");
            OutputHandlerInterfaceAssignmentFile.Save();
        }

        #endregion
    }
}
