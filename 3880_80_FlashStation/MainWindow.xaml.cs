using System;
using System.ComponentModel;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            PlcConfigurationFile.Default.Configuration = new PlcCommunicatorBase.PlcConfig();
            PlcConfigurationFile.Default.Save();

            var plcCommunication = new PlcCommunicator();
            var plcConfiguration = new PlcConfigurator();
            plcConfiguration.UpdateConfiguration(PlcConfigurationFile.Default.Configuration);
            plcCommunication.SetupConnection(plcConfiguration);

        }

        private void CloseApp(object sender, CancelEventArgs cancelEventArgs)
        {
            Environment.Exit(0);
        }

        private void StoreSettings(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ConnectDisconnect(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
