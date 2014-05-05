using System;
using System.Windows;
using System.Windows.Controls;

namespace _3880_80_FlashStation.Visual
{
    class PlcConfigurationGui : Gui
    {
        public Grid GeneralGrid;

        public override void Initialize(uint id, int xPosition, int yPosition)
        {
            Id = id;
            XPosition = xPosition;
            YPosition = yPosition;

            GeneralGrid = GuiFactory.CreateGrid(XPosition, YPosition, HorizontalAlignment.Center, VerticalAlignment.Top, 250);

            var guiCommunicationGroupBox = GuiFactory.CreateGroupBox("PLC Communication Setup", 0, 0, HorizontalAlignment.Center, VerticalAlignment.Top, 148, 334);
            GeneralGrid.Children.Add(guiCommunicationGroupBox);

            var guiCommunicationGrid = GuiFactory.CreateGrid();
            guiCommunicationGroupBox.Content = guiCommunicationGrid;

            guiCommunicationGrid.Children.Add(GuiFactory.CreateLabel("IP Address:", 68, 10, HorizontalAlignment.Left, VerticalAlignment.Top, 25, 80));

            /*<GroupBox Header="PLC Communication Setup" HorizontalAlignment="Center" Height="148" VerticalAlignment="Top" Width="334" Margin="0,0,339,0">
                        <Grid>
                            <TextBox Name="IpAddressBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="148,10,0,0" TextWrapping="Wrap" Text="192.168.10.80" VerticalAlignment="Top" Width="100" TextChanged="IpAddressBoxChanged"/>
                            <Label Content="Port:" HorizontalAlignment="Left" Height="25" Margin="68,37,0,0" VerticalAlignment="Top" Width="80"/>
                            <TextBox Name="PortBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="148,37,0,0" TextWrapping="Wrap" Text="102" VerticalAlignment="Top" Width="100" TextChanged="PortBoxChanged"/>
                            <Label Content="Rack:" HorizontalAlignment="Left" Height="25" Margin="68,64,0,0" VerticalAlignment="Top" Width="80"/>
                            <TextBox Name="RackBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="148,64,0,0" TextWrapping="Wrap" Text="0" VerticalAlignment="Top" Width="100" TextChanged="RackBoxChanged"/>
                            <Label Content="Slot:" HorizontalAlignment="Left" Height="25" Margin="68,91,0,0" VerticalAlignment="Top" Width="80"/>
                            <TextBox Name="SlotBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="148,91,0,0" TextWrapping="Wrap" Text="0" VerticalAlignment="Top" Width="100" TextChanged="SlotBoxChanged"/>
                        </Grid>
                    </GroupBox>
                    <GroupBox Header="PLC Data Setup" HorizontalAlignment="Left" Height="206" Margin="339,0,-1,0" VerticalAlignment="Top" Width="335">
                        <Grid>
                            <Label Content="Read DB Number:" HorizontalAlignment="Left" Height="25" Margin="63,10,0,0" VerticalAlignment="Top" Width="115"/>
                            <TextBox Name="ReadDbAddressBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="178,10,0,0" TextWrapping="Wrap" Text="1000" VerticalAlignment="Top" Width="85" TextChanged="ReadDbNumberBoxChanged"/>
                            <Label Content="Read Start Address:" HorizontalAlignment="Left" Height="25" Margin="63,37,0,0" VerticalAlignment="Top" Width="115"/>
                            <TextBox Name="ReadDbStartAddressBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="178,37,0,0" TextWrapping="Wrap" Text="0" VerticalAlignment="Top" Width="85" TextChanged="ReadStartAddressBoxChanged"/>
                            <Label Content="Read Data Length:" HorizontalAlignment="Left" Height="25" Margin="63,64,0,0" VerticalAlignment="Top" Width="115"/>
                            <TextBox Name="ReadDbLengthBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="178,64,0,0" TextWrapping="Wrap" Text="100" VerticalAlignment="Top" Width="85" TextChanged="ReadLengthBoxChanged"/>
                            <Label Content="Write DB Number:" HorizontalAlignment="Left" Height="25" Margin="63,92,0,0" VerticalAlignment="Top" Width="115"/>
                            <TextBox x:Name="WriteDbAddressBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="178,92,0,0" TextWrapping="Wrap" Text="1000" VerticalAlignment="Top" Width="85" TextChanged="WriteDbNumberBoxChanged"/>
                            <Label Content="Write Start Address:" HorizontalAlignment="Left" Height="25" Margin="63,119,0,0" VerticalAlignment="Top" Width="115"/>
                            <TextBox x:Name="WriteDbStartAddressBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="178,119,0,0" TextWrapping="Wrap" Text="512" VerticalAlignment="Top" Width="85" TextChanged="WriteStartAddressBoxChanged"/>
                            <Label Content="Write Data Length:" HorizontalAlignment="Left" Height="25" Margin="63,146,0,0" VerticalAlignment="Top" Width="115"/>
                            <TextBox x:Name="WriteDbLengthBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="178,146,0,0" TextWrapping="Wrap" Text="100" VerticalAlignment="Top" Width="85" TextChanged="WriteLengthBoxChanged"/>
                        </Grid>
                    </GroupBox>
                    <Button Content="Use Settings" HorizontalAlignment="Right" Height="25" Margin="0,211,0,0" VerticalAlignment="Top" Width="100" Click="StoreSettings"/>
                    <Button IsEnabled="True" Content="Load File" HorizontalAlignment="Right" Height="25" Margin="0,211,339,0" VerticalAlignment="Top" Width="100" Click="LoadSettingFile"/>
                    <GroupBox Header="Interface Configuration" HorizontalAlignment="Center" Height="58" VerticalAlignment="Top" Width="334" Margin="0,148,339,0">
                        <Grid>
                            <Label Content="Configuration File:" HorizontalAlignment="Left" Height="25" Margin="31,5,0,0" VerticalAlignment="Top" Width="112"/>
                            <TextBox x:Name="InterfacePathBox" HorizontalContentAlignment="Right" HorizontalAlignment="Left" Height="25" Margin="148,5,0,0" TextWrapping="Wrap" Text="File not loaded" VerticalAlignment="Top" Width="164"/>
                        </Grid>
                    </GroupBox>
             */
        }

        public override void MakeVisible(uint id)
        {
            GeneralGrid.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible(uint id)
        {
            GeneralGrid.Visibility = Visibility.Hidden;
        }
    }
}
