using System;
using System.Threading;
using System.Windows;
using _ttAgent.DataAquisition;
using _ttAgent.Log;

namespace _ttAgent.Output
{
    class OutputHandler
    {
        #region Variables

        private readonly uint _id;
        private Boolean _pcControlMode;
        private Boolean _pcControlModeChangeAllowed;

        private OutputWriter _outputWriter;

        private readonly CommunicationInterfaceComposite _inputComposite;
        private readonly CommunicationInterfaceComposite _outputComposite;

        private readonly Thread _outputThread;

        #endregion

        #region Properties

        public Boolean PcControlMode
        {
            get { return _pcControlMode; }
            set { if (_pcControlModeChangeAllowed) { _pcControlMode = value;}}
        }

        public OutputWriter OutputWriter
        {
            get { return _outputWriter; }
            set { _outputWriter = value; }
        }

        #endregion

        #region Constructor

        public OutputHandler(uint id, CommunicationInterfaceComposite inputComposite, CommunicationInterfaceComposite outputComposite)
        {
            _id = id;
            _inputComposite = inputComposite;
            _outputComposite = outputComposite;

            _outputThread = new Thread(OutputCommunicationThread);
            _outputThread.SetApartmentState(ApartmentState.STA);
            _outputThread.IsBackground = true;
        }

        #endregion

        #region Methods

        public void InitializeOutputHandler()
        {
            try { CheckInterface(); }
            catch (Exception)
            {
                MessageBox.Show("ID: " + _id + " Output Handler initialization failed", "Output Handler Failed");
                throw new OutputHandlerException("Output Handler initialization failed");
            }

            _outputThread.Start();
            Logger.Log("ID: " + _id + " Output Handler Initialized");
        }

        public void CreateOutput()
        {
            string fileName = OutputHandlerFile.Default.FileNameSuffix[_id];
            var interfaceVariable = fileName.Split('%');
            var interfaceComponent = _inputComposite.ReturnVariable(interfaceVariable[1]);
            if (interfaceComponent != null)
            {
                if (interfaceComponent.Type == CommunicationInterfaceComponent.VariableType.String)
                {
                    var ciString = (CiString) interfaceComponent;
                    fileName = ciString.Value;
                }
            }

            if (_outputWriter != null)
            {
                try
                {
                    _outputWriter.CreateOutput(fileName,
                        _outputWriter.InterfaceToStrings(_inputComposite,
                            OutputHandlerFile.Default.StartAddress[_id],
                            OutputHandlerFile.Default.EndAddress[_id]));
                }
                catch (Exception)
                {
                    MessageBox.Show("ID: " + _id + " : Output creation Failed!", "Error");
                    Logger.Log("ID: " + _id + " : Output creation Failed");
                }
            }
        }

        #endregion

        #region Background methods

        private void OutputCommunicationThread()
        {
            Int16 counter = 0;
            Int16 caseAuxiliary = 0;

            while (_outputThread.IsAlive)
            {
                var inputCompositeCommand = (CiInteger)_inputComposite.ReturnVariable("BEFEHL");
                
                _pcControlModeChangeAllowed = false;

                Int16 antwort;
                Int16 status;
                if (!_pcControlMode && CheckInterface())
                    switch (inputCompositeCommand.Value)
                    {
                        case 100:
                            if (caseAuxiliary != 100)
                            {
                                Logger.Log("ID: " + _id + " : Output file creation requested from PLC");
                                CreateOutput();
                            }
                            caseAuxiliary = 100;
                            antwort = 100;
                            status = 100;
                            break;
                        default:
                            caseAuxiliary = 0;
                            antwort = 0;
                            status = 0;
                            break;
                    }
                else
                {
                    antwort = 999;
                    status = 999;
                    _pcControlModeChangeAllowed = true;
                }

                if (_outputComposite != null)
                {
                    _outputComposite.ModifyValue("LEBENSZAECHLER", counter);
                    counter++;
                    _outputComposite.ModifyValue("ANTWORT", antwort);
                    _outputComposite.ModifyValue("STATUS", status);
                }
                Thread.Sleep(200);
            }
        }

        #endregion

        #region Auxiliaries

        public class OutputHandlerException : ApplicationException
        {
            public OutputHandlerException(string info) : base(info) { }
        }

        private Boolean CheckInterface()
        {
            CommunicationInterfaceComponent component = _inputComposite.ReturnVariable("BEFEHL");
            if (component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                throw new OutputHandlerException("The assigned interface does not contain a required component");

            component = _outputComposite.ReturnVariable("LEBENSZAECHLER");
            if (component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                throw new OutputHandlerException("The assigned interface does not contain a required component");
            component = _outputComposite.ReturnVariable("ANTWORT");
            if (component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                throw new OutputHandlerException("The assigned interface does not contain a required component");
            component = _outputComposite.ReturnVariable("STATUS");
            if (component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                throw new OutputHandlerException("The assigned interface does not contain a required component");

            return true;
        }

        #endregion
    }
}
