using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using _3880_80_FlashStation.Vector;
using GridView = System.Windows.Controls.GridView;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiVFlashPathBank : Gui
    {
        private Grid _generalGrid;

        private readonly ObservableCollection<VFlashDisplayProjectData> _vFlashProjectCollection = new ObservableCollection<VFlashDisplayProjectData>();

        public ObservableCollection<VFlashDisplayProjectData> VFlashProjectCollection
        { get { return _vFlashProjectCollection; } }

        private readonly Thread _updateThread;

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiVFlashPathBank(uint id)
        {
            Id = id;

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();
        }

        public override void Initialize(int xPosition, int yPosition)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Center, VerticalAlignment.Top, 240, 800);

            var list = new ListView();
            var gridView = new GridView();

            _generalGrid.Children.Add(list);

            list.ItemsSource = _vFlashProjectCollection;
            list.View = gridView;
            gridView.Columns.Add(new GridViewColumn()
            {
                Width = 60,
                Header = "Type",
                DisplayMemberBinding = new Binding("Type")
            });
            gridView.Columns.Add(new GridViewColumn()
            {
                Width = 60,
                Header = "Version",
                DisplayMemberBinding = new Binding("Version")
            });
            gridView.Columns.Add(new GridViewColumn()
            {
                Width = 540,
                Header = "Path",
                DisplayMemberBinding = new Binding("Path")
            });

            _vFlashProjectCollection.Add(new VFlashDisplayProjectData()
            {
                Path = "asdasd",
                Type = "asdasdasdasdds",
                Version = "123"
            });
            _vFlashProjectCollection.Add(new VFlashDisplayProjectData()
            {
                Path = "asdasd2",
                Type = "asdasdasdasdds2",
                Version = "1233"
            });
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
               Thread.Sleep(21);
            }
        }
    }
}
