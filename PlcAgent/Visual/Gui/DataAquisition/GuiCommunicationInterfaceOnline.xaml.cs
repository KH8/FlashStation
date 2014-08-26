using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Visual.Interfaces;

namespace _PlcAgent.Visual.Gui.DataAquisition
{
    /// <summary>
    /// Interaction logic for GuiCommunicationInterfaceOnline.xaml
    /// </summary>
    public partial class GuiCommunicationInterfaceOnline : IResizableGui
    {
        #region Variables

        private Boolean _isActive;

        #endregion


        #region Properties

        public TabItem TabItem = new TabItem();

        #endregion


        #region Constructors

        public GuiCommunicationInterfaceOnline(CommunicationInterfaceHandler communicationInterfaceHandler)
            : base(communicationInterfaceHandler)
        {
            InitializeComponent();

            CommunicationInterfaceHandler.OnInterfaceUpdatedDelegate += OnInterfaceUpdatedDelegate;

            CommunicationReadInterfaceListBox.View = CreateGridView();
            CommunicationReadInterfaceListBox.ItemsSource = CommunicationInterfaceHandler.ReadInterfaceCollection;
            CommunicationReadInterfaceListBox.Foreground = Brushes.Black;

            CommunicationWriteInterfaceListBox.View = CreateGridView();
            CommunicationWriteInterfaceListBox.ItemsSource = CommunicationInterfaceHandler.WriteInterfaceCollection;
            CommunicationWriteInterfaceListBox.Foreground = Brushes.Black;
        }

        #endregion


        #region Methods

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            CommunicationReadInterfaceListBox.Height = height;
            CommunicationReadInterfaceListBox.Width = Limiter.DoubleLimit((width/2) - 2, 0);
            CommunicationWriteInterfaceListBox.Height = height;
            CommunicationWriteInterfaceListBox.Width = Limiter.DoubleLimit((width/2) - 2, 0);

            CommunicationReadInterfaceListBox.View = CreateGridView();
            CommunicationWriteInterfaceListBox.View = CreateGridView();
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
            if (CommunicationInterfaceHandler.ReadInterfaceComposite == null || CommunicationInterfaceHandler.WriteInterfaceComposite == null || !_isActive) return;

            CommunicationInterfaceHandler.UpdateObservableCollections();
            CommunicationReadInterfaceListBox.Dispatcher.BeginInvoke((new Action(() => CommunicationReadInterfaceListBox.Items.Refresh())));
            CommunicationWriteInterfaceListBox.Dispatcher.BeginInvoke((new Action(() => CommunicationWriteInterfaceListBox.Items.Refresh())));
        }

        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (TabControl) sender;
            _isActive = Equals(tab.SelectedItem, TabItem);
        }

        #endregion

    }
}
