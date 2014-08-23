using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.Vector
{
    public abstract class VFlashHandlerComponent : UserControl
    {
        protected VFlashHandler VFlashHandler;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
        }

        protected VFlashHandlerComponent(VFlashHandler vFlashHandler)
        {
            VFlashHandler = vFlashHandler;
            VFlashHandler.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
