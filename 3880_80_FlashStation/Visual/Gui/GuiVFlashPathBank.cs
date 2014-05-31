using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using _3880_80_FlashStation.Vector;
using GridView = System.Windows.Controls.GridView;
using TextBox = System.Windows.Controls.TextBox;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiVFlashPathBank : Gui
    {
        private Grid _generalGrid;

        private ListView _vFlashBankListBox;
        private TextBox _typeNumberBox;
        private TextBox _typeVersionBox;

        private readonly VFlashTypeBank _vFlashTypeBank;

        private readonly ObservableCollection<VFlashDisplayProjectData> _vFlashProjectCollection = new ObservableCollection<VFlashDisplayProjectData>();

        public ObservableCollection<VFlashDisplayProjectData> VFlashProjectCollection
        { get { return _vFlashProjectCollection; } }

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiVFlashPathBank(uint id)
        {
            Id = id;

            _vFlashTypeBank = new VFlashTypeBank();
            VFlashTypeConverter.StringsToVFlashChannels(VFlashTypeBankFile.Default.TypeBank, _vFlashTypeBank);
            UpdateVFlashProjectCollection();
        }

        public override void Initialize(int xPosition, int yPosition)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Center, VerticalAlignment.Top, 240, 800);

            _generalGrid.Children.Add(_vFlashBankListBox = GuiFactory.CreateListView("VFlashBankListBox", 0, 0, HorizontalAlignment.Center, VerticalAlignment.Top, 210, 800, VFlashProjectbankListViewSelection));
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
                Width = 540,
                Header = "Path",
                DisplayMemberBinding = new Binding("Path")
            });

            _vFlashBankListBox.ItemsSource = _vFlashProjectCollection;
            _vFlashBankListBox.View = gridView;
            _vFlashBankListBox.Foreground = Brushes.Black;

            _generalGrid.Children.Add(GuiFactory.CreateButton("VFlashCreateTypeButton", "Create Type", 0, 0, HorizontalAlignment.Right, VerticalAlignment.Bottom, 25, 100, TypeCreation));
            _generalGrid.Children.Add(GuiFactory.CreateLabel("Type Number:" ,431 , 0,HorizontalAlignment.Left ,VerticalAlignment.Bottom, 25, 94));
            _generalGrid.Children.Add(GuiFactory.CreateLabel("Version:", 573, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom, 25, 55));
            _generalGrid.Children.Add(_typeNumberBox = GuiFactory.CreateTextBox("TypeNumberBox", "1", 522, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom, HorizontalAlignment.Right, 23, 42));
            _generalGrid.Children.Add(_typeVersionBox = GuiFactory.CreateTextBox("TypeVersionBox", "-001", 628, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom, HorizontalAlignment.Right, 23, 66));
        }

        private void TypeCreation(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".vflashpack", Filter = "Flash Path (.vflashpack)|*.vflashpack" };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                _vFlashTypeBank.Add(new VFlashTypeComponent(Convert.ToUInt16(_typeNumberBox.Text), _typeVersionBox.Text, dlg.FileName));
                UpdateVFlashProjectCollection();
            }
        }

        private void UpdateVFlashProjectCollection()
        {
            VFlashTypeBankFile.Default.TypeBank = VFlashTypeConverter.VFlashTypesToStrings(_vFlashTypeBank.Children);
            VFlashTypeBankFile.Default.Save();

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
