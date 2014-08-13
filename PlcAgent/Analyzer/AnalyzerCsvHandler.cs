using System;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;
using OxyPlot;
using OxyPlot.Axes;
using _PlcAgent.Log;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerCsvHandler : AnalyzerComponent
    {
        #region Variables

        private readonly string _filePath;

        #endregion


        #region Constructors

        public AnalyzerCsvHandler(Analyzer analyzer) : base(analyzer)
        {
            _filePath = "Analyzer\\Temp_ANALYZER_" + Analyzer.Header.Id + ".csv";
        }

        #endregion


        #region Methods

        public void ExportCsvFile(string fileName)
        {
            try { File.Copy(_filePath, fileName); }
            catch (Exception)
            {
                Logger.Log("ID: " + Analyzer.Header.Id + " Analysis export failed");
                return;
            }
            Logger.Log("ID: " + Analyzer.Header.Id + " Analysis exported to file: " + fileName);
        }

        public void InitCsvFile()
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
                for (var i = 1; i <= Analyzer.AnalyzerChannels.HighestId; i++)
                {
                    var fieldString = "";
                    var id = i;
                    foreach (var analyzerChannel in Analyzer.AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null && analyzerChannel.Id == id))
                    {
                        fieldString = "VARIABLE:" + analyzerChannel.AnalyzerObservableVariable.Name + ":[" +
                                          analyzerChannel.AnalyzerObservableVariable.Unit + "]:AXIS:Y";
                    }

                    writer.WriteField(fieldString != "" ? fieldString : "VARIABLE:EMPTY:AXIS:Y");
                }

                writer.NextRecord();
                streamWriter.Close();
            }
        }

        public void StorePointsInCsvFile()
        {
            using (var streamWriter = File.AppendText(_filePath))
            {
                var writer = new CsvWriter(streamWriter);
                writer.Configuration.Delimiter = ";";

                foreach (var analyzerChannel in Analyzer.AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
                {
                    writer.WriteField(TimeSpan.FromSeconds(analyzerChannel.AnalyzerObservableVariable.ValueX).ToString());
                    break;
                }
                for (var i = 1; i <= Analyzer.AnalyzerChannels.HighestId; i++)
                {
                    var fieldValue = 0.0;
                    var id = i;
                    foreach (var analyzerChannel in Analyzer.AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null && analyzerChannel.Id == id))
                    {
                        fieldValue = analyzerChannel.AnalyzerObservableVariable.ValueY;
                    }
                    writer.WriteField(fieldValue);
                }

                writer.NextRecord();
                streamWriter.Close();
            }
        }

        public void RetrivePointsFromCsvFile()
        {
            RetrivePointsFromCsvFile(_filePath);
        }

        public void RetrivePointsFromCsvFile(string filePath)
        {
            Analyzer.TimeObservableVariable.Clear();
            Analyzer.AnalyzerChannels.Clear();

            var lines = File.ReadAllLines(filePath);
            for (var i = 1; i < lines.Length; i++)
            {
                var variables = lines[i].Split(';');
                var time = TimeSpanAxis.ToDouble(TimeSpan.Parse(variables[0]));
                Analyzer.TimeObservableVariable.MainViewModel.Series.Points.Add(new DataPoint(time,0));
                Analyzer.TimeObservableVariable.MainViewModel.SynchronizeView(time);
                foreach (var analyzerChannel in Analyzer.AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null).Where(analyzerChannel => analyzerChannel.Id < variables.Length))
                {
                    analyzerChannel.AnalyzerObservableVariable.MainViewModel.Series.Points.Add(new DataPoint(time,
                        Convert.ToDouble(variables[analyzerChannel.Id])));
                    analyzerChannel.AnalyzerObservableVariable.MainViewModel.SynchronizeView(time);
                }
            }
        }

        #endregion


        #region Event Handlers

        protected override void OnRecordingChanged()
        {
            if (Analyzer.Recording) return;
            Thread.Sleep(1000);
            RetrivePointsFromCsvFile();
        }

        protected override void OnRecordingTimeChanged()
        {}

        protected override void OnDataCursorsVisibilityChanged()
        {}

        #endregion

    }
}
