using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.Output.OutputHandler
{
    public abstract class OutputHandlerComponent : UserControl
    {
        protected OutputHandler OutputHandler;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
        }

        protected OutputHandlerComponent(OutputHandler outputHandler)
        {
            OutputHandler = outputHandler;
            OutputHandler.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
