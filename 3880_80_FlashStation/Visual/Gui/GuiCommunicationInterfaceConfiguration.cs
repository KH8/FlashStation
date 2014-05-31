using System;
using System.Windows;
using System.Windows.Controls;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiComInterfacemunicationConfiguration : Gui
    {
        private Grid _generalGrid;

        private readonly CommunicationInterfaceHandler _communicationHandler;
        private readonly CommunicationInterfacePath _communicationInterfacePath;

        private TextBox _interfacePathBox = new TextBox();

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiComInterfacemunicationConfiguration(uint id, CommunicationInterfaceHandler communicationHandler, CommunicationInterfacePath communicationInterfacePath)
        {
            Id = id;

            _communicationHandler = communicationHandler;
            _communicationInterfacePath = communicationInterfacePath;
        }

        public override void Initialize(int xPosition, int yPosition)
        {
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Left, VerticalAlignment.Top, 240, 320);

            var guiInterfaceGroupBox = GuiFactory.CreateGroupBox("Interface Configuration", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 58, 320);
            _generalGrid.Children.Add(guiInterfaceGroupBox);

            var guiInterfaceGrid = GuiFactory.CreateGrid();
            guiInterfaceGroupBox.Content = guiInterfaceGrid;

            guiInterfaceGrid.Children.Add(GuiFactory.CreateLabel("Configuration File:", 31, 5, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 112));
            guiInterfaceGrid.Children.Add(_interfacePathBox = GuiFactory.CreateTextBox("InterfacePathBox", "File not loaded", 150, 5, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 120));

            string[] words = _communicationInterfacePath.Path[Id].Split('\\');
            _interfacePathBox.Text = words[words.Length - 1];

            _generalGrid.Children.Add(GuiFactory.CreateButton("LoadFileButton", "Load File", 0, 62, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100, LoadSettingFile));
        }

        private void LoadSettingFile(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".csv", Filter = "CSV (MS-DOS) (.csv)|*.csv" };
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                _communicationInterfacePath.Path[Id] = dlg.FileName;

                try { _communicationHandler.Initialize(); }
                catch (Exception)
                {
                    MessageBox.Show("Input file cannot be used", "Error");
                    return;
                }

                _communicationInterfacePath.ConfigurationStatus[Id] = 1;
                _communicationInterfacePath.Save();

                string[] words = dlg.FileName.Split('\\');
                _interfacePathBox.Text = words[words.Length - 1];
                Logger.Log("PLC Communication interface initialized with file: " + words[words.Length - 1]);
            }
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
