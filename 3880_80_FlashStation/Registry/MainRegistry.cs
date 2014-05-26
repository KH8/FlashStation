using System;
using System.Collections.Generic;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Output;
using _3880_80_FlashStation.PLC;
using _3880_80_FlashStation.Vector;

namespace _3880_80_FlashStation.Registry
{
    abstract class RegistryBase
    {
        public Dictionary<int, PlcCommunicator> PlcCommunicators = new Dictionary<int, PlcCommunicator>();
        public Dictionary<int, CommunicationInterfaceHandler> CommunicationInterfaceHandlers = new Dictionary<int, CommunicationInterfaceHandler>();

        public Dictionary<int, VFlashHandler> VFlashHandlers = new Dictionary<int, VFlashHandler>();
        public Dictionary<int, VFlashTypeBank> VFlashTypeBanks = new Dictionary<int, VFlashTypeBank>();
        
        public Dictionary<int, OutputWriter> OutputWriters = new Dictionary<int, OutputWriter>();
        
        public abstract void AddPlcCommunicator();
        public abstract void AddCommunicationInterface();
        public abstract void AddOutputWriter();
        public abstract void AddVFlashChannel();

        public abstract void RemovePlcCommunicator(int id);
        public abstract void RemoveCommunicationInterface(int id);
        public abstract void RemoveOutputWriter(int id);
        public abstract void RemoveVFlashChannel(int id);
    }

    class Registry : RegistryBase
    {
        public override void AddPlcCommunicator()
        {
            PlcCommunicators.Add(PlcCommunicators.Count + 1, new PlcCommunicator());
        }

        public override void AddCommunicationInterface()
        {
            throw new NotImplementedException();
        }

        public override void AddOutputWriter()
        {
            throw new NotImplementedException();
        }

        public override void AddVFlashChannel()
        {
            throw new NotImplementedException();
        }

        public override void RemovePlcCommunicator(int id)
        {
            throw new NotImplementedException();
        }

        public override void RemoveCommunicationInterface(int id)
        {
            throw new NotImplementedException();
        }

        public override void RemoveOutputWriter(int id)
        {
            throw new NotImplementedException();
        }

        public override void RemoveVFlashChannel(int id)
        {
            throw new NotImplementedException();
        }
    }
}
