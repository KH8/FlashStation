using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using CsvHelper;
using OxyPlot;
using OxyPlot.Axes;
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

        private double _startRecordingTime;
        private double _recordingTime;
        private MainViewModel _timeAxisViewModel;
        private readonly TimeSpanAxis _timeAxis;

        private readonly Thread _thread;

        private readonly string _filePath;

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
        }

        public double RecordingTime
        {
            get { return _recordingTime; }
        }

        public MainViewModel TimeAxisViewModel
        {
            get { return _timeAxisViewModel; }
            set { _timeAxisViewModel = value; }
        }

        #endregion

        #region Constructor

        public Analyzer(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, AnalyzerAssignmentFile analyzerAssignmentFile, AnalyzerSetupFile analyzerSetupFile) : base(id, name)
        {
            _pcControlMode = true;
            _pcControlModeChangeAllowed = true;

            _filePath = "Analyzer\\Temp_ANALYZER_" + id + ".csv"; 

            CommunicationInterfaceHandler = communicationInterfaceHandler;
            AnalyzerAssignmentFile = analyzerAssignmentFile;
            AnalyzerSetupFile = analyzerSetupFile;
            AnalyzerChannels = new AnalyzerChannelList(0, this);
            AnalyzerChannels.RetriveConfiguration();

            _timeAxisViewModel = new MainViewModel();
            _timeAxisViewModel.Model.Axes.Clear();
            _timeAxisViewModel.Model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false
            });
            _timeAxisViewModel.Model.Axes.Add(_timeAxis = new TimeSpanAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                StringFormat = "hh:mm:ss",
                MajorStep = 1,
                MinorStep = 0.1
            });
            _timeAxisViewModel.Brush = Brushes.Black;

            _thread = new Thread(AnalyzeThread) {IsBackground = true};

            CreateInterfaceAssignment(id, AnalyzerAssignmentFile);
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            InitCsvFile();

            if (AnalyzerSetupFile.SampleTime[Header.Id] < 10) AnalyzerSetupFile.SampleTime[Header.Id] = 100;
            if (AnalyzerSetupFile.TimeRange[Header.Id] < 1000) AnalyzerSetupFile.TimeRange[Header.Id] = 10000;

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

            _timeAxisViewModel.Clear();

            _startRecordingTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
            _recordingTime = 0.0;

            Logger.Log("ID: " + Header.Id + " Analysis cleared");
        }

        public void AddNewChannel()
        {
            AnalyzerSetupFile.NumberOfChannels[Header.Id] += 1;

            uint id = 0;

            for (uint i = 0; i < AnalyzerSetupFile.NumberOfChannels[Header.Id]; i++)
            {
                if (AnalyzerChannels.GetChannel(i) != null) continue;
                id = i;
                break;
            }

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

        public void SynchronizeView()
        {
            var timeDiff = (_timeAxis.ActualMaximum - _timeAxis.ActualMinimum) / 2.0;
            var timeTick = _timeAxis.ActualMinimum + timeDiff;

            _timeAxis.Reset();
            _timeAxis.Minimum = timeTick - (AnalyzerSetupFile.TimeRange[Header.Id] / 2000.0);
            _timeAxis.Maximum = timeTick + (AnalyzerSetupFile.TimeRange[Header.Id] / 2000.0);

            _timeAxisViewModel.Model.InvalidatePlot(true);

            Parallel.ForEach(AnalyzerChannels.Children,
                analyzerChannel =>
                {
                    if (analyzerChannel.AnalyzerObservableVariable == null) return;
                    analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Minimum = _timeAxis.ActualMinimum * 1000.0;
                    analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Maximum = _timeAxis.ActualMaximum * 1000.0;
                    analyzerChannel.AnalyzerObservableVariable.MainViewModel.Model.InvalidatePlot(true);
                });
        }

        #endregion

        public double GetTimePosition(double positionPercentage)
        {
            return AnalyzerSetupFile.TimeRange[Header.Id]*positionPercentage + _timeAxis.Minimum*1000.0;
        }

        #region Background methods

        private void AnalyzeThread()
        {
            var lastMilliseconds = 0.0;

            while (_thread.IsAlive)
            {
                if (_recording)
                {
                    var timeTick = DateTime.Now.TimeOfDay;

                    Parallel.ForEach(AnalyzerChannels.Children,
                        analyzerChannel =>
                        {
                            if (analyzerChannel.AnalyzerObservableVariable == null) return;
                            analyzerChannel.AnalyzerObservableVariable.StoreActualValue(timeTick.TotalMilliseconds);

                            analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Reset();
                            analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Minimum =
                                analyzerChannel.AnalyzerObservableVariable.ValueX -
                                (AnalyzerSetupFile.TimeRange[Header.Id]/2.0);
                            analyzerChannel.AnalyzerObservableVariable.MainViewModel.HorizontalAxis.Maximum =
                                analyzerChannel.AnalyzerObservableVariable.ValueX +
                                (AnalyzerSetupFile.TimeRange[Header.Id]/2.0);
                        });
                    StorePointsInCsvFile();

                    var timePoint = new DataPoint(TimeSpanAxis.ToDouble(timeTick), 0);
                    _timeAxisViewModel.AddPoint(timePoint);

                    _timeAxis.Reset();

                    _timeAxis.Minimum = timeTick.TotalSeconds - (AnalyzerSetupFile.TimeRange[Header.Id] / 2000.0);
                    _timeAxis.Maximum = timeTick.TotalSeconds + (AnalyzerSetupFile.TimeRange[Header.Id] / 2000.0);

                    _recordingTime = lastMilliseconds - _startRecordingTime;
                }
                else
                {
                    SynchronizeView();
                }

                var timeDifference = (int) (DateTime.Now.TimeOfDay.TotalMilliseconds - lastMilliseconds);
                if (timeDifference > AnalyzerSetupFile.SampleTime[Header.Id]) timeDifference = AnalyzerSetupFile.SampleTime[Header.Id];
                
                Thread.Sleep(AnalyzerSetupFile.SampleTime[Header.Id] - timeDifference);
                lastMilliseconds = DateTime.Now.TimeOfDay.TotalMilliseconds;
            }
        }

        #endregion

        #region CSV Storage

        public void ExportCsvFile(string fileName)
        {
            try { File.Copy(_filePath, fileName); }
            catch (Exception)
            {
                Logger.Log("ID: " + Header.Id + " Analysis export failed");
                return;
            }
            Logger.Log("ID: " + Header.Id + " Analysis exported to file: " + fileName);
        }

        private void InitCsvFile()
        {
            const string subPath = "Analyzer";
            var isExists = Directory.Exists(subPath);
            if (!isExists) Directory.CreateDirectory(subPath);

            File.Create(_filePath).Close(); 
            using (var streamWriter = File.AppendText(_filePath))
            {
                var writer = new CsvWriter(streamWriter);
                writer.Configuration.Delimiter = ";";

                writer.WriteField("GENERAL:AXIS:X");
                foreach (var analyzerChannel in AnalyzerChannels.Children)
                {
                    if (analyzerChannel.AnalyzerObservableVariable != null) writer.WriteField("VARIABLE:" + analyzerChannel.AnalyzerObservableVariable.Name + ":[" + analyzerChannel.AnalyzerObservableVariable.Unit + "]:AXIS:Y");
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

                foreach (var analyzerChannel in AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
                {
                    writer.WriteField(TimeSpan.FromMilliseconds(analyzerChannel.AnalyzerObservableVariable.ValueX).ToString());
                    break;
                }
                foreach (var analyzerChannel in AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
                {
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
