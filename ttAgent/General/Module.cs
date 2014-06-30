using _ttAgent.MainRegistry;
using _ttAgent.Output;

namespace _ttAgent.General
{
    public abstract class Module : RegistryComponent
    {
        public InterfaceAssignmentCollection InterfaceAssignmentCollection { get; set; }

        protected Module(uint id, string name) : base(id, name){}

        public abstract void UpdateAssignment();
    }
}
