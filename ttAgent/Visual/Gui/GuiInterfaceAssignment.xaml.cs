using System;
using System.Collections.ObjectModel;
using System.Windows;
using _ttAgent.General;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiInterfaceAssignment
    {
        private readonly Module _module;

        public ObservableCollection<InterfaceAssignment> InterfaceAssignmentCollection;

        public GuiInterfaceAssignment(Module module)
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
