﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Vector.vFlash.Automation;
using _PlcAgent.Annotations;
using _PlcAgent.Log;

namespace _PlcAgent.Vector
{
    #region Component

    public abstract class VFlashStationComponent : INotifyPropertyChanged
    {
        #region Variables

        private readonly uint _channelId;
        private VFlashCommand _command;
        private VFlashStatus _status;

        #endregion


        #region Properties

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
            Sequence,
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
            SequenceActive,
            SequenceDone,
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
            set
            {
                if (Equals(value, _command)) return;
                _command = value;
                OnPropertyChanged();
            }
        }

        public VFlashStatus Status
        {
            get { return _status; }
            set
            {
                if (Equals(value, _status)) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        #region Methods

        public abstract void Add(VFlashStationComponent c);
        public abstract void Remove(VFlashStationComponent c);
        public abstract bool LoadProject();
        public abstract bool UnloadProject();
        public abstract bool StartFlashing();
        public abstract bool AbortFlashing();

        #endregion


    }

    #endregion

    #region Controller (Composite) 

    static class VFlashStationControllerContext
    {
        public static VFlashStationController VFlashStationController { get; set; }
    }

    internal class VFlashStationController : VFlashStationComponent
    {
        private readonly List<VFlashStationComponent> _children = new List<VFlashStationComponent>();
        private readonly ReportErrorDelegate _reportErrorDelegate;
        
        public List<VFlashStationComponent> Children
        {
            get { return _children; }
        }

        public Boolean IsEmpty 
        {
            get
            {
                if (Children.Count == 0) return true;
                return false;
            }
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

        public override bool LoadProject()
        {
            foreach (var channel in _children.Cast<VFlashChannel>())
            {
                channel.ExecuteCommand(VFlashCommand.Load);
            }
            return true;
        }

        public override bool UnloadProject()
        {
            foreach (var channel in _children.Cast<VFlashChannel>())
            {
                channel.ExecuteCommand(VFlashCommand.Unload);
            }
            return true;
        }

        public override bool StartFlashing()
        {
            foreach (var channel in _children.Cast<VFlashChannel>())
            {
                channel.ExecuteCommand(VFlashCommand.Start);
            }
            return true;
        }

        public override bool AbortFlashing()
        {
            foreach (var channel in _children.Cast<VFlashChannel>())
            {
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

    public class VFlashChannel : VFlashStationComponent
    {
        #region Variables

        private long _projectHandle;
        private string _flashProjectPath;
        private VFlashTypeBank.VFlashTypeComponent _flashingSequence;
        private VFlashTypeBank.VFlashTypeComponent.Step _flashingStep;
        private Boolean _result;

        private uint _progressPercentage;
        private uint _remainingTimeInSecs;

        private readonly ReportErrorDelegate _reportErrorDelegate;

        private readonly Thread _vFlashThread;

        #endregion


        #region Constructors

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

        #endregion


        public long ProjectHandle
        {
            get { return _projectHandle; }
            set
            {
                if (Equals(value, _projectHandle)) return;
                _projectHandle = value;
                OnPropertyChanged();
            }
        }

        public string FlashProjectPath
        {
            get { return _flashProjectPath; }
            set
            {
                if (Equals(value, _flashProjectPath)) return;
                _flashProjectPath = value;
                OnPropertyChanged();
            }
        }

        public VFlashTypeBank.VFlashTypeComponent FlashingSequence
        {
            get { return _flashingSequence; }
            set
            {
                if (Equals(value, _flashingSequence)) return;
                _flashingSequence = value;
                OnPropertyChanged();
            }
        }

        public VFlashTypeBank.VFlashTypeComponent.Step FlashingStep
        {
            get { return _flashingStep; }
            set
            {
                if (Equals(value, _flashingStep)) return;
                _flashingStep = value;
                OnPropertyChanged();
            }
        }

        public bool Result
        {
            get { return _result; }
            set
            {
                if (Equals(value, _result)) return;
                _result = value;
                OnPropertyChanged();
            }
        }

        public uint ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                if (Equals(value, _progressPercentage)) return;
                _progressPercentage = value;
                OnPropertyChanged();
            }
        }

        public uint RemainingTimeInSecs
        {
            get { return _remainingTimeInSecs; }
            set
            {
                if (Equals(value, _remainingTimeInSecs)) return;
                _remainingTimeInSecs = value;
                OnPropertyChanged();
            }
        }

        public override void Add(VFlashStationComponent c)
        {}

        public override void Remove(VFlashStationComponent c)
        {}

        public void ExecuteCommand(VFlashCommand command)
        {
            Command = command;
        }

        public override bool LoadProject()
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

        public override bool UnloadProject()
        {
            if (ProjectHandle != -1)
            {
                var resUnload = VFlashStationAPI.UnloadProject(ProjectHandle);
                if (resUnload != VFlashStationResult.Success)
                {
                    string errMsg = VFlashStationAPI.GetLastErrorMessage(ProjectHandle);
                    _reportErrorDelegate(ChannelId, ProjectHandle, String.Format("Unload project failed ({0}).", errMsg));
                    return false;
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
                    return false;
                }
            }
            return true;
        }

        public bool StartFlashing(VFlashCommand command)
        {
            if (ProjectHandle != -1)
            {
                var res = VFlashStationAPI.Start(ProjectHandle, UpdateProgress, UpdateStatusConditioned);
                if (res != VFlashStationResult.Success)
                {
                    string errMsg = VFlashStationAPI.GetLastErrorMessage(ProjectHandle);
                    _reportErrorDelegate(ChannelId, ProjectHandle, String.Format("Start reprogramming failed ({0}).", errMsg));
                    return false;
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

        public bool ExecuteSequence(VFlashTypeBank.VFlashTypeComponent sequence)
        {
            Logger.Log("VFlash: Channel nr. " + ChannelId + " : Sequence " + sequence.Version + " has started");

            for (var i = 1; i < sequence.Steps.Count + 1; i++)
            {
                var id = i;
                FlashingStep = sequence.Steps.First(step => step.Id == id);

                //Unloading
                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Unloading");

                Status = VFlashStatus.Unloading;
                if (UnloadProject())
                {
                    Status = VFlashStatus.Unloaded;
                    Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Unloaded succesfully");

                    //Loading
                    Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Loading");

                    FlashProjectPath = FlashingStep.Path;
                    Status = VFlashStatus.Loading;
                    if (LoadProject())
                    {
                        Status = VFlashStatus.Loaded;
                        Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Loaded succesfully");

                        //Flashing
                        Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Flashing start");

                        Status = VFlashStatus.Flashing;
                        if (StartFlashing(VFlashCommand.Sequence))
                        {
                            Status = VFlashStatus.Flashing;
                        }

                        while (Status == VFlashStatus.Flashing)
                        {
                            Thread.Sleep(250);
                        }

                        if (Status != VFlashStatus.Flashed)
                        {
                            Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Flashing failed");
                            return false;
                        }

                        Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Flashed succesfully");
                        Thread.Sleep(FlashingStep.TransitionDelay);
                    }
                    else
                    {
                        Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Loading failed");
                        return false;
                    }
                }
                else
                {
                    Logger.Log("VFlash: Channel nr. " + ChannelId + " : Step " + i + " : Unloading failed");
                    return false;
                }
            }

            Status = VFlashStatus.SequenceDone;
            return true;
        }

        internal void UpdateStatus(long handle, VFlashStationStatus status)
        {
            if (status == VFlashStationStatus.Success)
            {
                Status = VFlashStatus.Flashed;
                ProgressPercentage = 0;
                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Flashed succesfully");
            }
            else
            {
                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Flashing failed with reason: " + status);
                _reportErrorDelegate(ChannelId, ProjectHandle, String.Format("Flashing failed ({0}).", status));
            }
        }

        internal void UpdateStatusConditioned(long handle, VFlashStationStatus status)
        {
            var statusCondition = FlashingStep.TransitionConditions.First(vFlashTypeComponentStepCondition => vFlashTypeComponentStepCondition.Status == status);
            if (statusCondition.Condition)
            {
                Status = VFlashStatus.Flashed;
                ProgressPercentage = 0;
                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Flashed succesfully in accordance with conditions set: " + status);
            }
            else
            {
                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Flashing failed with reason: " + status);
                _reportErrorDelegate(ChannelId, ProjectHandle, String.Format("Flashing failed ({0}).", status));
            }
        }

        internal void UpdateProgress(long handle, uint progressInPercent, uint remainingTimeInSecs)
        {
            ProgressPercentage = progressInPercent;
            RemainingTimeInSecs = remainingTimeInSecs;
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
                    case VFlashCommand.Sequence:
                        if (new List<VFlashStatus> { VFlashStatus.Created, VFlashStatus.Loaded, VFlashStatus.Fault, VFlashStatus.Flashed,  VFlashStatus.Aborted, VFlashStatus.Unloaded, VFlashStatus.SequenceDone }.Contains(Status))
                        {
                            Status = VFlashStatus.SequenceActive;
                            Result = ExecuteSequence(FlashingSequence);
                            if (Result)
                            {
                                Command = VFlashCommand.NoCommand;
                                Result = false;
                                Status = VFlashStatus.SequenceDone;
                                Logger.Log("VFlash: Channel nr. " + ChannelId + " : Sequence executed succesfully");
                            }
                        }
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
                        if (new List<VFlashStatus> { VFlashStatus.Loaded, VFlashStatus.Fault, VFlashStatus.Flashed, VFlashStatus.Aborted, VFlashStatus.SequenceDone }.Contains(Status))
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
                        if (new List<VFlashStatus> { VFlashStatus.Loaded, VFlashStatus.Fault, VFlashStatus.Flashed, VFlashStatus.Aborted, VFlashStatus.SequenceDone }.Contains(Status))
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
