using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.Log;
using _ttAgent.Output;

namespace _ttAgent.Visual.Gui
{
    class GuiOutputHandler : Gui
    {
        private Grid _generalGrid;

        private readonly OutputHandler _outputHandler;
        private readonly OutputHandlerFile _outputHandlerFile;

        private ComboBox _outputTypeComboBox = new ComboBox();

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiOutputHandler(uint id, OutputHandler outputHandler, OutputHandlerFile outputHandlerFile)
        {
            Id = id;
            _outputHandler = outputHandler;
            _outputHandlerFile = outputHandlerFile;
        }

        public override void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = generalGrid;

            var guiOutputCreatorGroupBox = GuiFactory.CreateGroupBox("Output Creation", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 215, 250);
            _generalGrid.Children.Add(guiOutputCreatorGroupBox);

            var guiOutputCreatorGrid = GuiFactory.CreateGrid();
            guiOutputCreatorGroupBox.Content = guiOutputCreatorGrid;

            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateLabel("File Name Suffix:", 5, 15, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateLabel("Start Position:", 5, 45, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateLabel("End Position:", 5, 75, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateTextBox("FileNameSuffixBox", _outputHandlerFile.FileNameSuffixes[Id], 130, 13, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, FileNameSuffixChanged));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateTextBox("StartPositionBox", _outputHandlerFile.StartAddress[Id].ToString(CultureInfo.InvariantCulture), 130, 43, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, StartPositionChanged));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateTextBox("EndPositionBox", _outputHandlerFile.EndAddress[Id].ToString(CultureInfo.InvariantCulture), 130, 73, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, EndPositionBoxChanged));

            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateLabel("Output File Type:", 5, 106, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 105));
            guiOutputCreatorGrid.Children.Add(_outputTypeComboBox = GuiFactory.CreateComboBox("OutputTypeComboBox", "Select", 130, 106, HorizontalAlignment.Left, VerticalAlignment.Top, 22, 100));
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xml", Content = "*.xml" });
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Csv", Content = "*.csv" });
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xls", Content = "*.xls" });
            _outputTypeComboBox.SelectedIndex = _outputHandlerFile.SelectedIndex[Id];
            _outputHandler.OutputWriter = OutputWriterFactory.CreateVariable(_outputTypeComboBox.SelectedItem.ToString());
            _outputTypeComboBox.SelectionChanged += ComboBoxOnSelectionChanged;

            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateButton("CreateOutputButton", "CreateOutput", 130, 136, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100, CreateOutput));
        }

        private void FileNameSuffixChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _outputHandlerFile.FileNameSuffixes[Id] = box.Text; }
            catch (Exception) { _outputHandlerFile.FileNameSuffixes[Id] = "noName"; }
            _outputHandlerFile.Save();
        }

        private void ComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var outputTypeComboBox = (ComboBox) sender;
            _outputHandlerFile.SelectedIndex[Id] = outputTypeComboBox.SelectedIndex;
            _outputHandler.OutputWriter = OutputWriterFactory.CreateVariable(_outputTypeComboBox.SelectedItem.ToString());
            _outputHandlerFile.Save();
        }

        private void StartPositionChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _outputHandlerFile.StartAddress[Id] = Convert.ToInt32(box.Text); }
            catch (Exception) { _outputHandlerFile.StartAddress[Id] = 0; }
            _outputHandlerFile.Save();
        }

        private void EndPositionBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _outputHandlerFile.EndAddress[Id] = Convert.ToInt32(box.Text); }
            catch (Exception) { _outputHandlerFile.EndAddress[Id] = 0; }
            _outputHandlerFile.Save();
        }

        private void CreateOutput(object sender, RoutedEventArgs e)
        {
            Logger.Log("ID: " + Id + " : Output file creation requested by the user");
            _outputHandler.CreateOutput();
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
