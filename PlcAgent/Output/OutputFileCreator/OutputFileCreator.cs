using System;
using System.Threading;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.Output.Template;

namespace _PlcAgent.Output.OutputFileCreator
{
    class OutputFileCreator : OutputModule
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

        #endregion


        #region Constructors

        public OutputFileCreator(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, OutputDataTemplate outputDataTemplate, OutputFileCreatorFile outputFileCreatorFile, OutputFileCreatorInterfaceAssignmentFile outputFileCreatorInterfaceAssignmentFile)
            : base(id, name, communicationInterfaceHandler, outputDataTemplate)
        {
            FileCreator = new XmlFileCreator();

            OutputFileCreatorFile = outputFileCreatorFile;
            OutputFileCreatorInterfaceAssignmentFile = outputFileCreatorInterfaceAssignmentFile;

            _communicationThread = new Thread(OutputCommunicationThread);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;

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

        public void CreateOutput(string fileName)
        {
            FileCreator.CreateOutput(fileName, OutputDataTemplate.Composite, FileCreator.OutputConfiguration.Composite);
        }

        #endregion


        #region Background methods

        private void OutputCommunicationThread()
        {
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
            if (Assignment.Length == 0) Assignment = new string[4];
            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();

            InterfaceAssignmentCollection.Add(0, "Command", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.In, Assignment);
            InterfaceAssignmentCollection.Add(1, "Life Counter", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(2, "Reply", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(3, "Status", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
        }

        #endregion

    }
}
