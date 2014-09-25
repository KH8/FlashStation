using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using _PlcAgent.Properties;
using _PlcAgent.MainRegistry;

namespace _PlcAgent.General
{
    public abstract class Module : RegistryComponent, INotifyPropertyChanged
    {
        protected Module(uint id, string name) : base(id, name) { }

        public abstract void Initialize();
        public abstract void Deinitialize();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public abstract class ExecutiveModule : Module
    {
        private Boolean _pcControlMode;
        protected Boolean PcControlModeChangeAllowed  { get; set; }

        public InterfaceAssignmentCollection InterfaceAssignmentCollection { get; set; }

        public abstract void UpdateAssignment();

        protected ExecutiveModule(uint id, string name) : base(id, name){}

        public Boolean PcControlMode
        {
            get { return _pcControlMode; }
            set { if (PcControlModeChangeAllowed) { _pcControlMode = value; } }
        }

        protected abstract Boolean CheckInterface();
        protected abstract void CreateInterfaceAssignment(uint id, string[][] assignment);
    }

    public abstract class OutputModule : ExecutiveModule
    {
        protected OutputModule(uint id, string name) : base(id, name)
        {
        }
    }

    public abstract class InputModule : ExecutiveModule
    {
        protected InputModule(uint id, string name) : base(id, name)
        {
        }
    }
}
