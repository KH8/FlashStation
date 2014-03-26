using System;
using System.Threading;
using _3880_80_FlashStation.DataAquisition;

namespace _3880_80_FlashStation.Vector
{
    class VFlashHandler
    {
        private Thread _vectorThread;
        private CommunicationInterfaceComposite _inputInterface;
        private CommunicationInterfaceComposite _outputInterface;

        public CommunicationInterfaceComposite InputInterface
        {
            set { _inputInterface = value; }
        }

        public CommunicationInterfaceComposite OutputInterface
        {
            get { return _outputInterface; }
            set { _outputInterface = value; }
        }

        public VFlashHandler()
        {
            _vectorThread = new Thread(VectorBackgroundThread);
            _vectorThread.SetApartmentState(ApartmentState.STA);
            _vectorThread.IsBackground = true;
            _vectorThread.Start();
        }

        private void VectorBackgroundThread()
        {
            Int16 val = 0;

            while(_vectorThread.IsAlive)
            {
                if (_outputInterface != null)
                {
                    val += 1;
                    _outputInterface.ModifyValue("ANTWORT", val);
                    _outputInterface.ModifyValue("FEHLERCODE", (Int16)(val - 2 * val));
                }
                Thread.Sleep(10);
            }
        }
    }
}
