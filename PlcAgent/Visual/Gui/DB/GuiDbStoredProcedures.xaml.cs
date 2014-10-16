using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Visual.Interfaces;
using _PlcAgent.Visual.TreeListView;

namespace _PlcAgent.Visual.Gui.DB
{
    /// <summary>
    /// Interaction logic for GuiCommunicationInterfaceOnline.xaml
    /// </summary>
    public partial class GuiDbStoredProcedures : IResizableGui
    {
        #region Variables

        private Point _storedPosition;

        #endregion


        #region Properties

        public TabItem TabItem = new TabItem();

        #endregion


        #region Constructors

        public GuiDbStoredProcedures(CommunicationInterfaceHandler communicationInterfaceHandler)
            : base(communicationInterfaceHandler)
        {
            InitializeComponent();

            CommunicationInterfaceHandler.OnInterfaceUpdatedDelegate += OnInterfaceUpdatedDelegate;

            StoredProcedureListBox.View = CreateGridView();
            StoredProcedureListBox.ItemsSource = CommunicationInterfaceHandler.ReadInterfaceCollection;
            StoredProcedureListBox.Foreground = Brushes.Black;

            ParameterListBox.View = CreateGridView();
            ParameterListBox.ItemsSource = CommunicationInterfaceHandler.WriteInterfaceCollection;
            ParameterListBox.Foreground = Brushes.Black;

        }

        #endregion


        #region Methods

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            StoredProcedureListBox.Height = height;
            StoredProcedureListBox.Width = Limiter.DoubleLimit((width / 2) - 2, 0);
            ParameterListBox.Height = height;
            ParameterListBox.Width = Limiter.DoubleLimit((width / 2) - 2, 0);

            StoredProcedureListBox.View = CreateGridView();
            ParameterListBox.View = CreateGridView();
        }

        private GridView CreateGridView()
        {
            var gridView = new GridView();

            gridView.Columns.Add(new GridViewColumn
            {
                Width = 80,
                Header = "Addr.",
                DisplayMemberBinding = new Binding("Address")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Width = Limiter.DoubleLimit((Width/2) - 280, 0),
                Header = "Name",
                DisplayMemberBinding = new Binding("Name")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Width = 80,
                Header = "Type",
                DisplayMemberBinding = new Binding("Type")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Width = 80,
                Header = "Value",
                DisplayMemberBinding = new Binding("Value")
            });

            return gridView;
        }

        #endregion


        #region Event Handlers

        public void OnInterfaceUpdatedDelegate()
        {
            StoredProcedureListBox.ItemsSource = null;
            StoredProcedureListBox.ItemsSource = CommunicationInterfaceHandler.ReadInterfaceCollection;
            ParameterListBox.ItemsSource = null;
            ParameterListBox.ItemsSource = CommunicationInterfaceHandler.WriteInterfaceCollection;
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
            // Get the dragged ListViewItem
            var listView = sender as ListView;
            var listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

            // Find the data behind the ListViewItem
            if (listViewItem == null || listView == null) return;
            var displayData = (DisplayDataBuilder.DisplayData)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

            // Initialize the drag & drop operation
            var dragData = new DataObject("Name", displayData);
            DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
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
