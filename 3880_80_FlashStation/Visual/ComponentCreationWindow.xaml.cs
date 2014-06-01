using System;
using System.Windows;
using System.Windows.Controls;

namespace _3880_80_FlashStation.Visual
{
    /// <summary>
    /// Interaction logic for FaultReport.xaml
    /// </summary>
    public partial class ComponentCreationWindow
    {
        public delegate void AssignDelegate1(uint id1);
        public delegate void AssignDelegate2(uint id1, uint id2);

        private readonly AssignDelegate1 _assignDelegate1;
        private readonly AssignDelegate2 _assignDelegate2;

        public ComponentCreationWindow(string prompt, TreeViewItem items, AssignDelegate1 assignDelegate1)
        {
            InitializeComponent();
            _assignDelegate1 = assignDelegate1;

            Prompt.Content = prompt;
            ComponentManagerTreeView.Items.Add(items);
        }

        public ComponentCreationWindow(string prompt, TreeViewItem items, AssignDelegate2 assignDelegate2)
        {
            InitializeComponent();
            _assignDelegate2 = assignDelegate2;

            Prompt.Content = prompt;
            ComponentManagerTreeView.Items.Add(items);
        }

        private void CancelSelection(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Done(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)ComponentManagerTreeView.SelectedItem;
            if (item == null) { return; }
            var result = (uint) item.AlternationCount;

            if (_assignDelegate1 != null)
            {
                _assignDelegate1(result);
                Close();
                return;
            }

            _assignDelegate2(result, result);
            Close();
        }
    }
}
