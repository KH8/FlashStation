using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using _ttAgent.DataAquisition;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiCommunicationInterfaceOnline.xaml
    /// </summary>
    public partial class GuiCommunicationInterfaceOnline
    {
        private readonly CommunicationInterfaceHandler _communicationInterfaceHandler;

        private Boolean _isActive;

        private readonly Thread _updateThread;

        public TabItem TabItem = new TabItem();

        public GuiCommunicationInterfaceOnline(CommunicationInterfaceHandler communicationInterfaceHandler)
        {
            _communicationInterfaceHandler = communicationInterfaceHandler;

            InitializeComponent();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();

            CommunicationReadInterfaceListBox.View = CreateGridView();
            CommunicationReadInterfaceListBox.ItemsSource = _communicationInterfaceHandler.ReadInterfaceCollection;
            CommunicationReadInterfaceListBox.Foreground = Brushes.Black;

            CommunicationWriteInterfaceListBox.View = CreateGridView();
            CommunicationWriteInterfaceListBox.ItemsSource = _communicationInterfaceHandler.WriteInterfaceCollection;
            CommunicationWriteInterfaceListBox.Foreground = Brushes.Black;
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            CommunicationReadInterfaceListBox.Height = height;
            CommunicationReadInterfaceListBox.Width = (width / 2) - 2;
            CommunicationWriteInterfaceListBox.Height = height;
            CommunicationWriteInterfaceListBox.Width = (width / 2) - 2;

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
                Width = (Width / 2) - 280,
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

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                if (_communicationInterfaceHandler.ReadInterfaceComposite != null &&
                    _communicationInterfaceHandler.WriteInterfaceComposite != null && _isActive)
                {
                    _communicationInterfaceHandler.UpdateObservableCollections();
                }
                Thread.Sleep(10);
            }
        }

        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (TabControl) sender;
            _isActive = Equals(tab.SelectedItem, TabItem);
        }
    }
}
