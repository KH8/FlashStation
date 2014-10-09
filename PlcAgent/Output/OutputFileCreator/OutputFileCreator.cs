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

        private readonly Thread _communicationThread;

        #endregion


        #region Properties

        public FileCreator FileCreator { get; set; }

        public OutputHandlerFile OutputHandlerFile { get; set; }
        public OutputHandlerInterfaceAssignmentFile OutputHandlerInterfaceAssignmentFile { get; set; }

        #endregion


        #region Constructors

        public OutputFileCreator(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler)
            : base(id, name, communicationInterfaceHandler)
        {
            FileCreator = new XmlFileCreator();

            _communicationThread = new Thread(OutputCommunicationThread);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;
        }

        #endregion


        #region Methods

        public void CreateOutput(string fileName, DataTemplateComposite outputDataTemplateComposite)
        {
            FileCreator.CreateOutput(fileName, outputDataTemplateComposite, FileCreator.OutputConfiguration.Composite);
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

        public override void UpdateAssignment()
        {
            throw new NotImplementedException();
        }

        protected override bool CheckInterface()
        {
            throw new NotImplementedException();
        }

        protected override void CreateInterfaceAssignment(uint id, string[][] assignment)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
