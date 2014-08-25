using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.Vector
{
    public abstract class VFlashHandlerComponent : UserControl
    {
        protected VFlashHandler VFlashHandler;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Command":
                    OnCommandChanged();
                    break;
                case "Status":
                    OnStatusChanged();
                    break;
                case "ProjectHandler":
                    OnProjectHandleChanged();
                    break;
                case "FlashProjectPath":
                    OnFlashProjectPathChanged();
                    break;
                case "Result":
                    OnResultChanged();
                    break;
                case "ProgressPercentage":
                    OnProgressPercentageChanged();
                    break;
                case "RemainingTimeInSecs":
                    OnRemainingTimeInSecsChanged();
                    break;
            }
        }

        protected abstract void OnCommandChanged();
        protected abstract void OnStatusChanged();
        protected abstract void OnProjectHandleChanged();
        protected abstract void OnFlashProjectPathChanged();
        protected abstract void OnResultChanged();
        protected abstract void OnProgressPercentageChanged();
        protected abstract void OnRemainingTimeInSecsChanged();

        protected VFlashHandlerComponent(VFlashHandler vFlashHandler)
        {
            VFlashHandler = vFlashHandler;
            var channel = VFlashHandler.ReturnChannelSetup(VFlashHandler.Header.Id);
            channel.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
