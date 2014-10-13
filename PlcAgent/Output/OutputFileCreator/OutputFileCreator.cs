using System;
using System.Threading;
using System.Windows;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.MainRegistry;
using _PlcAgent.Output.Template;

namespace _PlcAgent.Output.OutputFileCreator
{
    public class OutputFileCreator : OutputModule
    {
        #region Variables

        private FileCreator _fileCreator;
        private readonly Thread _communicationThread;

        #endregion


        #region Properties

        public FileCreator FileCreator
        {
            get { return _fileCreator; }
            set { _fileCreator = value; }
        }

        public OutputFileCreatorFile OutputFileCreatorFile { get; set; }
        public OutputFileCreatorInterfaceAssignmentFile OutputFileCreatorInterfaceAssignmentFile { get; set; }

        public override string Description
        {
            get { return Header.Name + " ; assigned components: " + CommunicationInterfaceHandler.Header.Name + " ; " + OutputDataTemplate.Header.Name; }
        }

        #endregion


        #region Constructors

        public OutputFileCreator(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, OutputDataTemplate outputDataTemplate, OutputFileCreatorFile outputFileCreatorFile, OutputFileCreatorInterfaceAssignmentFile outputFileCreatorInterfaceAssignmentFile)
            : base(id, name, communicationInterfaceHandler, outputDataTemplate)
        {
            OutputFileCreatorFile = outputFileCreatorFile;
            OutputFileCreatorInterfaceAssignmentFile = outputFileCreatorInterfaceAssignmentFile;

            _communicationThread = new Thread(OutputCommunicationThread);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;

            if (OutputFileCreatorInterfaceAssignmentFile.Assignment == null) OutputFileCreatorInterfaceAssignmentFile.Assignment = new string[9][];
            Assignment = OutputFileCreatorInterfaceAssignmentFile.Assignment[Header.Id];
            CreateInterfaceAssignment();
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            _communicationThread.Start();
            Logger.Log("ID: " + Header.Id + " Output File Creator Initialized");
        }

        public override void Deinitialize()
        {
            _communicationThread.Abort();
            Logger.Log("ID: " + Header.Id + " Output File Creator Deinitialized");
        }

        public void CreateOutput()
        {
            var fileName = OutputFileCreatorFile.Default.FileNameSuffixes[Header.Id];
            var directoryName = OutputFileCreatorFile.Default.DirectoryPaths[Header.Id];

            var interfaceVariable = fileName.Split('%');
            if (interfaceVariable.Length > 1)
            {
                var interfaceInputComponent = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(interfaceVariable[1]);
                if (interfaceInputComponent != null)
                {
                    if (interfaceInputComponent.TypeOfVariable == CommunicationInterfaceComponent.VariableType.String)
                    {
                        fileName = (string)interfaceInputComponent.Value;
                    }
                }
                var interfaceOutputComponent = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(interfaceVariable[1]);
                if (interfaceOutputComponent != null)
                {
                    if (interfaceOutputComponent.TypeOfVariable == CommunicationInterfaceComponent.VariableType.String)
                    {
                        fileName = (string)interfaceOutputComponent.Value;
                    }
                }
            }

            if (_fileCreator == null) return;
            try
            {
                _fileCreator.CreateOutput(FileNameCreator(fileName, directoryName), OutputDataTemplate.Composite, FileCreator.OutputConfiguration.Composite);
            }
            catch (Exception)
            {
                MessageBox.Show("ID: " + Header.Id + " : Output File creation Failed!", "Error");
                Logger.Log("ID: " + Header.Id + " : Output File creation Failed");
            }
        }

        #endregion


        #region Background methods

        private void OutputCommunicationThread()
        {
            Int16 counter = 0;
            Int16 caseAuxiliary = 0;

            while (_communicationThread.IsAlive)
            {
                PcControlModeChangeAllowed = false;

                Int16 antwort;
                Int16 status;

                if (!PcControlMode && CheckInterface())
                {
                    var inputCompositeCommand = (Int16)CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Command")).Value;

                    switch (inputCompositeCommand)
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
                    PcControlModeChangeAllowed = true;
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

        public class OutputFileCreatorException : ApplicationException
        {
            public OutputFileCreatorException(string info) : base(info) { }
        }

        protected override void AssignmentFileUpdate()
        {
            OutputFileCreatorInterfaceAssignmentFile.Assignment[Header.Id] = Assignment;
            OutputFileCreatorInterfaceAssignmentFile.Save();
        }

        protected override sealed void CreateInterfaceAssignment()
        {
            if (Assignment == null || Assignment.Length == 0) Assignment = new string[4];
            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();

            InterfaceAssignmentCollection.Add(0, "Command", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.In, Assignment);
            InterfaceAssignmentCollection.Add(1, "Life Counter", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(2, "Reply", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(3, "Status", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
        }

        #endregion

    }
}
