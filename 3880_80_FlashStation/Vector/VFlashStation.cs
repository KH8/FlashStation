using System;
using System.Collections.Generic;
using System.Linq;
using Vector.vFlash.Automation;

namespace _3880_80_FlashStation.Vector
{
    #region Component

    public abstract class VFlashStationComponent
    {
        private uint _channelId;
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
        public abstract bool LoadProject();
        public abstract bool UnloadProject();
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

        /// <summary>
        /// Initializes a new instance of the <see cref="VFlashStationController"/> class.
        /// </summary>
        /// <param name="reportErrorDelegate">The report error delegate.</param>
        /// <param name="id">Id</param>
        internal VFlashStationController(ReportErrorDelegate reportErrorDelegate, uint id)
            : base(id)
        {
            _reportErrorDelegate = reportErrorDelegate;
        }

        internal bool Initialize()
        {
            //--- Initialize vFlash Station ---
            VFlashStationResult resInit = VFlashStationAPI.Initialize();
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
            VFlashStationResult resDeinit = VFlashStationAPI.Deinitialize();
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

        public override bool LoadProject()
        {
            foreach (VFlashChannel channel in _children)
            {
                channel.LoadProject();
            }
            return true;
        }

        public override bool UnloadProject()
        {
            foreach (VFlashChannel channel in _children)
            {
                channel.UnloadProject();
            }
            return true;
        }

        public override bool StartFlashing()
        {
            foreach (VFlashChannel channel in _children)
            {
                channel.StartFlashing();
            }
            return true;
        }

        public override bool AbortFlashing()
        {
            foreach (VFlashChannel channel in _children)
            {
                channel.AbortFlashing();
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
        private CallbackProgressDelegate _progressDelegate;
        private CallbackStatusDelegate _statusDelegate;

        private readonly ReportErrorDelegate _reportErrorDelegate;

        public VFlashChannel(ReportErrorDelegate reportErrorDelegate, uint channelId)
            : base(channelId)
        {
            _reportErrorDelegate = reportErrorDelegate;
            _projectHandle = -1;
        }

        public VFlashChannel(ReportErrorDelegate reportErrorDelegate, string flashProjectPath, uint channelId,
            CallbackProgressDelegate progressDelegate, CallbackStatusDelegate statusDelegate)
            : base(channelId)
        {
            Command = "";
            Status = "Created";

            _reportErrorDelegate = reportErrorDelegate;
            _progressDelegate = progressDelegate;
            _statusDelegate = statusDelegate;
            _flashProjectPath = flashProjectPath;
            _projectHandle = -1;

            _result = false;
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

        public CallbackProgressDelegate ProgressDelegate
        {
            get { return _progressDelegate; }
            set { _progressDelegate = value; }
        }

        public CallbackStatusDelegate StatusDelegate
        {
            get { return _statusDelegate; }
            set { _statusDelegate = value; }
        }

        public bool Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public override void Add(VFlashStationComponent c)
        {}

        public override void Remove(VFlashStationComponent c)
        {}

        public override bool LoadProject()
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

        public override bool UnloadProject()
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
                VFlashStationResult res = VFlashStationAPI.Start(ProjectHandle, ProgressDelegate, StatusDelegate);

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
    }

    #endregion

}
