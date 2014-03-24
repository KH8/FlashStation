using System.Windows;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var plc = new PlcCommunicator();
            plc.SetupConnection(new PlcCommunicator.PlcConfig());
        }
    }
}
