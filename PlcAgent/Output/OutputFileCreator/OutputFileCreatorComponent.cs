using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.Output.OutputFileCreator
{
    public abstract class OutputFileCreatorComponent : UserControl
    {
        protected OutputFileCreator OutputFileCreator;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
        }

        protected OutputFileCreatorComponent(OutputFileCreator outputFileCreator)
        {
            OutputFileCreator = outputFileCreator;
            OutputFileCreator.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
