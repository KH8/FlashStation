using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Output.Template;
using _PlcAgent.Properties;
using _PlcAgent.Visual.TreeListView;

namespace _PlcAgent.MainRegistry
{
    public abstract class Module : RegistryComponent, INotifyPropertyChanged
    {
        protected Module(uint id, string name) : base(id, name) { }

        public abstract void Initialize();
        public abstract void Deinitialize();

        public abstract void TemplateGuiUpdate(TabControl mainTabControl, TabControl outputTabControl, TabControl connectionTabControl, Grid footerGrid);
        public abstract void TemplateRegistryComponentUpdateRegistryFile();
        public abstract void TemplateRegistryComponentCheckAssignment(RegistryComponent component);

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

        protected ExecutiveModule(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler)
            : base(id, name)
        {
            CommunicationInterfaceHandler = communicationInterfaceHandler;
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
                            component = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(assignment.Assignment);
                        break;
                        case InterfaceAssignment.Direction.Out:
                            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(assignment.Assignment);
                        break;
                }

                if (component == null ||
                    component.TypeOfVariable != assignment.Type)
                {
                    return false;
                }
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
            : base(id, name, communicationInterfaceHandler)
        {
            OutputDataTemplate = outputDataTemplate;
        }

        protected static string FileNameCreator(string fixedName, string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
            return directoryPath + "\\"
                + DateTime.Now.Year
                + FillTheStringUp(DateTime.Now.Month.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Day.ToString(CultureInfo.InvariantCulture)) + "_"
                + FillTheStringUp(DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Second.ToString(CultureInfo.InvariantCulture))
                + "_" + fixedName.Trim();
        }

        protected static string FillTheStringUp(string dateString)
        {
            if (dateString.Length < 2) return "0" + dateString;
            return dateString;
        }
    }

    public abstract class InputModule : ExecutiveModule
    {
        protected InputModule(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler)
            : base(id, name, communicationInterfaceHandler)
        {
        }
    }
}
