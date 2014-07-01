using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.Log;
using _ttAgent.MainRegistry;
using _ttAgent.Output;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiOutputHandler
    {
        private int _xPosition;
        private int _yPosition;
        private Grid _generalGridMemory = new Grid();

        private Boolean _save;

        private readonly OutputHandler _outputHandler;
        private readonly OutputHandlerFile _outputHandlerFile;

        public int XPosition
        {
            get { return _xPosition; }
            set { _xPosition = value; }
        }

        public int YPosition
        {
            get { return _yPosition; }
            set { _yPosition = value; }
        }

        public RegistryComponent.RegistryComponentHeader Header;

        public GuiOutputHandler(uint id, string name, OutputHandler outputHandler)
        {
            Header = new RegistryComponent.RegistryComponentHeader
            {
                Id = id,
                Name = name
            };

            _outputHandler = outputHandler;
            _outputHandlerFile = _outputHandler.OutputHandlerFile;
        }

        public void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            InitializeComponent();
            GeneralGrid.Margin = new Thickness(xPosition, yPosition, 0, 0);
            try
            {
                if (_generalGridMemory != null) _generalGridMemory.Children.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            generalGrid.Children.Add(this);
            _generalGridMemory = generalGrid;

            FileNameSuffixBox.Text = _outputHandlerFile.FileNameSuffixes[Header.Id];
            StartPositionBox.Text = _outputHandlerFile.StartAddress[Header.Id].ToString(CultureInfo.InvariantCulture);
            EndPositionBox.Text = _outputHandlerFile.EndAddress[Header.Id].ToString(CultureInfo.InvariantCulture);

            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xml", Content = "*.xml" });
            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Csv", Content = "*.csv" });
            OutputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xls", Content = "*.xls" });
            OutputTypeComboBox.SelectedIndex = _outputHandlerFile.SelectedIndex[Header.Id];
            _outputHandler.OutputWriter = OutputWriterFactory.CreateVariable(OutputTypeComboBox.SelectedItem.ToString());

            HeaderGroupBox.Header = "Output Handler " + Header.Id;
            _save = true;
        }

        private void FileNameSuffixChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { _outputHandlerFile.FileNameSuffixes[Header.Id] = box.Text; }
            catch (Exception) { _outputHandlerFile.FileNameSuffixes[Header.Id] = "noName"; }
            _outputHandlerFile.Save();
        }

        private void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!_save) return; 
            var outputTypeComboBox = (ComboBox)sender;
            _outputHandlerFile.SelectedIndex[Header.Id] = outputTypeComboBox.SelectedIndex;
            _outputHandler.OutputWriter = OutputWriterFactory.CreateVariable(OutputTypeComboBox.SelectedItem.ToString());
            _outputHandlerFile.Save();
        }

        private void StartPositionChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { _outputHandlerFile.StartAddress[Header.Id] = Convert.ToInt32(box.Text); }
            catch (Exception) { _outputHandlerFile.StartAddress[Header.Id] = 0; }
            _outputHandlerFile.Save();
        }

        private void EndPositionBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (!_save) return; 
            var box = (TextBox)sender;
            try { _outputHandlerFile.EndAddress[Header.Id] = Convert.ToInt32(box.Text); }
            catch (Exception) { _outputHandlerFile.EndAddress[Header.Id] = 0; }
            _outputHandlerFile.Save();
        }

        private void CreateOutput(object sender, RoutedEventArgs e)
        {
            Logger.Log("ID: " + Header.Id + " : Output file creation requested by the user");
            _outputHandler.CreateOutput();
        }
    }
}
