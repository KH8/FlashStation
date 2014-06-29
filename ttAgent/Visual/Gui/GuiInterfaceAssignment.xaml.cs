using System.Windows;
using _ttAgent.General;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiInterfaceAssignment
    {
        private InterfaceAssignmentCollection _interfaceAssignmentCollection;

        public GuiInterfaceAssignment(int xPosition, int yPosition, InterfaceAssignmentCollection interfaceAssignmentCollection)
        {
            InitializeComponent();
            GeneralGrid.Margin = new Thickness(xPosition,yPosition,0,0);
            _interfaceAssignmentCollection = interfaceAssignmentCollection;
            AssignmentDataGrid.ItemsSource = _interfaceAssignmentCollection.Children;
        }
    }
}
