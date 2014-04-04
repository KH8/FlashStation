using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vector.vFlash.Automation;
using _3880_80_FlashStation.Log;

namespace _3880_80_FlashStation.Vector
{
    #region Component

    public abstract class VFlashStationComponent
    {
        private readonly uint _channelId;
        private string _command;
        private string _status;

        public delegate void ReportErrorDelegate(uint channelId, long handle, string message);

        protected VFlashStationComponent(uint id)
        {
            _channelId = id;
        }

        public uint ChannelId
        {
            get { return _channelId; }
        }

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public abstract void Add(VFlashStationComponent c);
        public abstract void Remove(VFlashStationComponent c);
        protected abstract bool LoadProject();
        protected abstract bool UnloadProject();
        public abstract bool StartFlashing();
        public abstract bool AbortFlashing();
    }

    #endregion

    #region Controller (Composite) 

    internal class VFlashStationController : VFlashStationComponent
    {
        private readonly List<VFlashStationComponent> _children = new List<VFlashStationComponent>();
        private readonly ReportErrorDelegate _reportErrorDelegate;
        
        public List<VFlashStationComponent> Children
        {
            get { return _children; }
        }

        //--- vFlash Station Constructor ---
        internal VFlashStationController(ReportErrorDelegate reportErrorDelegate, uint id)
            : base(id)
        {
            _reportErrorDelegate = reportErrorDelegate;
        }

        internal bool Initialize()
        {
            //--- Initialize vFlash Station ---
            var resInit = VFlashStationAPI.Initialize();
            Logger.Log("VFlash: Initialization");
            if (resInit != VFlashStationResult.Success)
            {
                string errMsg = VFlashStationAPI.GetLastErrorMessage(-1);
                _reportErrorDelegate(0, -1, String.Format("Initialization of vFlash Station failed ({0}).", errMsg));
                return false;
            }
            return true;
        }

        internal bool Deinitialize()
        {
            //--- Deinitialize vFlash Station ---
            var resDeinit = VFlashStationAPI.Deinitialize();
            Logger.Log("VFlash: Deinitialization");
            if (resDeinit != VFlashStationResult.Success)
            {
                string errMsg = VFlashStationAPI.GetLastErrorMessage(-1);
                _reportErrorDelegate(0, -1, String.Format("Deinitialization of vFlash Station during rewinding failed ({0}).", errMsg));
            }
            return true;
        }

        public override void Add(VFlashStationComponent c)
        {
            _children.Add(c);
        }

        public override void Remove(VFlashStationComponent c)
        {
            _children.Remove(c);
        }

        protected override bool LoadProject()
        {
            foreach (var vFlashStationComponent in _children)
            {
                var channel = (VFlashChannel) vFlashStationComponent;
                channel.ExecuteCommand("Load");
            }
            return true;
        }

        protected override bool UnloadProject()
        {
            foreach (var vFlashStationComponent in _children)
            {
                var channel = (VFlashChannel) vFlashStationComponent;
                channel.ExecuteCommand("Unload");
            }
            return true;
        }

        public override bool StartFlashing()
        {
            foreach (var vFlashStationComponent in _children)
            {
                var channel = (VFlashChannel) vFlashStationComponent;
                channel.ExecuteCommand("Start");
            }
            return true;
        }

        public override bool AbortFlashing()
        {
            foreach (var vFlashStationComponent in _children)
            {
                var channel = (VFlashChannel) vFlashStationComponent;
                channel.ExecuteCommand("Abort");
            }
            return true;
        }

        public VFlashStationComponent ReturnChannelSetup(uint chanId)
        {
            return _children.FirstOrDefault(channel => channel.ChannelId == chanId);
        }
    }

    #endregion

    #region Channel

    internal class VFlashChannel : VFlashStationComponent
    {
        private long _projectHandle;
        private string _flashProjectPath;
        private Boolean _result;

        private uint _progressPercentage;
        private uint _remainingTimeInSecs;

        private readonly ReportErrorDelegate _reportErrorDelegate;

        private readonly Thread _vFlashThread;

        public VFlashChannel(ReportErrorDelegate reportErrorDelegate, string flashProjectPath, uint channelId)
            : base(channelId)
        {
            Command = "";
            Status = "Created";

            _reportErrorDelegate = reportErrorDelegate;
            _flashProjectPath = flashProjectPath;
            _projectHandle = -1;

            _progressPercentage = 0;
            _remainingTimeInSecs = 0;

            _result = false;

            _vFlashThread = new Thread(VectorBackgroundThread);
            _vFlashThread.SetApartmentState(ApartmentState.STA);
            _vFlashThread.IsBackground = true;
            _vFlashThread.Start();
        }

        public long ProjectHandle
        {
            get { return _projectHandle; }
            set { _projectHandle = value; }
        }

