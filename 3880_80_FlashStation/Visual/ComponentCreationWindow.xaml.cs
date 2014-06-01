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

            ComponentManagerTreeView1.Visibility = Visibility.Visible;
            ComponentManagerTreeView2.Visibility = Visibility.Hidden;

            ComponentManagerTreeView1.Width = 426;
            ComponentManagerTreeView1.Items.Add(items);
        }

        public ComponentCreationWindow(string prompt, TreeViewItem items1, TreeViewItem items2, AssignDelegate2 assignDelegate2)
        {
            InitializeComponent();
            _assignDelegate2 = assignDelegate2;

            Prompt.Content = prompt;

            ComponentManagerTreeView1.Visibility = Visibility.Visible;
            ComponentManagerTreeView2.Visibility = Visibility.Visible;

            ComponentManagerTreeView1.Items.Add(items1);
            ComponentManagerTreeView2.Items.Add(items2);
        }

        private void CancelSelection(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Done(object sender, RoutedEventArgs e)
        {
            var item1 = (TreeViewItem)ComponentManagerTreeView1.SelectedItem;
            if (item1 == null) { return; }
            var result1 = (uint) item1.AlternationCount;

            if (_assignDelegate1 != null)
            {
                _assignDelegate1(result1);
                Close();
                return;
            }

            var item2 = (TreeViewItem)ComponentManagerTreeView2.SelectedItem;
            if (item2 == null) { return; }
            var result2 = (uint)item2.AlternationCount;

            _assignDelegate2(result1, result2);
            Close();
        }
    }
}
