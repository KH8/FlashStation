using System;
using System.ComponentModel;
using System.Threading;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Thread _statusThread;
        private PlcCommunicator _plcCommunication;
        private PlcConfigurator _plcConfiguration;

        public MainWindow()
        {
            InitializeComponent();

            PlcConfigurationFile.Default.Configuration = new PlcCommunicatorBase.PlcConfig {PlcIpAddress = "99.99.99.99"};
            PlcConfigurationFile.Default.Save();

            _plcCommunication = new PlcCommunicator();
            _plcConfiguration = new PlcConfigurator();
            _plcConfiguration.UpdateConfiguration(PlcConfigurationFile.Default.Configuration);
            _plcCommunication.SetupConnection(_plcConfiguration);

            _statusThread = new Thread(StatusHandler);
            _statusThread.SetApartmentState(ApartmentState.STA);
            _statusThread.IsBackground = true;
            _statusThread.Start();

        }

        private void StatusHandler()
        {
            while (_statusThread.IsAlive)
            {
                if (_plcCommunication != null)
                {
                    ActIpAddressLabel.Dispatcher.BeginInvoke((new Action(delegate
                    {
                        ActIpAddressLabel.Content = _plcCommunication.PlcConfiguration.PlcIpAddress;
                    })));
                }

                Thread.Sleep(500);
            }
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
