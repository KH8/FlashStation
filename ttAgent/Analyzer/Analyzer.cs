using System;
using System.Collections.Generic;
using System.Threading;
using _ttAgent.DataAquisition;
using _ttAgent.General;

namespace _ttAgent.Analyzer
{
    public class Analyzer : Module
    {
        #region Variables

        private Boolean _pcControlMode;
        private readonly Boolean _pcControlModeChangeAllowed;
        private Boolean _recording;

        private readonly Thread _thread;

        #endregion

        #region Properties

        public Boolean PcControlMode
        {
            get { return _pcControlMode; }
            set { if (_pcControlModeChangeAllowed) { _pcControlMode = value;}}
        }

        public CommunicationInterfaceHandler CommunicationInterfaceHandler { get; set; }
        public AnalyzerAssignmentFile AnalyzerAssignmentFile { get; set; }
        public Dictionary<uint,AnalyzerObservableVariable> AnalyzerObservableVariablesDictionary { get; set; }

        #endregion

        #region Constructor

        public Analyzer(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler) : base(id, name)
        {
            _pcControlMode = true;
            _pcControlModeChangeAllowed = true;

            CommunicationInterfaceHandler = communicationInterfaceHandler;
            AnalyzerObservableVariablesDictionary = new Dictionary<uint, AnalyzerObservableVariable>();

            _thread = new Thread(AnalyzeThread);
            _thread.Start();
            _thread.IsBackground = true;

            //StartRecording();
        }

        #endregion

        #region Methods

        public void StartRecording()
        {
            _recording = true;
        }

        public void StopRecording()
        {
            _recording = false;
        }

        #endregion

        #region Background methods

        private void AnalyzeThread()
        {
            while (_thread.IsAlive)
            {
                if (_recording)
                {
                    //CheckInterface();
                    foreach (var analyzerObservableVariable in AnalyzerObservableVariablesDictionary)
                    {
                        analyzerObservableVariable.Value.StoreActualValue();
                    }
                }
                Thread.Sleep(100);
            }
        }

        #endregion

        #region Auxiliaries

        public class AnalyzerException : ApplicationException
        {
            public AnalyzerException(string info) : base(info) { }
        }

        private Boolean CheckInterface()
        {
            CommunicationInterfaceComponent component = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Command"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Life Counter"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Reply"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Status"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;

            return true;
        }

        #endregion

        public void CreateInterfaceAssignment(uint id, AnalyzerAssignmentFile analyzerAssignmentFile)
        {
            AnalyzerAssignmentFile = analyzerAssignmentFile;
            if (AnalyzerAssignmentFile.Assignment[id].Length == 0) AnalyzerAssignmentFile.Assignment[id] = new string[4];

            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.In,
                Name = "Command",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = AnalyzerAssignmentFile.Assignment[id][0]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Life Counter",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = AnalyzerAssignmentFile.Assignment[id][1]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Reply",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = AnalyzerAssignmentFile.Assignment[id][2]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Status",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = AnalyzerAssignmentFile.Assignment[id][3]
            });
        }

        public override void UpdateAssignment()
        {
            AnalyzerAssignmentFile.Assignment[Header.Id][0] =
                InterfaceAssignmentCollection.GetAssignment("Command");
            AnalyzerAssignmentFile.Assignment[Header.Id][1] =
                InterfaceAssignmentCollection.GetAssignment("Life Counter");
            AnalyzerAssignmentFile.Assignment[Header.Id][2] =
                InterfaceAssignmentCollection.GetAssignment("Reply");
            AnalyzerAssignmentFile.Assignment[Header.Id][3] =
                InterfaceAssignmentCollection.GetAssignment("Status");
            AnalyzerAssignmentFile.Save();
        }
    }
}
