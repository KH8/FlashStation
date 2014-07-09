using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using _ttAgent.DataAquisition;
using _ttAgent.PLC;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiCommunicationInterfaceOnline.xaml
    /// </summary>
    public partial class GuiCommunicationInterfaceOnline
    {
        private readonly PlcCommunicator _plcCommunication;
        private readonly CommunicationInterfaceHandler _communicationInterfaceHandler;

        private readonly ObservableCollection<DataDisplayer.DisplayData> _readInterfaceCollection = new ObservableCollection<DataDisplayer.DisplayData>();
        private readonly ObservableCollection<DataDisplayer.DisplayData> _writeInterfaceCollection = new ObservableCollection<DataDisplayer.DisplayData>();

        public ObservableCollection<DataDisplayer.DisplayData> ReadInterfaceCollection { get { return _readInterfaceCollection; } }
        public ObservableCollection<DataDisplayer.DisplayData> WriteInterfaceCollection { get { return _writeInterfaceCollection; } }

        private readonly Thread _updateThread;

        public GuiCommunicationInterfaceOnline(CommunicationInterfaceHandler communicationInterfaceHandler)
        {
            _communicationInterfaceHandler = communicationInterfaceHandler;
            _plcCommunication = _communicationInterfaceHandler.PlcCommunicator;

            InitializeComponent();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();

            if (ActualHeight > 0) { Height = ActualHeight; }
            if (ActualWidth > 0) { Width = ActualWidth; }

            CommunicationReadInterfaceListBox.ItemsSource = _readInterfaceCollection;
            CommunicationReadInterfaceListBox.View = CreateGridView();
            CommunicationReadInterfaceListBox.Foreground = Brushes.Black;

            CommunicationWriteInterfaceListBox.ItemsSource = _writeInterfaceCollection;
            CommunicationWriteInterfaceListBox.View = CreateGridView();
            CommunicationWriteInterfaceListBox.Foreground = Brushes.Black;
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

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
                if (_communicationInterfaceHandler.ReadInterfaceComposite != null && _communicationInterfaceHandler.WriteInterfaceComposite != null) DataDisplayer.Display(_readInterfaceCollection, _writeInterfaceCollection, _plcCommunication, _communicationInterfaceHandler);
                Thread.Sleep(1000);
            }
        }
    }
}
