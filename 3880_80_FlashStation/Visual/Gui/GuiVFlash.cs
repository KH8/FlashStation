using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiVFlash : Gui
    {
        private Grid _generalGrid;

        private ProgressBar _vFlashProgressBar = new ProgressBar();
        private Label _vFlashProjectPathLabel = new Label();
        private Label _vFlashStatusLabel = new Label();
        private Label _vFlashTimeLabel = new Label();
        private Button _vFlashLoadButton = new Button();
        private Button _vFlashUnloadButton = new Button();
        private Button _vFlashFlashButton = new Button();
        private Button _vFlashFaultsButton = new Button();
        private CheckBox _vFlashControlBox = new CheckBox();

        public Grid GeneralGrid
        {
            get { return _generalGrid; }
            set { _generalGrid = value; }
        }

        public GuiVFlash()
        {
            
        }

        public override void Initialize(uint id, int xPosition, int yPosition)
        {
            Id = id;
            XPosition = xPosition;
            YPosition = yPosition;

            _generalGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Center, VerticalAlignment.Top, 120, 800);

            var guiVFlashGroupBox = GuiFactory.CreateGroupBox("Channel " + Id, 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 120, 795);
            _generalGrid.Children.Add(guiVFlashGroupBox);

            var guiVFlashGrid = GuiFactory.CreateGrid();
            guiVFlashGroupBox.Content = guiVFlashGrid;

            guiVFlashGrid.Children.Add(_vFlashProgressBar = GuiFactory.CreateProgressBar("VFlash1ProgressBar", 62, 37, HorizontalAlignment.Left, VerticalAlignment.Top, 16, 721));

            guiVFlashGrid.Children.Add(GuiFactory.CreateLabel("Actual Path Path: ", 0, 5, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 112));
            guiVFlashGrid.Children.Add(GuiFactory.CreateLabel("Progress: ", 0, 31, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 61));

            guiVFlashGrid.Children.Add(_vFlashProjectPathLabel = GuiFactory.CreateLabel("VFlash1ProjectPathLabel", "Channel is not activated", 112, 7, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Left, 22, 671));
            var converter = new BrushConverter();
            _vFlashProjectPathLabel.BorderBrush = (Brush)converter.ConvertFromString("#FFCFCFCF");
            _vFlashProjectPathLabel.BorderThickness = new Thickness(1);
            _vFlashProjectPathLabel.Padding = new Thickness(5, 1, 2, 2);
            _vFlashProjectPathLabel.Background = Brushes.White;
            
            guiVFlashGrid.Children.Add(_vFlashStatusLabel = GuiFactory.CreateLabel("VFlash1StatusLabel", "No project loaded.", 4, 51, HorizontalAlignment.Right, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 562));
            _vFlashStatusLabel.Foreground = Brushes.Red;
            _vFlashStatusLabel.FontSize = 10;

            guiVFlashGrid.Children.Add(_vFlashTimeLabel = GuiFactory.CreateLabel("VFlash1TimeLabel", "Remaining time: 00:00:00", 4, 33, HorizontalAlignment.Right, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 483));
            _vFlashTimeLabel.Foreground = Brushes.Black;
            _vFlashTimeLabel.FontSize = 10;

            guiVFlashGrid.Children.Add(_vFlashLoadButton = GuiFactory.CreateButton("VFlash1LoadButton", "Load Path", 5, 10, HorizontalAlignment.Left, VerticalAlignment.Bottom, 25, 90, LoadVFlashProject));
            guiVFlashGrid.Children.Add(_vFlashUnloadButton = GuiFactory.CreateButton("VFlash1UnloadButton", "Unload Path", 100, 10, HorizontalAlignment.Left, VerticalAlignment.Bottom, 25, 90, UnloadVFlashProject));
            guiVFlashGrid.Children.Add(_vFlashFlashButton = GuiFactory.CreateButton("VFlash1FlashButton", "Flash", 195, 10, HorizontalAlignment.Left, VerticalAlignment.Bottom, 25, 90, FlashVFlashProject));
            _vFlashFlashButton.FontWeight = FontWeights.Bold;
            guiVFlashGrid.Children.Add(_vFlashFaultsButton = GuiFactory.CreateButton("VFlash1FaultsButton", "Faults", 290, 10, HorizontalAlignment.Left, VerticalAlignment.Bottom, 25, 90, VFlashShowFaults));

            guiVFlashGrid.Children.Add(_vFlashControlBox = GuiFactory.CreateCheckBox("VFlash1ControlBox", "PC Control", 5, 10, HorizontalAlignment.Right, VerticalAlignment.Bottom, 77, VFlashControlModeChanged));
        }

        private void VFlashControlModeChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LoadVFlashProject(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UnloadVFlashProject(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FlashVFlashProject(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void VFlashShowFaults(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
