using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using OxyPlot.Axes;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.Visual.Gui;

namespace _PlcAgent.Analyzer
{
    public class Analyzer : OutputModule
    {
        #region Variables

        private Boolean _recording;

        private double _startRecordingTime;
        private double _recordingTime;

        private readonly Thread _thread;
        private readonly Thread _visualThread;

        private readonly string _filePath;

        #endregion


        #region Properties

        public CommunicationInterfaceHandler CommunicationInterfaceHandler { get; set; }
        public AnalyzerAssignmentFile AnalyzerAssignmentFile { get; set; }
        public AnalyzerSetupFile AnalyzerSetupFile { get; set; }
        public AnalyzerChannelList AnalyzerChannels { get; set; }

        public GuiAnalyzerMainFrame GuiAnalyzerMainFrame { get; set; }
        public GuiAnalyzerDataCursorTable GuiAnalyzerDataCursorTable { get; set; }

        public AnalyzerDataCursorPointCollection AnalyzerDataCursorPointCollection;

        public bool Recording
        {
            get { return _recording; }
            private set
            {
                if (value == _recording) return;
                _recording = value;
                OnPropertyChanged();
            }
        }

        public double RecordingTime
        {
            get { return _recordingTime; }
            private set
            {
                if (Math.Abs(value - _recordingTime) < 1) return;
                _recordingTime = value;
                OnPropertyChanged();
            }
        }

        public AnalyzerObservableTime ObservableTime { get; set; }

        #endregion


        #region Constructors

        public Analyzer(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, AnalyzerAssignmentFile analyzerAssignmentFile, AnalyzerSetupFile analyzerSetupFile) : base(id, name)
        {
            PcControlModeChangeAllowed = true;
            PcControlMode = true;

            _filePath = "Analyzer\\Temp_ANALYZER_" + id + ".csv"; 

            CommunicationInterfaceHandler = communicationInterfaceHandler;
            AnalyzerAssignmentFile = analyzerAssignmentFile;
            AnalyzerSetupFile = analyzerSetupFile;

            ObservableTime = new AnalyzerObservableTime(this);
            
            AnalyzerChannels = new AnalyzerChannelList(0, this);
            AnalyzerChannels.RetriveConfiguration();

            AnalyzerDataCursorPointCollection = new AnalyzerDataCursorPointCollection();

            _thread = new Thread(AnalyzeThread) { IsBackground = true };
            _visualThread = new Thread(VisualThread) { IsBackground = true };

            CreateInterfaceAssignment(id, AnalyzerAssignmentFile.Assignment);
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            InitCsvFile();

            if (AnalyzerSetupFile.SampleTime[Header.Id] < 10) AnalyzerSetupFile.SampleTime[Header.Id] = 100;
            if (AnalyzerSetupFile.TimeRange[Header.Id] < 1000) AnalyzerSetupFile.TimeRange[Header.Id] = 10000;

            _thread.Start();
            _visualThread.Start();

            Logger.Log("ID: " + Header.Id + " Analyzer Initialized");
        }

        public override void Deinitialize()
        {
            Recording = false;
            Thread.Sleep(AnalyzerSetupFile.SampleTime[Header.Id]);
            Logger.Log("ID: " + Header.Id + " Analyzer Deinitialized");
        }

        public void StartStopRecording()
        {
            if (!Recording)
            {
                Clear();
                InitCsvFile();
            }
            Recording = !Recording;
        }

        public void Clear()
        {
            AnalyzerChannels.Clear();
            ObservableTime.Clear();

            RecordingTime = 0.0;
            _startRecordingTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
            
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
        }

        public void RemoveChannel(AnalyzerChannel analyzerChannel)
        {
            AnalyzerChannels.Remove(analyzerChannel);

            AnalyzerSetupFile.NumberOfChannels[Header.Id] -= 1;
            AnalyzerSetupFile.Save();
        }