        public string FlashProjectPath
        {
            get { return _flashProjectPath; }
            set { _flashProjectPath = value; }
        }

        public bool Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public uint ProgressPercentage
        {
            get { return _progressPercentage; }
        }

        public uint RemainingTimeInSecs
        {
            get { return _remainingTimeInSecs; }
        }

        public override void Add(VFlashStationComponent c)
        {}

        public override void Remove(VFlashStationComponent c)
        {}

        public void ExecuteCommand(string command)
        {
            if (Array.LastIndexOf(new[] {"Load", "Unload", "Start", "Abort"}, command) != -1 && FlashProjectPath != "")
            {
                Command = command;
            }
        }

        protected override bool LoadProject()
        {
            long projectHandle;
            VFlashStationResult resLoad = VFlashStationAPI.LoadProjectForChannel(FlashProjectPath, ChannelId, out projectHandle);

            if (resLoad != VFlashStationResult.Success)
            {
                string errMsg = VFlashStationAPI.GetLastErrorMessage(projectHandle);
                _reportErrorDelegate(ChannelId, projectHandle, String.Format("Loading project failed ({0}) -> Rewind!", errMsg));
                UnloadProject();
                return false;
            }
            ProjectHandle = projectHandle;
            return true;
        }

        protected override bool UnloadProject()
        {
            if (ProjectHandle != -1)
            {
                VFlashStationResult resUnload = VFlashStationAPI.UnloadProject(ProjectHandle);
                if (resUnload != VFlashStationResult.Success)
                {
                    string errMsg = VFlashStationAPI.GetLastErrorMessage(ProjectHandle);
                    _reportErrorDelegate(ChannelId, ProjectHandle, String.Format("Unload project failed ({0}).", errMsg));
                }
                ProjectHandle = -1;
            }
            return true;
        }

        public override bool StartFlashing()
        {
            if (ProjectHandle != -1)
            {
                VFlashStationResult res = VFlashStationAPI.Start(ProjectHandle, UpdateProgress, UpdateStatus);
                if (res != VFlashStationResult.Success)
                {
                    string errMsg = VFlashStationAPI.GetLastErrorMessage(ProjectHandle);
                    _reportErrorDelegate(ChannelId, ProjectHandle, String.Format("Start reprogramming failed ({0}).", errMsg));
                }
            }
            return true;
        }

        public override bool AbortFlashing()
        {
            bool errorOccurredButContinued = false;
            if (ProjectHandle != -1)
            {
                VFlashStationResult res = VFlashStationAPI.Stop(ProjectHandle);
                if (res != VFlashStationResult.Success)
                {
                    string errMsg = VFlashStationAPI.GetLastErrorMessage(ProjectHandle);
                    _reportErrorDelegate(ChannelId, ProjectHandle, String.Format("Stop reprogramming failed ({0}).", errMsg));
                    errorOccurredButContinued = true;
                }
            }
            return !errorOccurredButContinued;
        }

        internal void UpdateStatus(long handle, VFlashStationStatus status)
        {
           Status = status.ToString();
            if (status == VFlashStationStatus.Success)
            {
                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Flashed succesfully");
            }
            else
            {
                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Flashing failed with reason: " + status);
                _reportErrorDelegate(ChannelId, ProjectHandle, String.Format("Flashing failed ({0}).", status));
            }
        }

        internal void UpdateProgress(long handle, uint progressInPercent, uint remainingTimeInSecs)
        {
            _progressPercentage = progressInPercent;
            _remainingTimeInSecs = remainingTimeInSecs;
        }

        private void VectorBackgroundThread()
        {
            while (_vFlashThread.IsAlive)
            {
                switch (Command)
                {
                    default:
                        Command = "";
                        break;
                    case "Load":
                        Status = "Loading";
                        Result = LoadProject();
                        if (Result)
                        {
                            Command = "";
                            Result = false;
                            Status = "Loaded";
                            Logger.Log("VFlash: Channel nr. " + ChannelId + " : Loaded succesfully");
                        }
                        break;
                    case "Unload":
                        Status = "Unloading";
                        Result = UnloadProject();
                        if (Result)
                        {
                            Command = "";
                            Result = false;
                            Status = "Unloaded";
                            Logger.Log("VFlash: Channel nr. " + ChannelId + " : Unloaded succesfully");
                        }
                        break;
                    case "Start":
                        Status = "Flashing";
                        Result = StartFlashing();
                        if (Result)
                        {
                            Command = "";
                            Result = false;
                            Status = "Flashing";
                        }
                        break;
                    case "Abort":
                        Status = "Aborting";
                        Result = AbortFlashing();
                        if (Result)
                        {
                            Command = "";
                            Result = false;
                            Status = "Aborted";
                            Logger.Log("VFlash: Channel nr. " + ChannelId + " : Aborted");
                        }
                        break;
                }
                Thread.Sleep(50);
            }
        }
    }

    #endregion

}
