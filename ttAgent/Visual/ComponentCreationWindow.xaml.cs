using System.Windows;
using System.Windows.Controls;

namespace ttAgent.Visual
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

        public ComponentCreationWindow(string prompt, TreeViewItem items, AssignmentDelegate assignmentDelegate)
        {
            InitializeComponent();
            _assignmentDelegate = assignmentDelegate;

            Prompt.Content = prompt;

            ComponentManagerTreeView.Visibility = Visibility.Visible;
            ComponentManagerTreeViewExtension.Visibility = Visibility.Hidden;

            ComponentManagerTreeView.Width = 428;
            ComponentManagerTreeView.Items.Add(items);
        }

        public ComponentCreationWindow(string prompt, TreeViewItem items, TreeViewItem itemsExtension, AssignmentDelegateExtended assignmentDelegate)
        {
            InitializeComponent();
            _assignmentDelegateExtended = assignmentDelegate;

            Prompt.Content = prompt;

            ComponentManagerTreeView.Visibility = Visibility.Visible;
            ComponentManagerTreeViewExtension.Visibility = Visibility.Visible;

            ComponentManagerTreeView.Items.Add(items);
            ComponentManagerTreeViewExtension.Items.Add(itemsExtension);
        }

        private void CancelSelection(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Done(object sender, RoutedEventArgs e)
        {
            var selectedItem = (TreeViewItem)ComponentManagerTreeView.SelectedItem;
            if (selectedItem == null) { return; }
            var result = (uint)selectedItem.AlternationCount;

            if (_assignmentDelegate != null)
            {
                _assignmentDelegate(result);
                Close();
                return;
            }

            selectedItem = (TreeViewItem)ComponentManagerTreeViewExtension.SelectedItem;
            if (selectedItem == null) { return; }
            var resultExtension = (uint)selectedItem.AlternationCount;

            _assignmentDelegateExtended(result, resultExtension);
            Close();
        }
    }
}
