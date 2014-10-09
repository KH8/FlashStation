using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using _PlcAgent.General;
using _PlcAgent.Output.Template;
using _PlcAgent.Visual.Interfaces;
using _PlcAgent.Visual.TreeListView;

namespace _PlcAgent.Visual.Gui.Output
{
    /// <summary>
    /// Interaction logic for GuiOutputDataTemplate.xaml
    /// </summary>
    public partial class GuiOutputDataTemplate : IResizableGui
    {
        #region Variables

        private Point _storedPosition;

        #endregion


        #region Properties

        public TabItem TabItem = new TabItem();

        #endregion


        #region Constructors

        public GuiOutputDataTemplate(OutputDataTemplate template) : base(template)
        {
            InitializeComponent();

            var collection = new ObservableCollection<object>();
            new DisplayDataHierarchicalBuilder().Build(collection, OutputDataTemplate.Composite);

            OutputDataTemplateTreeListView.ItemsSource = collection;
        }

        #endregion


        #region Methods

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            OutputDataTemplateTreeListView.Height = height - 30;
            OutputDataTemplateTreeListView.Width = Limiter.DoubleLimit(width, 0);

            FooterGrid.Width = Limiter.DoubleLimit(width, 0);
        }

        #endregion


        #region Event Handlers

        public void OnInterfaceUpdatedDelegate()
        {
        }

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            _storedPosition = e.GetPosition(null);
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            var mousePos = e.GetPosition(null);
            var diff = _storedPosition - mousePos;

            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance) &&
                !(Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) return;
            // Get the dragged TreeListViewItem
            var treeListViewItem = FindAncestor<TreeListViewItem>((DependencyObject)e.OriginalSource);

            // Find the data behind the TreeListViewItem
            if (treeListViewItem == null) return;
            var displayData = (DisplayDataBuilder.DisplayData)treeListViewItem.Header;

            // Initialize the drag & drop operation
            var dragData = new DataObject("Name", displayData);
            DragDrop.DoDragDrop(treeListViewItem, dragData, DragDropEffects.Move);
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            OutputDataTemplate.Clear();

            var collection = new ObservableCollection<object>();
            new DisplayDataHierarchicalBuilder().Build(collection, OutputDataTemplate.Composite);

            OutputDataTemplateTreeListView.ItemsSource = collection;
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new OpenFileDialog { DefaultExt = ".xml", Filter = "eXtensible Markup Language File (.xml)|*.xml" };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result != true) return;

            OutputDataTemplate.Import(dlg.FileName);

            var collection = new ObservableCollection<object>();
            new DisplayDataHierarchicalBuilder().Build(collection, OutputDataTemplate.Composite);

            OutputDataTemplateTreeListView.ItemsSource = collection;
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "OutputDataTemplate",
                DefaultExt = ".xml",
                Filter = "eXtensible Markup Language File (.xml)|*.xml"
            };

            var result = dlg.ShowDialog();

            if (result != true) return;
            OutputDataTemplate.Export(dlg.FileName);
        }

        #endregion


        #region Auxiliaries

        private static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T) current;
                }
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);
            return null;
        }

        #endregion

    }
}
