using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.Analyzer
{
    public abstract class AnalyzerGuiComponent : UserControl
    {
        protected Analyzer Analyzer;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {   
                case "Recording":
                    OnRecordingChanged();
                    break;
                case "RecordingTime":
                    OnRecordingTimeChanged();
                    break;
            }
        }

        protected abstract void OnRecordingChanged();
        protected abstract void OnRecordingTimeChanged();

        protected AnalyzerGuiComponent(Analyzer analyzer)
        {
            Analyzer = analyzer;
            Analyzer.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
