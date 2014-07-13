using System.Windows;
using System.Windows.Controls;

namespace _ttAgent.Visual
{
    /// <summary>
    /// Interaction logic for ComponentCreationWindowTreeView.xaml
    /// </summary>
    public partial class ComponentCreationWindowTreeView : UserControl
    {
        public ComponentCreationWindowTreeView()
        {
            InitializeComponent();
        }

        private void ComponentManagerSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = (TreeView)sender;
            var selection = (TreeViewItem)treeView.SelectedItem;
            if (selection != null) ComponentManagerSelectionLabel.Content = selection.Header;
        }
    }
}
