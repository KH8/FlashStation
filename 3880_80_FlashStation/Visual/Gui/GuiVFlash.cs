using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Visual.Gui
{
    class GuiVFlash : Gui
    {
        private Grid _generalGrid;

        //

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

            _generalGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Center, VerticalAlignment.Top, 150, 800);

            var guiVFlashGroupBox = GuiFactory.CreateGroupBox("Channel " + Id, 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 150, 795);
            _generalGrid.Children.Add(guiVFlashGroupBox);

            var guiVFlashGrid = GuiFactory.CreateGrid();
            guiVFlashGroupBox.Content = guiVFlashGrid;

            guiVFlashGrid.Children.Add(GuiFactory.CreateLabel("Actual Path Path: ", 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 112));
            guiVFlashGrid.Children.Add(GuiFactory.CreateLabel("Progress: ", 0, 26, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 61));

            //guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("IP Address:", 68, 10, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));
            //guiDataGrid.Children.Add(GuiFactory.CreateTextBox("WriteDbLengthBox", _guiPlcConfiguration.PlcWriteLength.ToString(CultureInfo.InvariantCulture), 178, 146, HorizontalAlignment.Left, VerticalAlignment.Top, HorizontalAlignment.Right, 25, 85, WriteLengthBoxChanged));
            //_generalGrid.Children.Add(GuiFactory.CreateButton("LoadFileButton", "Load File", 235, 211, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 100, LoadSettingFile));

            /*<GroupBox Header="Channel 1" HorizontalAlignment="Center" Height="120" VerticalAlignment="Top" Width="673">
                        <Grid>
                            <ProgressBar Name="VFlash1ProgressBar" HorizontalAlignment="Left" Height="16" Margin="62,0,0,51" VerticalAlignment="Bottom" Width="598" Foreground="{x:Null}" UseLayoutRounding="False"/>
/////<Label Content="Actual Path Path: " HorizontalAlignment="Left" VerticalAlignment="Top" Width="112"/>
                            <Label Name="VFlash1ProjectPathLabel" Content="Channel is not activated" HorizontalAlignment="Left" VerticalAlignment="Top" Width="548" Height="22" Margin="112,2,0,0" BorderBrush="#FFCFCFCF" Background="White" BorderThickness="1" Padding="5,1,2,2"/>
/////<Label Content="Progress: " HorizontalAlignment="Left" VerticalAlignment="Top" Width="61" Margin="0,26,0,0"/>
                            <Label Name="VFlash1StatusLabel" Content="No project loaded." HorizontalContentAlignment="Right" HorizontalAlignment="Right" VerticalAlignment="Top" Width="562" Margin="0,43,0,0" FontSize="10" Foreground="Red" Height="25"/>
                            <CheckBox Name="VFlash1ControlBox" Content="PC Control" HorizontalAlignment="Right" VerticalAlignment="Top" Margin="0,80,5,0" Width="75" FlowDirection="RightToLeft" Checked="VFlashControlModeChanged" Unchecked="VFlashControlModeChanged"/>
                            <Button Name="VFlash1LoadButton" Content="Load Path" HorizontalAlignment="Left" Margin="5,0,0,5" VerticalAlignment="Bottom" Width="90" Height="25" RenderTransformOrigin="0.5,0.5" Click="LoadVFlashProject" ClickMode="Release"/>
                            <Button Name="VFlash1UnloadButton" Content="Unload Path" HorizontalAlignment="Left" Margin="100,0,0,5" VerticalAlignment="Bottom" Width="90" Height="25" RenderTransformOrigin="0.5,0.5" Click="UnloadVFlashProject" ClickMode="Release"/>
                            <Button Name="VFlash1FlashButton" Content="Flash" HorizontalAlignment="Left" Margin="195,0,0,5" VerticalAlignment="Bottom" Width="90" Height="25" RenderTransformOrigin="0.444,1.12" Click="FlashVFlashProject"  ClickMode="Release" FontWeight="Bold"/>
                            <Button Name="VFlash1FaultsButton" Content="Faults" HorizontalAlignment="Left" Margin="290,0,0,5" VerticalAlignment="Bottom" Width="90" Height="25" RenderTransformOrigin="0.444,1.12" Click="VFlashShowFaults"/>
                            <Label x:Name="VFlash1TimeLabel" Content="Remaining time: 0" HorizontalContentAlignment="Right" HorizontalAlignment="Right" VerticalAlignment="Top" Width="483" Margin="0,26,4,0" FontSize="10" Foreground="Black" Height="25"/>
                        </Grid>
                    </GroupBox>*/
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
