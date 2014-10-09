using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using _PlcAgent.Log;
using _PlcAgent.Output;
using _PlcAgent.Output.OutputHandler;
using _PlcAgent.Visual.TreeListView;
using ComboBox = System.Windows.Controls.ComboBox;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace _PlcAgent.Visual.Gui.Output
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiOutputHandler
    {
        private readonly Boolean _save;

        public GuiOutputHandler(OutputHandler outputHandler)
            : base(outputHandler)
        {
            InitializeComponent();

            FileNameSuffixBox.Text = OutputHandler.OutputHandlerFile.FileNameSuffixes[OutputHandler.Header.Id];
            StartPositionBox.Text = OutputHandler.OutputHandlerFile.StartAddress[OutputHandler.Header.Id].ToString(CultureInfo.InvariantCulture);
            EndPositionBox.Text = OutputHandler.OutputHandlerFile.EndAddress[OutputHandler.Header.Id].ToString(CultureInfo.InvariantCulture);
            DirectoryPathBox.Text = OutputHandler.OutputHandlerFile.DirectoryPaths[OutputHandler.Header.Id];

            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xml", Content = "*.xml" });
            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Csv", Content = "*.csv" });
            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xls", Content = "*.xls" });
            OutputTypeComboBox.SelectedIndex = OutputHandler.OutputHandlerFile.SelectedIndex[OutputHandler.Header.Id];

            OutputHandler.OutputWriter = OutputWriterFactory.CreateVariable(OutputTypeComboBox.SelectedItem.ToString());

            HeaderGroupBox.Header = "Output Handler " + OutputHandler.Header.Id;
            _save = true;
        }

        private void FileNameSuffixChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { OutputHandler.OutputHandlerFile.FileNameSuffixes[OutputHandler.Header.Id] = box.Text; }
            catch (Exception) { OutputHandler.OutputHandlerFile.FileNameSuffixes[OutputHandler.Header.Id] = "noName"; }
            OutputHandler.OutputHandlerFile.Save();
        }

        private void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!_save) return; 
            var outputTypeComboBox = (ComboBox)sender;
            OutputHandler.OutputHandlerFile.SelectedIndex[OutputHandler.Header.Id] = outputTypeComboBox.SelectedIndex;
            OutputHandler.OutputWriter = OutputWriterFactory.CreateVariable(OutputTypeComboBox.SelectedItem.ToString());
            OutputHandler.OutputHandlerFile.Save();
        }

        private void StartPositionChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { OutputHandler.OutputHandlerFile.StartAddress[OutputHandler.Header.Id] = Convert.ToInt32(box.Text); }
            catch (Exception) { OutputHandler.OutputHandlerFile.StartAddress[OutputHandler.Header.Id] = 0; }
            OutputHandler.OutputHandlerFile.Save();
        }

        private void EndPositionBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { OutputHandler.OutputHandlerFile.EndAddress[OutputHandler.Header.Id] = Convert.ToInt32(box.Text); }
            catch (Exception) { OutputHandler.OutputHandlerFile.EndAddress[OutputHandler.Header.Id] = 0; }
            OutputHandler.OutputHandlerFile.Save();
        }

        private void CreateOutput(object sender, RoutedEventArgs e)
        {
            Logger.Log("ID: " + OutputHandler.Header.Id + " : Output file creation requested by the user");
            OutputHandler.CreateOutput();
        }

        private void SetDirectoryPath(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog {SelectedPath = @"C:\"};
            var result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                DirectoryPathBox.Text = folderDialog.SelectedPath;

            OutputHandler.OutputHandlerFile.DirectoryPaths[OutputHandler.Header.Id] = DirectoryPathBox.Text;
            OutputHandler.OutputHandlerFile.Save();
        }

        private void TextBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = DragDropEffects.Move;
            if (!e.Data.GetDataPresent("Name")) e.Effects = DragDropEffects.None;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("Name")) return;
            var displayData = e.Data.GetData("Name") as DisplayDataBuilder.DisplayData;

            if (displayData != null) FileNameSuffixBox.Text = "%" + displayData.Name;
        }
    }
}
