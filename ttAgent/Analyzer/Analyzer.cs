using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using _ttAgent.DataAquisition;
using _ttAgent.General;
using _ttAgent.Log;
using _ttAgent.Visual.Gui;

namespace _ttAgent.Analyzer
{
    public class Analyzer : Module
    {
        #region Variables

        private Boolean _pcControlMode;
        private readonly Boolean _pcControlModeChangeAllowed;
        private Boolean _recording;
        private uint _numberOfChannels;

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
        public GuiComponent AnalyzerMainFrame { get; set; }

        public bool Recording
        {
            get { return _recording; }
            set { _recording = value; }
        }

        #endregion

        #region Constructor

        public Analyzer(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, AnalyzerAssignmentFile analyzerAssignmentFile) : base(id, name)
        {
            _pcControlMode = true;
            _pcControlModeChangeAllowed = true;

            _numberOfChannels = 0;

            CommunicationInterfaceHandler = communicationInterfaceHandler;
            AnalyzerAssignmentFile = analyzerAssignmentFile;
            AnalyzerObservableVariablesDictionary = new Dictionary<uint, AnalyzerObservableVariable>();
            AnalyzerMainFrame = new GuiComponent(0, "", new GuiAnalyzerMainFrame());

            _thread = new Thread(AnalyzeThread);
            _thread.Start();
            _thread.IsBackground = true;

            CreateInterfaceAssignment(id, AnalyzerAssignmentFile);
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            Logger.Log("ID: " + Header.Id + " Analyzer Initialized");
        }

        public void StartStopRecording()
        {
            _recording = !_recording;
        }

        public void AddNewChannel()
        {
            _numberOfChannels += 1;

            var analyzerMainFrameGrid = (GuiAnalyzerMainFrame) AnalyzerMainFrame.UserControl;

            var analyzerSingleFigure = new GuiComponent(_numberOfChannels, "", new GuiAnalyzerSingleFigure(_numberOfChannels, this));
            analyzerSingleFigure.Initialize(0, ((int)_numberOfChannels - 1) * 130, analyzerMainFrameGrid.GeneralGrid);

            var userControl = (GuiAnalyzerSingleFigure)analyzerSingleFigure.UserControl;
            userControl.UpdateSizes(analyzerMainFrameGrid.Height, analyzerMainFrameGrid.Width);
        }

        #endregion

        #region Background methods

        private void AnalyzeThread()
        {
            while (_thread.IsAlive)
            {
                if (_recording)
                {
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

        /*private Boolean CheckInterface()
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
        }*/

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
