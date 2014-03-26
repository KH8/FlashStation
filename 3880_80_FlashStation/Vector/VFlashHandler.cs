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
            while(_vectorThread.IsAlive)
            {
                if (_outputInterface != null)
                {
                    _outputInterface.ModifyValue("ANTWORT", (Int16)99);
                    _outputInterface.ModifyValue("FEHLERCODE", (Int16)32);
                }
                Thread.Sleep(10);
            }
        }
    }
}
