using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.PLC;
using GridView = System.Windows.Controls.GridView;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiCommunicationInterfaceOnline : Gui
    {
        private Grid _generalGrid;

        private ListView _communicationReadInterfaceListBox;
        private ListView _communicationWriteInterfaceListBox;

        private readonly PlcCommunicator _plcCommunication;
        private readonly CommunicationInterfaceHandler _communicationInterfaceHandler;

        private readonly ObservableCollection<DataDisplayer.DisplayData> _readInterfaceCollection = new ObservableCollection<DataDisplayer.DisplayData>();
        private readonly ObservableCollection<DataDisplayer.DisplayData> _writeInterfaceCollection = new ObservableCollection<DataDisplayer.DisplayData>();

        public ObservableCollection<DataDisplayer.DisplayData> ReadInterfaceCollection { get { return _readInterfaceCollection; } }
        public ObservableCollection<DataDisplayer.DisplayData> WriteInterfaceCollection { get { return _writeInterfaceCollection; } }

        private readonly Thread _updateThread;

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiCommunicationInterfaceOnline(uint id, PlcCommunicator plcCommunication, CommunicationInterfaceHandler communicationInterfaceHandler)
        {
            Id = id;
            _plcCommunication = plcCommunication;
            _communicationInterfaceHandler = communicationInterfaceHandler;
            
            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }

        public override void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = generalGrid;

            if (_generalGrid.ActualHeight > 0) { _generalGrid.Height = _generalGrid.ActualHeight; }
            if (_generalGrid.ActualWidth > 0) { _generalGrid.Width = _generalGrid.ActualWidth; }

            _generalGrid.Children.Add(_communicationReadInterfaceListBox = GuiFactory.CreateListView("OnlineReadDataListBox", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, _generalGrid.Height, (_generalGrid.Width/2) - 2));
            _generalGrid.Children.Add(_communicationWriteInterfaceListBox = GuiFactory.CreateListView("OnlineWriteDataListBox", 0, 0, HorizontalAlignment.Right, VerticalAlignment.Top, _generalGrid.Height, (_generalGrid.Width/2) - 2));

            _communicationReadInterfaceListBox.ItemsSource = _readInterfaceCollection;
            _communicationReadInterfaceListBox.View = CreateGridView();
            _communicationReadInterfaceListBox.Foreground = Brushes.Black;

            _communicationWriteInterfaceListBox.ItemsSource = _writeInterfaceCollection;
            _communicationWriteInterfaceListBox.View = CreateGridView();
            _communicationWriteInterfaceListBox.Foreground = Brushes.Black;
        }

        public void UpdateSizes(double height, double width)
        {
            _generalGrid.Height = height;
            _generalGrid.Width = width;

            _communicationReadInterfaceListBox.Height = height;
            _communicationReadInterfaceListBox.Width = (width / 2) - 2;
            _communicationWriteInterfaceListBox.Height = height;
            _communicationWriteInterfaceListBox.Width = (width / 2) - 2;

            _communicationReadInterfaceListBox.View = CreateGridView();
            _communicationWriteInterfaceListBox.View = CreateGridView();
        }

        private GridView CreateGridView()
        {
            var gridView = new GridView();

            gridView.Columns.Add(new GridViewColumn
            {
                Width = 70,
                Header = "Addr.",
                DisplayMemberBinding = new Binding("Address")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Width = (_generalGrid.Width / 2 ) - 210,
                Header = "Name",
                DisplayMemberBinding = new Binding("Name")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Width = 50,
                Header = "Type",
                DisplayMemberBinding = new Binding("Type")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Width = 50,
                Header = "Value",
                DisplayMemberBinding = new Binding("Value")
            });

            return gridView;
        }

        public override void MakeVisible()
        {
            _generalGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible()
        {
            _generalGrid.Visibility = Visibility.Hidden;
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                if (_communicationInterfaceHandler.ReadInterfaceComposite != null && _communicationInterfaceHandler.WriteInterfaceComposite != null) DataDisplayer.Display(_readInterfaceCollection, _writeInterfaceCollection, _plcCommunication, _communicationInterfaceHandler);
                Thread.Sleep(21);
            }
        }
    }
}
