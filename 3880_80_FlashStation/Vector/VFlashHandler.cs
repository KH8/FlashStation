using System;
using System.Linq;
using System.Threading;
using Vector.vFlash.Automation;
using _3880_80_FlashStation.DataAquisition;

namespace _3880_80_FlashStation.Vector
{
    class VFlashHandler
    {
        #region Variables

        private readonly VFlashStationController _vFlashStationController;
        private readonly VFlashErrorCollector _errorCollector;

        private readonly CommunicationInterfaceComposite _inputComposite;
        private readonly CommunicationInterfaceComposite _outputComposite;

        private readonly Thread _vFlashThread;

        #endregion

        #region Properties

        public VFlashErrorCollector ErrorCollector
        {
            get { return _errorCollector; }
        }

        #endregion

        #region Constructor

        public VFlashHandler(CommunicationInterfaceComposite inputComposite, CommunicationInterfaceComposite outputComposite, CallbackProgressDelegate updateProgressDelegate, CallbackStatusDelegate updateStatusDelegate)
        {
            _inputComposite = inputComposite;
            _outputComposite = outputComposite;

            _errorCollector = new VFlashErrorCollector();
            _vFlashStationController = new VFlashStationController(ReportError, 0);
            _vFlashStationController.Add(new VFlashChannel(ReportError, "", 1, updateProgressDelegate, updateStatusDelegate));
            _vFlashStationController.Initialize();

            _vFlashThread = new Thread(VectorBackgroundThread);
            _vFlashThread.SetApartmentState(ApartmentState.STA);
            _vFlashThread.IsBackground = true;
            _vFlashThread.Start();
        }

        #endregion

        #region Methods

        public void LoadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be loaded was not found");
            channelFound.ExecuteCommand("Load"); 
        }

        public void UnloadProject(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be unloaded was not found");
            channelFound.ExecuteCommand("Unload"); 
        }

        public void StartFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be flashed was not found");
            channelFound.ExecuteCommand("Start"); 
        }

        public void AbortFlashing(uint chanId)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be aborted was not found");
            channelFound.ExecuteCommand("Abort"); 
        }

        public void SetProjectPath(uint chanId, string projectPath)
        {
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == chanId);
            if (channelFound == null) throw new FlashHandlerException("Error: Channel to be aborted was not found");
            channelFound.FlashProjectPath = projectPath;
        }

        public VFlashChannel ReturnChannelSetup(uint chanId)
        {
            return (VFlashChannel)_vFlashStationController.ReturnChannelSetup(chanId);
        }

        public void Deinitialize()
        {
            _vFlashStationController.Deinitialize();
        }

        #endregion

        #region Background methods

        private void VectorBackgroundThread()
        {
            Int16 val = 0;
            while (_vFlashThread.IsAlive)
            {
                if (_outputComposite != null)
                {
                    val += 1;
                    _outputComposite.ModifyValue("ANTWORT", val);
                    _outputComposite.ModifyValue("FEHLERCODE", (Int16)(val - 2 * val));
                }
                Thread.Sleep(50);
            }
        }

        #endregion

        #region Auxiliaries

        public class FlashHandlerException : ApplicationException
        {
            public FlashHandlerException(string info) : base(info) { }
        }

        internal void ReportError(uint channelId, long handle, string errorMessage)
        {
            ErrorCollector.AddMessage(DateTime.Now + "Handle {0}: {1}", handle, errorMessage);
            var channelFound = (VFlashChannel)_vFlashStationController.Children.FirstOrDefault(channel => channel.ChannelId == channelId);
            if (channelFound != null)
            {
                channelFound.Command = "";
                channelFound.Status = "Fault occured!";
            }
        }

        #endregion
    }
}
