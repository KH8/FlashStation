using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.DataAquisition
{
    public abstract class CommunicationInterfaceHandlerComponent : UserControl
    {
        protected CommunicationInterfaceHandler CommunicationInterfaceHandler;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {}

        protected CommunicationInterfaceHandlerComponent(CommunicationInterfaceHandler communicationInterfaceHandler)
        {
            CommunicationInterfaceHandler = communicationInterfaceHandler;
            CommunicationInterfaceHandler.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
