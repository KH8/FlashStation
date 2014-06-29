using System.Windows;
using _ttAgent.General;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiInterfaceAssignment
    {
        public GuiInterfaceAssignment(int xPosition, int yPosition, InterfaceAssignmentCollection interfaceAssignmentCollection)
        {
            InitializeComponent();
            GeneralGrid.Margin = new Thickness(xPosition,yPosition,0,0);
            AssignmentDataGrid.ItemsSource = interfaceAssignmentCollection.Children;
        }
    }
}
