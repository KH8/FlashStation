using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using _PlcAgent.DataAquisition;
using _PlcAgent.Output.Template;
using _PlcAgent.Properties;
using _PlcAgent.MainRegistry;
using _PlcAgent.Visual.TreeListView;

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
        protected string[] Assignment { get; set; }

        public InterfaceAssignmentCollection InterfaceAssignmentCollection { get; set; }
        public CommunicationInterfaceHandler CommunicationInterfaceHandler { get; set; }

        protected delegate void OnAssignmentUpdateDelegate();
        protected OnAssignmentUpdateDelegate OnAssignmentUpdate;
        abstract protected void AssignmentFileUpdate();

        protected ExecutiveModule(uint id, string name) : base(id, name)
        {
            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();
            OnAssignmentUpdate += AssignmentFileUpdate;
        }

        public Boolean PcControlMode
        {
            get { return _pcControlMode; }
            set { if (PcControlModeChangeAllowed) { _pcControlMode = value; } }
        }

        protected abstract void CreateInterfaceAssignment();

        protected Boolean CheckInterface()
        {
            foreach (var assignment in InterfaceAssignmentCollection.Children)
            {
                CommunicationInterfaceComponent component = null;

                switch (assignment.VariableDirection)
                {
                        case InterfaceAssignment.Direction.In:
                            component = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(assignment.Name);
                        break;
                        case InterfaceAssignment.Direction.Out:
                            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(assignment.Name);
                        break;
                }

                if (component == null || component.TypeOfVariable != CommunicationInterfaceComponent.VariableType.Integer) return false;
            }
            return true;
        }

        public void UpdateAssignment()
        {
            foreach (var assignment in InterfaceAssignmentCollection.Children) { Assignment[assignment.Id] = assignment.Assignment; }
            if (OnAssignmentUpdate != null) OnAssignmentUpdate();
        }
    }

    public abstract class OutputModule : ExecutiveModule
    {
        private readonly DisplayDataBuilder.DisplayDataContainer _outputDataTemplateCollection = new DisplayDataBuilder.DisplayDataContainer();

        public OutputDataTemplate OutputDataTemplate { get; set; }
        public DisplayDataBuilder.DisplayDataContainer ReadInterfaceCollection { get { return _outputDataTemplateCollection; } }

        protected OutputModule(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, OutputDataTemplate outputDataTemplate)
            : base(id, name)
        {
            CommunicationInterfaceHandler = communicationInterfaceHandler;
            OutputDataTemplate = outputDataTemplate;
        }
    }

    public abstract class InputModule : ExecutiveModule
    {
        protected InputModule(uint id, string name) : base(id, name)
        {
        }
    }
}
