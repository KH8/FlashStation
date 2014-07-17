using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.Output;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiOutputHandler
    {
        private readonly Boolean _save;
        private readonly OutputHandler _outputHandler;
        private readonly OutputHandlerFile _outputHandlerFile;

        public GuiOutputHandler(Module module)
        {
            _outputHandler = (OutputHandler)module;
            _outputHandlerFile = _outputHandler.OutputHandlerFile;

            InitializeComponent();
            FileNameSuffixBox.Text = _outputHandlerFile.FileNameSuffixes[_outputHandler.Header.Id];
            StartPositionBox.Text = _outputHandlerFile.StartAddress[_outputHandler.Header.Id].ToString(CultureInfo.InvariantCulture);
            EndPositionBox.Text = _outputHandlerFile.EndAddress[_outputHandler.Header.Id].ToString(CultureInfo.InvariantCulture);
            DirectoryPathBox.Text = _outputHandlerFile.DirectoryPaths[_outputHandler.Header.Id];

            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xml", Content = "*.xml" });
            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Csv", Content = "*.csv" });
            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xls", Content = "*.xls" });
            OutputTypeComboBox.SelectedIndex = _outputHandlerFile.SelectedIndex[_outputHandler.Header.Id];

            _outputHandler.OutputWriter = OutputWriterFactory.CreateVariable(OutputTypeComboBox.SelectedItem.ToString());

            HeaderGroupBox.Header = "Output Handler " + _outputHandler.Header.Id;
            _save = true;
        }

        private void FileNameSuffixChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { _outputHandlerFile.FileNameSuffixes[_outputHandler.Header.Id] = box.Text; }
            catch (Exception) { _outputHandlerFile.FileNameSuffixes[_outputHandler.Header.Id] = "noName"; }
            _outputHandlerFile.Save();
        }

        private void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!_save) return; 
            var outputTypeComboBox = (ComboBox)sender;
            _outputHandlerFile.SelectedIndex[_outputHandler.Header.Id] = outputTypeComboBox.SelectedIndex;
            _outputHandler.OutputWriter = OutputWriterFactory.CreateVariable(OutputTypeComboBox.SelectedItem.ToString());
            _outputHandlerFile.Save();
        }

        private void StartPositionChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { _outputHandlerFile.StartAddress[_outputHandler.Header.Id] = Convert.ToInt32(box.Text); }
            catch (Exception) { _outputHandlerFile.StartAddress[_outputHandler.Header.Id] = 0; }
            _outputHandlerFile.Save();
        }

        private void EndPositionBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { _outputHandlerFile.EndAddress[_outputHandler.Header.Id] = Convert.ToInt32(box.Text); }
            catch (Exception) { _outputHandlerFile.EndAddress[_outputHandler.Header.Id] = 0; }
            _outputHandlerFile.Save();
        }

        private void CreateOutput(object sender, RoutedEventArgs e)
        {
            Logger.Log("ID: " + _outputHandler.Header.Id + " : Output file creation requested by the user");
            _outputHandler.CreateOutput();
        }

        private void SetDirectoryPath(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog {SelectedPath = @"C:\"};
            var result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                DirectoryPathBox.Text = folderDialog.SelectedPath;

            _outputHandlerFile.DirectoryPaths[_outputHandler.Header.Id] = DirectoryPathBox.Text;
            _outputHandlerFile.Save();
        }
    }
}
