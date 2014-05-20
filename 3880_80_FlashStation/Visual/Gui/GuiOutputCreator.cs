using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.Output;
using _3880_80_FlashStation.Vector;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiOutputCreator : Gui
    {
        private Grid _generalGrid;

        private CommunicationInterfaceHandler _communicationHandler;

        private ComboBox _outputTypeComboBox = new ComboBox();

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiOutputCreator(CommunicationInterfaceHandler communicationHandler)
        {
            _communicationHandler = communicationHandler;
        }

        public override void Initialize(uint id, int xPosition, int yPosition)
        {
            Id = id;
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Left, VerticalAlignment.Top, 100, 250);

            var guiOutputCreatorGroupBox = GuiFactory.CreateGroupBox("Output Creation", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 100, 250);
            _generalGrid.Children.Add(guiOutputCreatorGroupBox);

            var guiOutputCreatorGrid = GuiFactory.CreateGrid();
            guiOutputCreatorGroupBox.Content = guiOutputCreatorGrid;

            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateLabel("Output File Type:", 5, 10, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 105));
            guiOutputCreatorGrid.Children.Add(GuiFactory.CreateButton("CreateOutputButton", "CreateOutput", 130, 40, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100, CreateOutput));

            guiOutputCreatorGrid.Children.Add(_outputTypeComboBox = GuiFactory.CreateComboBox("OutputTypeComboBox", "Select", 110, 13, HorizontalAlignment.Left, VerticalAlignment.Top, 22, 120));
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xml", Content = "*.xml" });
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Csv", Content = "*.csv" });
            _outputTypeComboBox.Items.Add(new ComboBoxItem { Name = "Xls", Content = "*.xls" });
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
                    outputWriter.CreateOutput("out", outputWriter.InterfaceToStrings(_communicationHandler.WriteInterfaceComposite, 0, 10));
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
