using System;
using System.IO;
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

        public static void Log(string logMessage)
        {
            while (_readingActive)
            {
                Thread.Sleep(5);
            }

            _loggingActive = true;
                _writer = File.AppendText("Log/Log.txt");
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
                Thread.Sleep(5);
            }

            _readingActive = true;
                listBox.Items.Clear();
                _reader = File.OpenText("Log/Log.txt");
                string line;
                while ((line = _reader.ReadLine()) != null) { listBox.Items.Add(line);}
                _reader.Close();
            _readingActive = false;
        }
    }
}
