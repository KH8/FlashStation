using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;
using _PlcAgent.DataAquisition;
using _PlcAgent.Log;
using _PlcAgent.Output;
using _PlcAgent.Output.OutputFileCreator;
using _PlcAgent.Output.Template;

namespace _PlcAgent.Visual.Gui.DataAquisition
{
    /// <summary>
    /// Interaction logic for GuiCommunicationInterfaceConfiguration_.xaml
    /// </summary>
    public partial class GuiCommunicationInterfaceConfiguration
    {
        #region Constructors

        public GuiCommunicationInterfaceConfiguration(CommunicationInterfaceHandler communicationHandler)
            : base(communicationHandler)
        {
            InitializeComponent();

            CommunicationInterfaceHandler.PlcCommunicator.PropertyChanged += OnConnectionStatusChanged;

            var words = CommunicationInterfaceHandler.PathFile.Path[CommunicationInterfaceHandler.Header.Id].Split('\\');
            InterfacePathBox.Text = words[words.Length - 1];
        }

        #endregion


        #region Event Handlers

        private void LoadSettingFile(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new OpenFileDialog {DefaultExt = ".csv", Filter = "CSV (MS-DOS) (.csv)|*.csv"};
            // Set filter for file extension and default file extension
            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result != true) return;

            CommunicationInterfaceHandler.PathFile.Path[CommunicationInterfaceHandler.Header.Id] = dlg.FileName;

            CommunicationInterfaceHandler.Initialize();
            CommunicationInterfaceHandler.PathFile.ConfigurationStatus[CommunicationInterfaceHandler.Header.Id] = 1;
            CommunicationInterfaceHandler.PathFile.Save();

            var words = dlg.FileName.Split('\\');
            InterfacePathBox.Text = words[words.Length - 1];
            Logger.Log("PLC Communication interface initialized with file: " + words[words.Length - 1]);
        }

        private void ExportToTemplate(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = CommunicationInterfaceHandler.Header.Name + "_Template",
                DefaultExt = ".xml",
                Filter = "eXtensible Markup Language File (.xml)|*.xml"
            };
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result != true) return;

            new XmlFileCreator().CreateOutput(dlg.FileName, (DataTemplateComposite)OutputDataTemplateBuilder.ComunicationInterfaceToTemplate(CommunicationInterfaceHandler.ReadInterfaceComposite), FileCreator.OutputConfiguration.Template);
           
            Logger.Log("PLC Communication interface exported to Template file: " + dlg.FileName);
        }

        public void OnConnectionStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ConnectionStatus") return;
            LoadFileButton.Dispatcher.BeginInvoke(
                (new Action(
                    delegate
                    {
                        LoadFileButton.IsEnabled = CommunicationInterfaceHandler.PlcCommunicator.ConnectionStatus != 1;
                    })));
        }

        #endregion

        
    }
}
