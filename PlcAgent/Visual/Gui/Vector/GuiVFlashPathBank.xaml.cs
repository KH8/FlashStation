using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Win32;
using _PlcAgent.General;
using _PlcAgent.Vector;

namespace _PlcAgent.Visual.Gui.Vector
{
    /// <summary>
    /// Interaction logic for GuiVFlashPathBank.xaml
    /// </summary>
    public partial class GuiVFlashPathBank
    {
        #region Variables

        private readonly VFlashTypeBank _vFlashTypeBank;
        private readonly VFlashTypeBankFile _vFlashTypeBankFile;

        #endregion


        #region Creators

        public GuiVFlashPathBank(VFlashTypeBank vFlashTypeBank)
        {
            _vFlashTypeBank = vFlashTypeBank;
            _vFlashTypeBankFile = _vFlashTypeBank.VFlashTypeBankFile;

            InitializeComponent();

            VersionDataGrid.ItemsSource = _vFlashTypeBank.Children;
            VersionDataGrid.Foreground = Brushes.Black;

            SequenceDataGrid.Foreground = Brushes.Black;
        }

        #endregion


        #region Methods

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

        private void UpdateVFlashProjectCollection()
        {
            /*_vFlashTypeBankFile.TypeBank[_vFlashTypeBank.Header.Id] = VFlashTypeBank.VFlashTypeConverter.VFlashTypesToStrings(_vFlashTypeBank.Children);
            _vFlashTypeBankFile.Save();*/

            VersionDataGrid.Items.Refresh();
            SequenceDataGrid.Items.Refresh();
        }

        #endregion


        #region Event Handlers

        private void TypeCreation(object sender, RoutedEventArgs e)
        {
            var newtemplate = new VFlashTypeBank.VFlashTypeComponent(TypeVersionBox.Text);
            _vFlashTypeBank.Add(newtemplate);

            VersionDataGrid.SelectedItem = newtemplate;

            UpdateVFlashProjectCollection();
        }

        private void TypeDeletion(object sender, RoutedEventArgs e)
        {
            var template = (VFlashTypeBank.VFlashTypeComponent) VersionDataGrid.SelectedItem;
            if (template == null) return;

            _vFlashTypeBank.Remove(template);
            VersionDataGrid.SelectedItem = _vFlashTypeBank.Children.FirstOrDefault();

            UpdateVFlashProjectCollection();
        }

        private void VersionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SequenceDataGrid.ItemsSource = null;

            var gridView = (DataGrid) sender;
            var template = (VFlashTypeBank.VFlashTypeComponent) gridView.SelectedItem;
            if (template == null) return;

            TypeVersionBox.Text = template.Version;
            SequenceDataGrid.ItemsSource = template.Steps;

            UpdateVFlashProjectCollection();
        }

        private void StepCreation(object sender, RoutedEventArgs e)
        {
            var template = (VFlashTypeBank.VFlashTypeComponent)VersionDataGrid.SelectedItem;
            if (template == null) return;

            template.Steps.Add(new VFlashTypeBank.VFlashTypeComponent.Step(template.Steps.Count + 1));

            UpdateVFlashProjectCollection();
        }

        private void StepDeletion(object sender, RoutedEventArgs e)
        {
            var step = (VFlashTypeBank.VFlashTypeComponent.Step)SequenceDataGrid.SelectedItem;
            if (step == null) return;

            var template = (VFlashTypeBank.VFlashTypeComponent)VersionDataGrid.SelectedItem;
            if (template == null) return;

            template.Steps.Remove(step);

            foreach (var residualStep in template.Steps.Where(residualStep => residualStep.Id >= step.Id))
            {
                residualStep.Id --;
            }

            SequenceDataGrid.SelectedItem = template.Steps.FirstOrDefault();

            UpdateVFlashProjectCollection();
        }

        private void PathModyfication(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBlock = (TextBlock) sender;

            var dlg = new OpenFileDialog
            {
                DefaultExt = ".vflashpack",
                Filter = "Flash Path (.vflashpack)|*.vflashpack"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;

            var step = (VFlashTypeBank.VFlashTypeComponent.Step)textBlock.DataContext;
            step.Path = dlg.FileName;

            UpdateVFlashProjectCollection();
        }

        #endregion

    }
}
