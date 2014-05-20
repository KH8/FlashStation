using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.Output;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiOutputCreator : Gui
    {
        private Grid _generalGrid;

        private readonly CommunicationInterfaceHandler _communicationHandler;
        private OutputCreatorFile _outputCreatorFile;

        private ComboBox _outputTypeComboBox = new ComboBox();

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiOutputCreator(CommunicationInterfaceHandler communicationHandler, OutputCreatorFile outputCreatorFile)
        {
            _communicationHandler = communicationHandler;
            _outputCreatorFile = outputCreatorFile;
        }

        public override void Initialize(uint id, int xPosition, int yPosition)
        {
            Id = id;
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Left, VerticalAlignment.Top, 240, 250);

            var guiOutputCreatorGroupBox = GuiFactory.CreateGroupBox("Output Creation", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 240, 250);
            _generalGrid.Children.Add(guiOutputCreatorGroupBox);

            var guiOutputCreatorGrid = GuiFactory.CreateGrid();
            guiOutputCreatorGroupBox.Content = guiOutputCreatorGrid;

            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateLabel("Start Position:", 5, 15, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateLabel("End Position:", 5, 43, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateTextBox("StartPositionBox", _outputCreatorFile.StartAddress.ToString(CultureInfo.InvariantCulture), 130, 10, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, StartPositionChanged));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateTextBox("EndPositionBox", _outputCreatorFile.EndAddress.ToString(CultureInfo.InvariantCulture), 130, 38, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 100, EndPositionBoxChanged));

            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateLabel("Output File Type:", 5, 71, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 105));
            guiOutputCreatorGrid.Children.Add(_outputTypeComboBox = GuiFactory.CreateComboBox("OutputTypeComboBox", "Select", 130, 71, HorizontalAlignment.Left, VerticalAlignment.Top, 22, 100));
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xml", Content = "*.xml" });
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Csv", Content = "*.csv" });
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xls", Content = "*.xls" });

            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateButton("CreateOutputButton", "CreateOutput", 130, 99, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100, CreateOutput));
        }

        private void StartPositionChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _outputCreatorFile.StartAddress = Convert.ToInt32(box.Text); }
            catch (Exception) { _outputCreatorFile.StartAddress = 0; }
            _outputCreatorFile.Save();
        }

        private void EndPositionBoxChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            try { _outputCreatorFile.EndAddress = Convert.ToInt32(box.Text); }
            catch (Exception) { _outputCreatorFile.EndAddress = 0; }
            _outputCreatorFile.Save();
        }

        private void CreateOutput(object sender, RoutedEventArgs e)
        {
            OutputWriter outputWriter = null;
            var selection = _outputTypeComboBox.SelectedValue;
            if (selection == null)
            {
                MessageBox.Show("No file type selected!", "Error");
                return;
            }
            switch (selection.ToString())
            {
                case "System.Windows.Controls.ComboBoxItem: *.xml":
                    outputWriter = new OutputXmlWriter();
                    break;
                case "System.Windows.Controls.ComboBoxItem: *.csv":
                    outputWriter = new OutputCsvWriter();
                    break;
                case "System.Windows.Controls.ComboBoxItem: *.xls":
                    outputWriter = new OutputXlsWriter();
                    break;
            }
            if (outputWriter != null)
                try
                {
                    outputWriter.CreateOutput("out", outputWriter.InterfaceToStrings(_communicationHandler.WriteInterfaceComposite, _outputCreatorFile.StartAddress, _outputCreatorFile.EndAddress));
                }
                catch (Exception)
                {
                    MessageBox.Show("Output creation Failed!", "Error");
                    Logger.Log("Output creation Failed");
                }
        }

        public override void MakeVisible(uint id)
        {
            _generalGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible(uint id)
        {
            _generalGrid.Visibility = Visibility.Hidden;
        }
    }
}
