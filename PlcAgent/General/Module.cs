using System;
using _PlcAgent.MainRegistry;

namespace _PlcAgent.General
{
    public abstract class Module : RegistryComponent
    {
        protected Module(uint id, string name) : base(id, name) { }

        public abstract void Initialize();
        public abstract void Deinitialize();
    }

    public abstract class OutputModule : Module
    {
        public InterfaceAssignmentCollection InterfaceAssignmentCollection { get; set; }

        public abstract void UpdateAssignment();

        protected OutputModule(uint id, string name) : base(id, name){}

        protected abstract Boolean CheckInterface();
        protected abstract void CreateInterfaceAssignment(uint id, string[][] assignment);
    }
}
