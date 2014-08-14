using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.PLC
{
    public abstract class PlcCommunicatorComponent : UserControl
    {
        protected PlcCommunicator PlcCommunicator;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ConnectionStatus":
                    OnConnectionStatusChanged();
                    break;
                case "ConfigurationStatus":
                    OnConfigurationStatusChanged();
                    break;
            }
        }

        protected abstract void OnConnectionStatusChanged();
        protected abstract void OnConfigurationStatusChanged();

        protected PlcCommunicatorComponent(PlcCommunicator plcCommunicator)
        {
            PlcCommunicator = plcCommunicator;
            PlcCommunicator.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
