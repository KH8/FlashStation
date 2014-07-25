using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.Visual.Gui;

namespace _PlcAgent.Analyzer
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
        public AnalyzerSetupFile AnalyzerSetupFile { get; set; }
        public List<AnalyzerChannel> AnalyzerChannels { get; set; } 
        public GuiComponent AnalyzerMainFrame { get; set; }

        public bool Recording
        {
            get { return _recording; }
            set { _recording = value; }
        }

        #endregion

        #region Constructor

        public Analyzer(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, AnalyzerAssignmentFile analyzerAssignmentFile, AnalyzerSetupFile analyzerSetupFile) : base(id, name)
        {
            _pcControlMode = true;
            _pcControlModeChangeAllowed = true;

            CommunicationInterfaceHandler = communicationInterfaceHandler;
            AnalyzerAssignmentFile = analyzerAssignmentFile;
            AnalyzerSetupFile = analyzerSetupFile;
            AnalyzerChannels = new List<AnalyzerChannel>();
            AnalyzerMainFrame = new GuiComponent(0, "", new GuiAnalyzerMainFrame());

            _thread = new Thread(AnalyzeThread) {IsBackground = true};

            CreateInterfaceAssignment(id, AnalyzerAssignmentFile);
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            if (AnalyzerSetupFile.SampleTime[Header.Id] < 10) AnalyzerSetupFile.SampleTime[Header.Id] = 10;
            if (AnalyzerSetupFile.TimeRange[Header.Id] < 1000) AnalyzerSetupFile.TimeRange[Header.Id] = 1000;
            _thread.Start();
            Logger.Log("ID: " + Header.Id + " Analyzer Initialized");
        }

        public void InitrializeChannels()
        {
            for (uint i = 1; i <= AnalyzerSetupFile.NumberOfChannels[Header.Id]; i++)
            {
                AnalyzerChannels.Add(new AnalyzerChannel(i));
                DrawChannel(i);
            }
        }

        public void StartStopRecording()
        {
            _recording = !_recording;
        }

        public void AddNewChannel()
        {
            AnalyzerSetupFile.NumberOfChannels[Header.Id] += 1;
            var id = (uint)AnalyzerSetupFile.NumberOfChannels[Header.Id];
            AnalyzerChannels.Add(new AnalyzerChannel(id));
            DrawChannel(id);
            AnalyzerSetupFile.Save();
        }

        private void DrawChannel(uint id)
        {
            var analyzerMainFrameGrid = (GuiAnalyzerMainFrame)AnalyzerMainFrame.UserControl;

            var analyzerSingleFigure = new GuiComponent(id, "", new GuiAnalyzerSingleFigure(id, this));
            analyzerSingleFigure.Initialize(0, ((int)id - 1) * 130, analyzerMainFrameGrid.GeneralGrid);

            var userControl = (GuiAnalyzerSingleFigure)analyzerSingleFigure.UserControl;
            userControl.UpdateSizes(analyzerMainFrameGrid.Height, analyzerMainFrameGrid.Width);
        }

        public AnalyzerChannel GetChannel(uint id)
        {
            return AnalyzerChannels.FirstOrDefault(analyzerChannel => analyzerChannel.Id == id);
        }

        public void RemoveChannel(AnalyzerChannel analyzerChannel)
        {
            AnalyzerChannels.Remove(analyzerChannel);
            RefreshGui();
            AnalyzerSetupFile.NumberOfChannels[Header.Id] -= 1;
            AnalyzerSetupFile.Save();
        }

        public void RefreshGui()
        {
            var analyzerMainFrameGrid = (GuiAnalyzerMainFrame)AnalyzerMainFrame.UserControl;
            analyzerMainFrameGrid.GeneralGrid.Children.Clear();

            foreach (var analyzerChannel in AnalyzerChannels) { DrawChannel(analyzerChannel.Id); }
        }

        #endregion

        #region Background methods

        private void AnalyzeThread()
        {
            while (_thread.IsAlive)
            {
                if (_recording)
                {
                    Parallel.ForEach(AnalyzerChannels,
                        analyzerChannel =>
                        {
                            if (analyzerChannel.AnalyzerObservableVariable == null) return;
                            analyzerChannel.AnalyzerObservableVariable.StoreActualValue();
                            analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Minimum = analyzerChannel.AnalyzerObservableVariable.ValueX - (AnalyzerSetupFile.TimeRange[Header.Id] / 2.0);
                            analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Maximum = analyzerChannel.AnalyzerObservableVariable.ValueX + (AnalyzerSetupFile.TimeRange[Header.Id] / 2.0);
                        });
                }
                Thread.Sleep(AnalyzerSetupFile.SampleTime[Header.Id]);
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

        #endregion
    }
}
