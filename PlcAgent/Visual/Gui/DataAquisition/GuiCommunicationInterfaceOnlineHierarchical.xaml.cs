﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Visual.Interfaces;
using _PlcAgent.Visual.TreeListView;

namespace _PlcAgent.Visual.Gui.DataAquisition
{
    /// <summary>
    /// Interaction logic for GuiCommunicationInterfaceOnlineHierarchical.xaml
    /// </summary>
    public partial class GuiCommunicationInterfaceOnlineHierarchical : IResizableGui
    {
        #region Variables

        private Point _storedPosition;

        #endregion


        #region Properties

        public TabItem TabItem = new TabItem();

        #endregion


        #region Constructors

        public GuiCommunicationInterfaceOnlineHierarchical(CommunicationInterfaceHandler communicationInterfaceHandler)
            : base(communicationInterfaceHandler)
        {
            InitializeComponent();

            CommunicationInterfaceHandler.OnInterfaceUpdatedDelegate += OnInterfaceUpdatedDelegate;
            
            CommunicationReadInterfaceTreeListView.ItemsSource = CommunicationInterfaceHandler.ReadInterfaceCollection;
            CommunicationWriteInterfaceTreeListView.ItemsSource = CommunicationInterfaceHandler.WriteInterfaceCollection;

        }

        #endregion


        #region Methods

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            CommunicationReadInterfaceTreeListView.Height = height;
            CommunicationReadInterfaceTreeListView.Width = Limiter.DoubleLimit((width / 2) - 2, 0);
            CommunicationWriteInterfaceTreeListView.Height = height;
            CommunicationWriteInterfaceTreeListView.Width = Limiter.DoubleLimit((width / 2) - 2, 0);
        }

        #endregion


        #region Event Handlers

        public void OnInterfaceUpdatedDelegate()
        {
            CommunicationReadInterfaceTreeListView.ItemsSource = null;
            CommunicationReadInterfaceTreeListView.ItemsSource = CommunicationInterfaceHandler.ReadInterfaceCollection;
            CommunicationWriteInterfaceTreeListView.ItemsSource = null;
            CommunicationWriteInterfaceTreeListView.ItemsSource = CommunicationInterfaceHandler.WriteInterfaceCollection;
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
