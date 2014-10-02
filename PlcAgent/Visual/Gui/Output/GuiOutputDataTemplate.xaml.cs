using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using _PlcAgent.General;
using _PlcAgent.Log;
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
        private readonly OutputDataTemplateComposite _outputDataTemplateComposite;

        #endregion


        #region Properties

        public TabItem TabItem = new TabItem();

        #endregion


        #region Constructors

        public GuiOutputDataTemplate(OutputDataTemplateComposite template)
        {
            InitializeComponent();

            _outputDataTemplateComposite = template;

            var collection = new ObservableCollection<object>();
            new DisplayDataHierarchicalBuilder().Build(collection, _outputDataTemplateComposite);

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
            OutputDataTemplateTreeListView.Width = Limiter.DoubleLimit((width / 3) - 2, 0);

            FooterGrid.Width = Limiter.DoubleLimit((width / 3) - 2, 0);
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
            _outputDataTemplateComposite.Clear();

            var collection = new ObservableCollection<object>();
            new DisplayDataHierarchicalBuilder().Build(collection, _outputDataTemplateComposite);

            OutputDataTemplateTreeListView.ItemsSource = collection;
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            _outputDataTemplateComposite.Clear();

            //todo import logic

            var collection = new ObservableCollection<object>();
            new DisplayDataHierarchicalBuilder().Build(collection, _outputDataTemplateComposite);

            OutputDataTemplateTreeListView.ItemsSource = collection;
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "OutputDataTemplate",
                DefaultExt = ".xml",
                Filter = "Extensible Markup Language File (.xml)|*.xml"
            };

            var result = dlg.ShowDialog();

            if (result != true) return;
            Logger.Log("Exporting Output Data Template to file: " + dlg.FileName);
            _outputDataTemplateComposite.Export(dlg.FileName);
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
