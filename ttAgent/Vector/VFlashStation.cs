using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vector.vFlash.Automation;
using _ttAgent.Log;

namespace _ttAgent.Vector
{
    #region Component

    public abstract class VFlashStationComponent
    {
        private readonly uint _channelId;
        private VFlashCommand _command;
        private VFlashStatus _status;

        public delegate void ReportErrorDelegate(uint channelId, long handle, string message);

        protected VFlashStationComponent(uint id)
        {
            _channelId = id;
        }

        public enum VFlashCommand
        {
            Load,
            Unload,
            Start,
            Abort,
            NoCommand
        }

        public enum VFlashStatus
        {
            Created,
            Loading,
            Loaded,
            Unloading,
            Unloaded,
            Flashing,
            Flashed,
            Aborting,
            Aborted,
            Fault
        }

        public uint ChannelId
        {
            get { return _channelId; }
        }

        public VFlashCommand Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public VFlashStatus Status
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
            
            if (resInit != VFlashStationResult.Success)
            {
                string errMsg = VFlashStationAPI.GetLastErrorMessage(-1);
                _reportErrorDelegate(0, -1, String.Format("ID: " + ChannelId + " Initialization of vFlash Station failed ({0}).", errMsg));
                return false;
            }
            Logger.Log("ID: " + ChannelId + " vFlash Station: Initialized");
            return true;
        }

        internal bool Deinitialize()
        {
            //--- Deinitialize vFlash Station ---
            var resDeinit = VFlashStationAPI.Deinitialize();

            if (resDeinit != VFlashStationResult.Success)
            {
                string errMsg = VFlashStationAPI.GetLastErrorMessage(-1);
                _reportErrorDelegate(0, -1, String.Format("ID: " + ChannelId + " Deinitialization of vFlash Station during rewinding failed ({0}).", errMsg));
            }
            Logger.Log("ID: " + ChannelId + " vFlash Station: Deinitialized");
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
                channel.ExecuteCommand(VFlashCommand.Load);
            }
            return true;
        }

        protected override bool UnloadProject()
        {
            foreach (var vFlashStationComponent in _children)
            {
                var channel = (VFlashChannel) vFlashStationComponent;
                channel.ExecuteCommand(VFlashCommand.Unload);
            }
            return true;
        }

        public override bool StartFlashing()
        {
            foreach (var vFlashStationComponent in _children)
            {
                var channel = (VFlashChannel) vFlashStationComponent;
                channel.ExecuteCommand(VFlashCommand.Start);
            }
            return true;
        }

        public override bool AbortFlashing()
        {
            foreach (var vFlashStationComponent in _children)
            {
                var channel = (VFlashChannel) vFlashStationComponent;
                channel.ExecuteCommand(VFlashCommand.Abort);
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
            Command = VFlashCommand.NoCommand;
            Status = VFlashStatus.Created;

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

        public void ExecuteCommand(VFlashCommand command)
        {
            if (FlashProjectPath != "")
            {
                Command = command;
            }
        }

        protected override bool LoadProject()
        {
            long projectHandle;
            var resLoad = VFlashStationAPI.LoadProjectForChannel(FlashProjectPath, ChannelId, out projectHandle);

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
                var resUnload = VFlashStationAPI.UnloadProject(ProjectHandle);
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
                var res = VFlashStationAPI.Start(ProjectHandle, UpdateProgress, UpdateStatus);
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
            var errorOccurredButContinued = false;
            if (ProjectHandle != -1)
            {
                var res = VFlashStationAPI.Stop(ProjectHandle);
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
            if (status == VFlashStationStatus.Success)
            {
                Status = VFlashStatus.Flashed;
                _progressPercentage = 0;
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
                        Command = VFlashCommand.NoCommand;
                        break;
                    case VFlashCommand.Load:
                        if (new List<VFlashStatus> { VFlashStatus.Created, VFlashStatus.Fault, VFlashStatus.Unloaded }.Contains(Status))
                        {
                            Status = VFlashStatus.Loading;
                            Result = LoadProject();
                            if (Result)
                            {
                                Command = VFlashCommand.NoCommand;
                                Result = false;
                                Status = VFlashStatus.Loaded;
                                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Loaded succesfully");
                            }
                        }
                        break;
                    case VFlashCommand.Unload:
                        if (new List<VFlashStatus> { VFlashStatus.Loaded, VFlashStatus.Fault, VFlashStatus.Flashed, VFlashStatus.Aborted }.Contains(Status))
                        {
                            Status = VFlashStatus.Unloading;
                            Result = UnloadProject();
                            if (Result)
                            {
                                Command = VFlashCommand.NoCommand;
                                Result = false;
                                Status = VFlashStatus.Unloaded;
                                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Unloaded succesfully");
                            }
                        }
                        break;
                    case VFlashCommand.Start:
                        if (new List<VFlashStatus> { VFlashStatus.Loaded, VFlashStatus.Fault, VFlashStatus.Flashed, VFlashStatus.Aborted }.Contains(Status))
                        {
                            Status = VFlashStatus.Flashing;
                            Result = StartFlashing();
                            if (Result)
                            {
                                Command = VFlashCommand.NoCommand;
                                Result = false;
                                Status = VFlashStatus.Flashing;
                            }
                        }
                        break;
                    case VFlashCommand.Abort:
                        if (new List<VFlashStatus> { VFlashStatus.Flashing }.Contains(Status))
                        {
                            Status = VFlashStatus.Aborting;
                            Result = AbortFlashing();
                            if (Result)
                            {
                                Command = VFlashCommand.NoCommand;
                                Result = false;
                                Status = VFlashStatus.Aborted;
                                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Aborted");
                            }
                        }
                        break;
                }
                Thread.Sleep(50);
            }
        }
    }

    #endregion

}
