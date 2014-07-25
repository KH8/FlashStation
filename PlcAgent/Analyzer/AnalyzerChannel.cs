using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace _PlcAgent.Analyzer
{
    public class AnalyzerChannel
    {
        private uint _id;
        private string _unit;
        private Brush _brush;
        private AnalyzerObservableVariable _analyzerObservableVariable;

        public AnalyzerChannel(uint id)
        {
            _id = id;
            _unit = "1";
            Brush = Brushes.Green;
        }

        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        public Brush Brush
        {
            get { return _brush; }
            set { _brush = value; }
        }

        public AnalyzerObservableVariable AnalyzerObservableVariable
        {
            get { return _analyzerObservableVariable; }
            set { _analyzerObservableVariable = value; }
        }
    }
}