using System.Windows;
using System.Windows.Controls;
using _ttAgent.General;
using _ttAgent.Vector;

namespace _ttAgent.Visual.Gui
{
    internal class GuiInterfaceAssignmentComponent : Gui
    {
        public GuiInterfaceAssignment GuiInterfaceAssignment;

        public GuiInterfaceAssignmentComponent(uint id, string name, Module module)
            : base(id, name)
        {
            GuiInterfaceAssignment = new GuiInterfaceAssignment(id, name, module);
        }

        public override void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            GuiInterfaceAssignment.Initialize(xPosition, yPosition, generalGrid);
        }

        public override void MakeVisible()
        {
            GuiInterfaceAssignment.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible()
        {
            GuiInterfaceAssignment.Visibility = Visibility.Hidden;
        }
    }
}
