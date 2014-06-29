using System.Collections.Generic;
using System.Collections.ObjectModel;
using _ttAgent.DataAquisition;

namespace _ttAgent.General
{
    public struct InterfaceAssignment
    {
        public enum Direction
        {
            In,
            Out,
        }

        public Direction VariableDirection { get; set; }
        public string Name { get; set; }
        public CommunicationInterfaceComponent.VariableType Type { get; set; }
        public string Assignment { get; set; }
    }

    public class InterfaceAssignmentCollection
    {
        public ObservableCollection<InterfaceAssignment> Children { get; set; }

        public InterfaceAssignmentCollection()
        {
            Children = new ObservableCollection<InterfaceAssignment>();
        }
    }
}
