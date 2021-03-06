﻿using System.Collections.ObjectModel;
using System.Linq;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.General
{
    public class InterfaceAssignment
    {
        public enum Direction
        {
            In,
            Out,
        }

        public uint Id { get; set; }
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

        public string GetAssignment(string name)
        {
            return Children.Where(assignment => assignment.Name == name).Select(assignment => assignment.Assignment).FirstOrDefault();
        }

        public void Add(uint id, string name, CommunicationInterfaceComponent.VariableType type, InterfaceAssignment.Direction direction, string[] assignment)
        {
            Children.Add(new InterfaceAssignment
            {
                Id = id,
                VariableDirection = direction,
                Name = name,
                Type = type,
                Assignment = assignment[id]
            });
        }
    }
}
