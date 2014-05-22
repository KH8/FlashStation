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
        public Dictionary<int, PlcCommunicator> PlcCommunicators;
        public Dictionary<int, PlcConfigurator> PlcConfigurators;
        public Dictionary<int, PlcConfigurationFile> PlcConfigurationFiles;
        public Dictionary<int, PlcStartUpConnection> PlcStartUpConnections;

        public Dictionary<int, CommunicationInterfaceHandler> CommunicationInterfaceHandlers;
        public Dictionary<int, CommunicationInterfacePath> CommunicationInterfacePaths;

        public Dictionary<int, VFlashHandler> VFlashHandlers;
        public Dictionary<int, VFlashTypeBank> VFlashTypeBanks;
        public Dictionary<int, VFlashTypeBankFile> VFlashTypeBankFiles;

        public Dictionary<int, OutputWriter> OutputWriters;
        public Dictionary<int, OutputCreatorFile> OutputCreatorFiles;

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
            PlcCommunicators.Add(1, new PlcCommunicator());
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
