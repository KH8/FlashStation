using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using _PlcAgent.Log;
using _PlcAgent.Output;
using _PlcAgent.Output.OutputFileCreator;
using _PlcAgent.Visual.TreeListView;
using ComboBox = System.Windows.Controls.ComboBox;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace _PlcAgent.Visual.Gui.Output
{
    /// <summary>
    /// Interaction logic for GuiOutputFileCreator.xaml
    /// </summary>
    public partial class GuiOutputFileCreator
    {
        private readonly Boolean _save;

        public GuiOutputFileCreator(OutputFileCreator outputFileCreator)
            : base(outputFileCreator)
        {
            InitializeComponent();

            FileNameSuffixBox.Text = OutputFileCreator.OutputFileCreatorFile.FileNameSuffixes[OutputFileCreator.Header.Id];
            DirectoryPathBox.Text = OutputFileCreator.OutputFileCreatorFile.DirectoryPaths[OutputFileCreator.Header.Id];

            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xml", Content = "*.xml" });
            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Csv", Content = "*.csv" });
            OutputTypeComboBox.SelectedIndex = OutputFileCreator.OutputFileCreatorFile.SelectedIndex[OutputFileCreator.Header.Id];

            OutputFileCreator.FileCreator = FileCreatorFactory.CreateVariable(OutputTypeComboBox.SelectedItem.ToString());

            HeaderGroupBox.Header = "Output File Creator " + OutputFileCreator.Header.Id;
            _save = true;
        }

        private void FileNameSuffixChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { OutputFileCreator.OutputFileCreatorFile.FileNameSuffixes[OutputFileCreator.Header.Id] = box.Text; }
            catch (Exception) { OutputFileCreator.OutputFileCreatorFile.FileNameSuffixes[OutputFileCreator.Header.Id] = "noName"; }
            OutputFileCreator.OutputFileCreatorFile.Save();
        }

        private void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!_save) return; 
            var outputTypeComboBox = (ComboBox)sender;
            OutputFileCreator.OutputFileCreatorFile.SelectedIndex[OutputFileCreator.Header.Id] = outputTypeComboBox.SelectedIndex;
            OutputFileCreator.FileCreator = FileCreatorFactory.CreateVariable(OutputTypeComboBox.SelectedItem.ToString());
            OutputFileCreator.OutputFileCreatorFile.Save();
        }

        private void CreateOutput(object sender, RoutedEventArgs e)
        {
            Logger.Log("ID: " + OutputFileCreator.Header.Id + " : Output file creation requested by the user");
            OutputFileCreator.CreateOutput();
        }

        private void SetDirectoryPath(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog {SelectedPath = @"C:\"};
            var result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                DirectoryPathBox.Text = folderDialog.SelectedPath;

            OutputFileCreator.OutputFileCreatorFile.DirectoryPaths[OutputFileCreator.Header.Id] = DirectoryPathBox.Text;
            OutputFileCreator.OutputFileCreatorFile.Save();
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
