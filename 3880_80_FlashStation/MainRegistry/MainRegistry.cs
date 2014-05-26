using System;
using System.Collections.Generic;
using _3880_80_FlashStation.Configuration;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Output;
using _3880_80_FlashStation.PLC;
using _3880_80_FlashStation.Vector;
using _3880_80_FlashStation.Visual.Gui;

namespace _3880_80_FlashStation.MainRegistry
{
    abstract class RegistryBase
    {
        public Dictionary<int, PlcCommunicator> PlcCommunicators = new Dictionary<int, PlcCommunicator>();
        public Dictionary<int, GuiCommunicationStatus> PlcGuiCommunicationStatuses = new Dictionary<int, GuiCommunicationStatus>();
        public Dictionary<int, GuiCommunicationStatusBar> PlcGuiCommunicationStatusBars = new Dictionary<int, GuiCommunicationStatusBar>();
        public Dictionary<int, GuiPlcConfiguration> PlcGuiConfigurations = new Dictionary<int, GuiPlcConfiguration>(); 

        public Dictionary<int, CommunicationInterfaceHandler> CommunicationInterfaceHandlers = new Dictionary<int, CommunicationInterfaceHandler>();

        public Dictionary<int, VFlashHandler> VFlashHandlers = new Dictionary<int, VFlashHandler>();
        public Dictionary<int, VFlashTypeBank> VFlashTypeBanks = new Dictionary<int, VFlashTypeBank>();
        
        public Dictionary<int, OutputWriter> OutputWriters = new Dictionary<int, OutputWriter>();
        
        public abstract int AddPlcCommunicator();
        public abstract int AddCommunicationInterface();
        public abstract int AddOutputWriter();
        public abstract int AddVFlashChannel();

        public abstract void RemovePlcCommunicator(int id);
        public abstract void RemoveCommunicationInterface(int id);
        public abstract void RemoveOutputWriter(int id);
        public abstract void RemoveVFlashChannel(int id);
    }

    class Registry : RegistryBase
    {
        public override int AddPlcCommunicator()
        {
            var id = PlcCommunicators.Count + 1;
            PlcCommunicators.Add(id, new PlcCommunicator());
            PlcGuiCommunicationStatuses.Add(id, new GuiCommunicationStatus(PlcCommunicators[id], PlcStartUpConnection.Default)); //todo
            PlcGuiCommunicationStatusBars.Add(id, new GuiCommunicationStatusBar(PlcCommunicators[id]));
            PlcGuiConfigurations.Add(id, new GuiPlcConfiguration(PlcCommunicators[id],  PlcConfigurationFile.Default)); //todo
            return id;
        }

        public override int AddCommunicationInterface()
        {
            throw new NotImplementedException();
        }

        public override int AddOutputWriter()
        {
            throw new NotImplementedException();
        }

        public override int AddVFlashChannel()
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