        public void UpdateDataCursorTable()
        {
            double timePointBlue;
            double timePointRed;
            double timeDifference;

            try { timePointBlue = GetTimePosition(GuiAnalyzerMainFrame.AnalyzerDataCursorBlue.PercentageActualPosition); }
            catch (Exception) { timePointBlue = Double.NaN; }
            try { timePointRed = GetTimePosition(GuiAnalyzerMainFrame.AnalyzerDataCursorRed.PercentageActualPosition); }
            catch (Exception) { timePointRed = Double.NaN; }
            try { timeDifference = GetTimePosition(GuiAnalyzerMainFrame.AnalyzerDataCursorRed.PercentageActualPosition) 
                                                           - GetTimePosition(GuiAnalyzerMainFrame.AnalyzerDataCursorBlue.PercentageActualPosition); }
            catch (Exception) { timeDifference = Double.NaN; }

            if (AnalyzerDataCursorPointCollection == null) return;

            AnalyzerDataCursorPointCollection.Dispatcher.BeginInvoke((new Action(delegate
            {
                AnalyzerDataCursorPointCollection.Children.Clear();
                AnalyzerDataCursorPointCollection.Children.Add(new AnalyzerDataCursorPoint
                {
                    Name = "Time base",
                    BlueValue = FromMilliseconds(timePointBlue),
                    RedValue = FromMilliseconds(timePointRed),
                    Difference = FromMilliseconds(timeDifference)
                });

                foreach (var analyzerChannel in AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
                {
                    timePointBlue = analyzerChannel.AnalyzerObservableVariable.GetValue(GetTimePosition(GuiAnalyzerMainFrame.AnalyzerDataCursorBlue.PercentageActualPosition), AnalyzerSetupFile.SampleTime[Header.Id]);
                    timePointRed = analyzerChannel.AnalyzerObservableVariable.GetValue(GetTimePosition(GuiAnalyzerMainFrame.AnalyzerDataCursorRed.PercentageActualPosition), AnalyzerSetupFile.SampleTime[Header.Id]);
                    timeDifference = timePointRed - timePointBlue;

                    AnalyzerDataCursorPointCollection.Children.Add(new AnalyzerDataCursorPoint
                    {
                        Name = analyzerChannel.AnalyzerObservableVariable.Name,
                        BlueValue = FormatNumber(timePointBlue,15),
                        RedValue = FormatNumber(timePointRed, 15),
                        Difference = FormatNumber(timeDifference, 15)
                    });
                }
            })));
        }

        public double GetTimePosition(double positionPercentage)
        {
            return AnalyzerSetupFile.TimeRange[Header.Id] * positionPercentage + ObservableTime.MainViewModel.HorizontalAxis.Minimum * 1000.0;
        }

        #endregion

        
        #region Background methods

        private void AnalyzeThread()
        {
            var lastMilliseconds = 0.0;

            while (_thread.IsAlive)
            {
                if (Recording)
                {
                    var timeTick = DateTime.Now.TimeOfDay;

                    ObservableTime.StoreActualValue(TimeSpanAxis.ToDouble(timeTick));
                    Parallel.ForEach(AnalyzerChannels.Children,
                        analyzerChannel =>
                    {
                        if (analyzerChannel.AnalyzerObservableVariable == null) return;
                        analyzerChannel.AnalyzerObservableVariable.StoreActualValue(timeTick.TotalMilliseconds);
                    });

                    StorePointsInCsvFile();
                    RecordingTime = lastMilliseconds - _startRecordingTime;
                }

                var timeDifference = (int) (DateTime.Now.TimeOfDay.TotalMilliseconds - lastMilliseconds);
                if (timeDifference > AnalyzerSetupFile.SampleTime[Header.Id]) timeDifference = AnalyzerSetupFile.SampleTime[Header.Id];
                
                Thread.Sleep(AnalyzerSetupFile.SampleTime[Header.Id] - timeDifference);
                lastMilliseconds = DateTime.Now.TimeOfDay.TotalMilliseconds;
            }
        }

        private void VisualThread()
        {
            while (_visualThread.IsAlive)
            {
                if (!Recording)
                {
                    UpdateDataCursorTable();
                }
                else 
                {
                    foreach (var dataCursorPoint in AnalyzerDataCursorPointCollection.Children)
                    {
                        dataCursorPoint.BlueValue = "---";
                        dataCursorPoint.RedValue = "---";
                        dataCursorPoint.Difference = "---";
                    }
                    GuiAnalyzerDataCursorTable.CursorTableDataGrid.Dispatcher.BeginInvoke((new Action(
                        () => GuiAnalyzerDataCursorTable.CursorTableDataGrid.Items.Refresh())));
                }   
                Thread.Sleep(100);
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
                foreach (var analyzerChannel in AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
                {
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

        protected override Boolean CheckInterface()
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

        protected override sealed void CreateInterfaceAssignment(uint id, string[][] assignment)
        {
            if (assignment[id].Length == 0) assignment[id] = new string[4];

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

        private static string FromMilliseconds(double value)
        {
            return Double.IsNaN(value) ? "N/A" : TimeSpan.FromMilliseconds(value).ToString();
        }

        public string FormatNumber(double number, int length)
        {
            var stringRepresentation = number.ToString(CultureInfo.InvariantCulture);

            if (stringRepresentation.Length > length)
                stringRepresentation = stringRepresentation.Substring(0, length);

            if (stringRepresentation.Length == length && stringRepresentation.EndsWith("."))
                stringRepresentation = stringRepresentation.Substring(0, length - 1);

            return stringRepresentation.PadLeft(length);
        }

        #endregion
    }
}
