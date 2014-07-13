using System.Windows;
using System.Windows.Controls;

namespace _ttAgent.Visual
{
    /// <summary>
    /// Interaction logic for FaultReport.xaml
    /// </summary>
    public partial class ComponentCreationWindow
    {
        public delegate void AssignmentDelegate(uint id);
        public delegate void AssignmentDelegateExtended(uint id, uint idExtension);

        private readonly AssignmentDelegate _assignmentDelegate;
        private readonly AssignmentDelegateExtended _assignmentDelegateExtended;

        private readonly ComponentCreationWindowTreeView _componentCreationWindowTreeView;
        private readonly ComponentCreationWindowTreeView _componentCreationWindowTreeViewExtended;

        public ComponentCreationWindow(string prompt, TreeViewItem items, AssignmentDelegate assignmentDelegate)
        {
            InitializeComponent();
            _assignmentDelegate = assignmentDelegate;

            Prompt.Content = prompt;

            _componentCreationWindowTreeView = new ComponentCreationWindowTreeView();
            _componentCreationWindowTreeView.ComponentManagerTreeView.Items.Add(items);
            _componentCreationWindowTreeView.Margin = new Thickness(0,30,0,0);

            GeneralGrid.Children.Add(_componentCreationWindowTreeView);
            GeneralGrid.Height = 185;
            Height = 225;
        }

        public ComponentCreationWindow(string prompt, TreeViewItem items, TreeViewItem itemsExtension, AssignmentDelegateExtended assignmentDelegate)
        {
            InitializeComponent();
            _assignmentDelegateExtended = assignmentDelegate;

            Prompt.Content = prompt;

            _componentCreationWindowTreeView = new ComponentCreationWindowTreeView();
            _componentCreationWindowTreeView.ComponentManagerTreeView.Items.Add(items);
            _componentCreationWindowTreeView.Margin = new Thickness(0, 30, 0, 0);

            _componentCreationWindowTreeViewExtended = new ComponentCreationWindowTreeView();
            _componentCreationWindowTreeViewExtended.ComponentManagerTreeView.Items.Add(itemsExtension);
            _componentCreationWindowTreeViewExtended.Margin = new Thickness(0, 142, 0, 0);

            GeneralGrid.Children.Add(_componentCreationWindowTreeView);
            GeneralGrid.Children.Add(_componentCreationWindowTreeViewExtended);
            GeneralGrid.Height = 297;
            Height = 337;
        }

        private void CancelSelection(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Done(object sender, RoutedEventArgs e)
        {
            var selectedItem = (TreeViewItem)_componentCreationWindowTreeView.ComponentManagerTreeView.SelectedItem;
            if (selectedItem == null) { return; }
            var result = (uint)selectedItem.AlternationCount;
            if (result == 0) { return; }

            if (_assignmentDelegate != null)
            {
                _assignmentDelegate(result);
                Close();
                return;
            }

            selectedItem = (TreeViewItem)_componentCreationWindowTreeViewExtended.ComponentManagerTreeView.SelectedItem;
            if (selectedItem == null) { return; }
            var resultExtension = (uint)selectedItem.AlternationCount;
            if (resultExtension == 0) { return; }

            _assignmentDelegateExtended(result, resultExtension);
            Close();
        }
    }
}
