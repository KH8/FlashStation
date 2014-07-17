using System;
using System.Threading;
using System.Windows;
using _PlcAgent.DataAquisition;
using _PlcAgent.Log;
using _PlcAgent.PLC;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiCommunicationInterfaceConfiguration_.xaml
    /// </summary>
    public partial class GuiCommunicationInterfaceConfiguration
    {
        private readonly PlcCommunicator _plcCommunicator;
        private readonly CommunicationInterfaceHandler _communicationHandler;
        private readonly CommunicationInterfacePath _communicationInterfacePath;

        private readonly Thread _updateThread;

        public GuiCommunicationInterfaceConfiguration(CommunicationInterfaceHandler communicationHandler)
        {
            _communicationHandler = communicationHandler;
            _plcCommunicator = _communicationHandler.PlcCommunicator;
            _communicationInterfacePath = _communicationHandler.PathFile;

            InitializeComponent();

            _updateThread = new Thread(Update);
            _updateThread.SetApartmentState(ApartmentState.STA);
            _updateThread.IsBackground = true;
            _updateThread.Start();

            var words = _communicationInterfacePath.Path[_communicationHandler.Header.Id].Split('\\');
            InterfacePathBox.Text = words[words.Length - 1];
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
                _communicationInterfacePath.Path[_communicationHandler.Header.Id] = dlg.FileName;

                _communicationHandler.Initialize();
                _communicationInterfacePath.ConfigurationStatus[_communicationHandler.Header.Id] = 1;
                _communicationInterfacePath.Save();

                var words = dlg.FileName.Split('\\');
                InterfacePathBox.Text = words[words.Length - 1];
                Logger.Log("PLC Communication interface initialized with file: " + words[words.Length - 1]);
            }
        }

        public void Update()
        {
            while (_updateThread.IsAlive)
            {
                LoadFileButton.Dispatcher.BeginInvoke((new Action(delegate { LoadFileButton.IsEnabled = _plcCommunicator.ConnectionStatus != 1; })));
                Thread.Sleep(21);
            }
        }
    }
}
