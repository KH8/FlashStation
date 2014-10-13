using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _PlcAgent.General;
using _PlcAgent.MainRegistry;
using _PlcAgent.Visual.TreeListView;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiInterfaceAssignment
    {
        #region Variables

        private readonly ExecutiveModule _module;

        #endregion


        #region Properties

        public ObservableCollection<InterfaceAssignment> InterfaceAssignmentCollection;

        #endregion


        #region Constructors

        public GuiInterfaceAssignment(ExecutiveModule module)
        {
            _module = module;
            InitializeComponent();
            InterfaceAssignmentCollection = _module.InterfaceAssignmentCollection.Children;
            AssignmentDataGrid.ItemsSource = InterfaceAssignmentCollection;
        }

        #endregion


        #region EventHandlers

        private void AssignmentChanged(object sender, EventArgs e)
        {
            _module.UpdateAssignment();
        }

        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("Name") || sender == e.Source) e.Effects = DragDropEffects.None;
        }

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("Name")) return;
            var displayData = e.Data.GetData("Name") as DisplayDataBuilder.DisplayData;

            var hit = VisualTreeHelper.HitTest(AssignmentDataGrid, e.GetPosition(AssignmentDataGrid));

            if (hit.VisualHit.GetType() != typeof (TextBlock)) return;
            var textBlock = hit.VisualHit as TextBlock;

            if (textBlock == null) return;
            if (textBlock.BindingGroup == null) return;
            if (textBlock.BindingGroup.Owner == null) return;
            if (textBlock.BindingGroup.Owner.GetType() != typeof (DataGridRow)) return;

            var dataGridRow = (DataGridRow) textBlock.BindingGroup.Owner;
            var interfaceAssignment = (InterfaceAssignment) dataGridRow.Item;

            if (displayData == null) return;
            interfaceAssignment.Assignment = displayData.Name;

            AssignmentDataGrid.Items.Refresh();
            _module.UpdateAssignment();
        }

        #endregion

    }
}
