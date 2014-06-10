using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ttAgent.Vector;
using GridView = System.Windows.Controls.GridView;
using TextBox = System.Windows.Controls.TextBox;

namespace ttAgent.Visual.Gui
{
    class GuiVFlashPathBank : Gui
    {
        private Grid _generalGrid;

        private ListView _vFlashBankListBox;
        private TextBox _typeNumberBox;
        private TextBox _typeVersionBox;

        private readonly VFlashTypeBank _vFlashTypeBank;
        private readonly VFlashTypeBankFile _vFlashTypeBankFile;

        private readonly ObservableCollection<VFlashDisplayProjectData> _vFlashProjectCollection = new ObservableCollection<VFlashDisplayProjectData>();

        public ObservableCollection<VFlashDisplayProjectData> VFlashProjectCollection
        { get { return _vFlashProjectCollection; } }

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiVFlashPathBank(uint id, VFlashTypeBankFile vFlashTypeBankFile, VFlashTypeBank vFlashTypeBank)
        {
            _vFlashTypeBankFile = vFlashTypeBankFile;
            _vFlashTypeBank = vFlashTypeBank;
            Id = id;

            VFlashTypeConverter.StringsToVFlashChannels(_vFlashTypeBankFile.TypeBank[Id], _vFlashTypeBank);
            UpdateVFlashProjectCollection();
        }

        public override void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = generalGrid;

            if (_generalGrid.ActualHeight > 0) { _generalGrid.Height = _generalGrid.ActualHeight; }
            if (_generalGrid.ActualWidth > 0) { _generalGrid.Width = _generalGrid.ActualWidth; }

            _generalGrid.Children.Add(_vFlashBankListBox = GuiFactory.CreateListView("VFlashBankListBox", 0, 0, HorizontalAlignment.Center, VerticalAlignment.Top, _generalGrid.Height - 30, _generalGrid.Width, VFlashProjectbankListViewSelection));

            _vFlashBankListBox.ItemsSource = _vFlashProjectCollection;
            _vFlashBankListBox.View = CreateGridView();
            _vFlashBankListBox.Foreground = Brushes.Black;

            _generalGrid.Children.Add(GuiFactory.CreateButton("VFlashCreateTypeButton", "Create Type", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom, 25, 100, TypeCreation));
            _generalGrid.Children.Add(GuiFactory.CreateLabel("Type Number:", 110, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom, 25, 90));
            _generalGrid.Children.Add(GuiFactory.CreateLabel("Version:", 260, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom, 25, 60));
            _generalGrid.Children.Add(_typeNumberBox = GuiFactory.CreateTextBox("TypeNumberBox", "1", 200, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom, HorizontalAlignment.Right, 23, 50));
            _generalGrid.Children.Add(_typeVersionBox = GuiFactory.CreateTextBox("TypeVersionBox", "-001", 320, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom, HorizontalAlignment.Right, 23, 70));
        }

        public void UpdateSizes(double height, double width)
        {
            _generalGrid.Height = height;
            _generalGrid.Width = width;

            _vFlashBankListBox.Height = height - 30;
            _vFlashBankListBox.Width = width;

            _vFlashBankListBox.View = CreateGridView();
        }

        public GridView CreateGridView()
        {
            var gridView = new GridView();

            gridView.Columns.Add(new GridViewColumn
            {
                Width = 60,
                Header = "Type",
                DisplayMemberBinding = new Binding("Type")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Width = 60,
                Header = "Version",
                DisplayMemberBinding = new Binding("Version")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Width = _generalGrid.Width - 130,
                Header = "Path",
                DisplayMemberBinding = new Binding("Path")
            });

            return gridView;
        }

        private void TypeCreation(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".vflashpack", 
                Filter = "Flash Path (.vflashpack)|*.vflashpack"
            };
            
            var result = dlg.ShowDialog();
            if (result == true)
            {
                _vFlashTypeBank.Add(new VFlashTypeComponent(Convert.ToUInt16(_typeNumberBox.Text), _typeVersionBox.Text, dlg.FileName));
                UpdateVFlashProjectCollection();
            }
        }

        private void UpdateVFlashProjectCollection()
        {
            _vFlashTypeBankFile.TypeBank[Id] = VFlashTypeConverter.VFlashTypesToStrings(_vFlashTypeBank.Children);
            _vFlashTypeBankFile.Save();

            _vFlashProjectCollection.Clear();
            foreach (var vFlashType in _vFlashTypeBank.Children)
            {
                var type = (VFlashTypeComponent)vFlashType;
                _vFlashProjectCollection.Add(new VFlashDisplayProjectData
                {
                    Type = type.Type.ToString(CultureInfo.InvariantCulture),
                    Version = type.Version,
                    Path = type.Path
                });
            }
        }

        private void VFlashProjectbankListViewSelection(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            var projectdata = (VFlashDisplayProjectData)listView.SelectedItem;
            if (projectdata != null) _typeNumberBox.Text = projectdata.Type;
            if (projectdata != null) _typeVersionBox.Text = projectdata.Version;
        }

        public override void MakeVisible()
        {
            _generalGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible()
        {
            _generalGrid.Visibility = Visibility.Hidden;
        }
    }
}
