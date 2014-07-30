using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
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

        private readonly string _filePath = "Analyzer\\Temp.csv";

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
        public AnalyzerChannelList AnalyzerChannels { get; set; }
        public GuiAnalyzerMainFrame GuiAnalyzerMainFrame { get; set; }

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
            AnalyzerChannels = new AnalyzerChannelList(0, this);
            AnalyzerChannels.RetriveConfiguration();

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

        public void Deinitialize()
        {
            _recording = false;
            Thread.Sleep(AnalyzerSetupFile.SampleTime[Header.Id]);
            Logger.Log("ID: " + Header.Id + " Analyzer Deinitialized");
        }

        public void StartStopRecording()
        {
            if (!_recording)
            {
                Clear();
                InitCsvFile();
            }
            _recording = !_recording;
        }

        public void Clear()
        {
            Parallel.ForEach(AnalyzerChannels.Children,
                analyzerChannel =>
                {
                    if (analyzerChannel.AnalyzerObservableVariable == null) return;
                    analyzerChannel.AnalyzerObservableVariable.Clear();
                });
        }

        public void AddNewChannel()
        {
            AnalyzerSetupFile.NumberOfChannels[Header.Id] += 1;

            var id = (uint)AnalyzerSetupFile.NumberOfChannels[Header.Id];
            AnalyzerChannels.Add(new AnalyzerChannel(id, this));
            AnalyzerSetupFile.Save();

            if (GuiAnalyzerMainFrame == null) return;
            GuiAnalyzerMainFrame.RefreshGui();
        }

        public void RemoveChannel(AnalyzerChannel analyzerChannel)
        {
            AnalyzerChannels.Remove(analyzerChannel);

            AnalyzerSetupFile.NumberOfChannels[Header.Id] -= 1;
            AnalyzerSetupFile.Save();

            if (GuiAnalyzerMainFrame == null) return;
            GuiAnalyzerMainFrame.RefreshGui();
        }

        #endregion

        #region Background methods

        private void AnalyzeThread()
        {
            while (_thread.IsAlive)
            {
                if (_recording)
                {
                    var timeTick = DateTime.Now.TimeOfDay.TotalMilliseconds;

                    Parallel.ForEach(AnalyzerChannels.Children,
                        analyzerChannel =>
                        {
                            if (analyzerChannel.AnalyzerObservableVariable == null) return;
                            analyzerChannel.AnalyzerObservableVariable.StoreActualValue(timeTick);
                            analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Minimum = analyzerChannel.AnalyzerObservableVariable.ValueX - (AnalyzerSetupFile.TimeRange[Header.Id] / 2.0);
                            analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Maximum = analyzerChannel.AnalyzerObservableVariable.ValueX + (AnalyzerSetupFile.TimeRange[Header.Id] / 2.0);
                        });
                    StorePointsInCsvFile();
                }
                Thread.Sleep(AnalyzerSetupFile.SampleTime[Header.Id]);
            }
        }

        #endregion

        #region CSV Storage

        private void InitCsvFile()
        {
            File.Create(_filePath).Close();
            using (var streamWriter = File.AppendText(_filePath))
            {
                var writer = new CsvWriter(streamWriter);
                writer.Configuration.Delimiter = ";";

                writer.WriteField("GENERAL:AXIS:X");
                foreach (var analyzerChannel in AnalyzerChannels.Children)
                {
                    if (analyzerChannel.AnalyzerObservableVariable == null) break;
                    writer.WriteField("VARIABLE:" + analyzerChannel.AnalyzerObservableVariable.Name + ":[" + analyzerChannel.AnalyzerObservableVariable.Unit + "]:AXIS:Y");
                }

                writer.NextRecord();
                streamWriter.Close();
            }
        }

        private void StorePointsInCsvFile()
        {
            using (var streamWriter = File.AppendText(_filePath))
            {
                var writer = new CsvWriter(streamWriter);
                writer.Configuration.Delimiter = ";";

                foreach (var analyzerChannel in AnalyzerChannels.Children)
                {
                    if (analyzerChannel.AnalyzerObservableVariable == null) break;
                    writer.WriteField(analyzerChannel.AnalyzerObservableVariable.ValueX); break;
                }
                foreach (var analyzerChannel in AnalyzerChannels.Children)
                {
                    if (analyzerChannel.AnalyzerObservableVariable == null) break;
                    writer.WriteField(analyzerChannel.AnalyzerObservableVariable.ValueY);
                }

                writer.NextRecord();
                streamWriter.Close();
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
