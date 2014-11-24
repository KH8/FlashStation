using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace _PlcAgent.Log
{
    static class Logger
    {
        private static Boolean _loggingActive;
        private static Boolean _readingActive;
        private static StreamWriter _writer;
        private static StreamReader _reader;

        public static string FileName
        {
            get { return "Log/" + DateTime.Now.Date.ToShortDateString().Replace("/","") + "_Log.txt"; }
        }

        public static void Log(string logMessage)
        {
            while (_readingActive)
            {
                Thread.Sleep(50);
            }

            //Delete all log files older than 3 months
            Directory.GetFiles("Log").Select(f => new FileInfo(f)).Where(f => f.LastWriteTime < DateTime.Now.AddMonths(-3)).ToList().ForEach(f => f.Delete());

            //Log new entry
            _loggingActive = true;
            _writer = File.AppendText(FileName);
                _writer.Write("\r\nLog Entry : ");
                _writer.WriteLine("{0} {1} : ", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                _writer.WriteLine(logMessage);
                _writer.Close();
            _loggingActive = false;    
        }

        public static void DumpLog(ListBox listBox)
        {
            while (_loggingActive)
            {
                Thread.Sleep(50);
            }

            _readingActive = true;
                listBox.Items.Clear();
                _reader = File.OpenText(FileName);
                string line;
                while ((line = _reader.ReadLine()) != null) { listBox.Items.Add(line);}
                _reader.Close();
            _readingActive = false;
        }
    }
}
