using System;
using System.Collections.ObjectModel;
using _PlcAgent.General;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiInterfaceAssignment
    {
        private readonly OutputModule _module;

        public ObservableCollection<InterfaceAssignment> InterfaceAssignmentCollection;

        public GuiInterfaceAssignment(OutputModule module)
        {
            _module = module;
            InitializeComponent();
            InterfaceAssignmentCollection = _module.InterfaceAssignmentCollection.Children;
            AssignmentDataGrid.ItemsSource = InterfaceAssignmentCollection;
        }

        private void AssignmentChanged(object sender, EventArgs e)
        {
            _module.UpdateAssignment();
        }
    }
}
