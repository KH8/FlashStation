using System.Collections.ObjectModel;
using System.Windows;
using _ttAgent.General;
using _ttAgent.MainRegistry;
using _ttAgent.Output;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiInterfaceAssignment
    {
        private readonly Module _module;

        public ObservableCollection<InterfaceAssignment> InterfaceAssignmentCollection;public RegistryComponent.RegistryComponentHeader Header;

        public GuiInterfaceAssignment(int xPosition, int yPosition, Module module)
        {
            InitializeComponent();
            GeneralGrid.Margin = new Thickness(xPosition,yPosition,0,0);

            _module = module;
            InterfaceAssignmentCollection = module.InterfaceAssignmentCollection.Children;
            AssignmentDataGrid.ItemsSource = InterfaceAssignmentCollection;
        }

        private void AssignmentChanged(object sender, System.EventArgs e)
        {
            _module.UpdateAssignment();
        }
    }
}
