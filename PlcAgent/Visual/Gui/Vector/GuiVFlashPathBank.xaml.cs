using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using _PlcAgent.General;
using _PlcAgent.Vector;

namespace _PlcAgent.Visual.Gui.Vector
{
    /// <summary>
    /// Interaction logic for GuiVFlashPathBank.xaml
    /// </summary>
    public partial class GuiVFlashPathBank
    {
        private readonly VFlashTypeBank _vFlashTypeBank;
        private readonly VFlashTypeBankFile _vFlashTypeBankFile;

        public GuiVFlashPathBank(VFlashTypeBank vFlashTypeBank)
        {
            _vFlashTypeBank = vFlashTypeBank;
            _vFlashTypeBankFile = _vFlashTypeBank.VFlashTypeBankFile;

            InitializeComponent();

            VersionDataGrid.ItemsSource = _vFlashTypeBank.Children;
            VersionDataGrid.Foreground = Brushes.Black;

            SequenceDataGrid.Foreground = Brushes.Black;
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            VersionDataGrid.Height = height - 27;
            VersionDataGrid.Width = 400;
            SequenceDataGrid.Height = height - 27;
            SequenceDataGrid.Width = Limiter.DoubleLimit(width - VersionDataGrid.Width - 4, 0);
        }

        private void TypeCreation(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".vflashpack",
                Filter = "Flash Path (.vflashpack)|*.vflashpack"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            _vFlashTypeBank.Add(new VFlashTypeBank.VFlashTypeComponent(TypeVersionBox.Text));

            UpdateVFlashProjectCollection();
        }

        private void UpdateVFlashProjectCollection()
        {
            /*_vFlashTypeBankFile.TypeBank[_vFlashTypeBank.Header.Id] = VFlashTypeBank.VFlashTypeConverter.VFlashTypesToStrings(_vFlashTypeBank.Children);
            _vFlashTypeBankFile.Save();*/

            VersionDataGrid.Items.Refresh();
            SequenceDataGrid.Items.Refresh();
        }

        private void VersionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gridView = (DataGrid)sender;
            var template = (VFlashTypeBank.VFlashTypeComponent)gridView.SelectedItem;
            if (template == null) return;

            TypeVersionBox.Text = template.Version;
            SequenceDataGrid.ItemsSource = template.Steps;

            UpdateVFlashProjectCollection();
        }
    }
}
