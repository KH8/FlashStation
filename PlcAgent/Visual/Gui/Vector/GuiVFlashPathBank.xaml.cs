using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using _PlcAgent.General;
using _PlcAgent.Vector;
using DataGrid = System.Windows.Controls.DataGrid;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace _PlcAgent.Visual.Gui.Vector
{
    /// <summary>
    /// Interaction logic for GuiVFlashPathBank.xaml
    /// </summary>
    public partial class GuiVFlashPathBank
    {
        #region Variables

        private readonly Boolean _save;

        private DateTime _clickTimeStamp;
        private readonly VFlashTypeBank _vFlashTypeBank;

        #endregion


        #region Creators

        public GuiVFlashPathBank(VFlashTypeBank vFlashTypeBank)
        {
            _vFlashTypeBank = vFlashTypeBank;

            InitializeComponent();

            VersionDataGrid.ItemsSource = _vFlashTypeBank.Children;
            VersionDataGrid.Foreground = Brushes.Black;

            SequenceDataGrid.Foreground = Brushes.Black;

            _save = true;
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
            if (!_save) return;
            _vFlashTypeBank.Update();

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
            if ((DateTime.Now - _clickTimeStamp).TotalMilliseconds > SystemInformation.DoubleClickTime)
            {
                _clickTimeStamp = DateTime.Now;
                return;
            }

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

        private void ConditionsModyfication(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((DateTime.Now - _clickTimeStamp).TotalMilliseconds > SystemInformation.DoubleClickTime)
            {
                _clickTimeStamp = DateTime.Now;
                return;
            }

            var textBlock = (TextBlock)sender;
            var step = (VFlashTypeBank.VFlashTypeComponent.Step) textBlock.DataContext;

            var window = new GuiVFlashPathBankTransitionConditions(step.TransitionConditions, _vFlashTypeBank.Update) {Topmost = true};
            window.Show();
        }

        private void TextChanged(object sender, RoutedEventArgs e)
        {
            if (!_save) return;
            _vFlashTypeBank.Update();
        }

        #endregion

    }
}
