using System;
using System.Collections.Generic;
using System.Windows;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Output;
using _3880_80_FlashStation.PLC;
using _3880_80_FlashStation.Vector;
using _3880_80_FlashStation.Visual.Gui;

namespace _3880_80_FlashStation.MainRegistry
{
    abstract class RegistryBase
    {
        public Dictionary<uint, PlcCommunicator> PlcCommunicators = new Dictionary<uint, PlcCommunicator>();
        public Dictionary<uint, GuiCommunicationStatus> PlcGuiCommunicationStatuses = new Dictionary<uint, GuiCommunicationStatus>();
        public Dictionary<uint, GuiCommunicationStatusBar> PlcGuiCommunicationStatusBars = new Dictionary<uint, GuiCommunicationStatusBar>();
        public Dictionary<uint, GuiPlcConfiguration> PlcGuiConfigurations = new Dictionary<uint, GuiPlcConfiguration>(); 

        public Dictionary<uint, CommunicationInterfaceHandler> CommunicationInterfaceHandlers = new Dictionary<uint, CommunicationInterfaceHandler>();
        public Dictionary<uint, GuiComInterfacemunicationConfiguration> GuiComInterfacemunicationConfigurations = new Dictionary<uint, GuiComInterfacemunicationConfiguration>();

        public Dictionary<uint, VFlashHandler> VFlashHandlers = new Dictionary<uint, VFlashHandler>();
        public Dictionary<uint, VFlashTypeBank> VFlashTypeBanks = new Dictionary<uint, VFlashTypeBank>();
        
        public Dictionary<uint, OutputWriter> OutputWriters = new Dictionary<uint, OutputWriter>();
        
        public abstract uint AddPlcCommunicator();
        public abstract uint AddCommunicationInterface();
        public abstract uint AddOutputWriter();
        public abstract uint AddVFlashChannel();

        public abstract void RemovePlcCommunicator(int id);
        public abstract void RemoveCommunicationInterface(int id);
        public abstract void RemoveOutputWriter(int id);
        public abstract void RemoveVFlashChannel(int id);
    }

    class Registry : RegistryBase
    {
        public override uint AddPlcCommunicator()
        {
            var id = (uint) PlcCommunicators.Count + 1;
            PlcCommunicators.Add(id, new PlcCommunicator());
            PlcGuiCommunicationStatuses.Add(id, new GuiCommunicationStatus(id, PlcCommunicators[id], PlcConfigurationFile.Default));
            PlcGuiCommunicationStatusBars.Add(id, new GuiCommunicationStatusBar(id, PlcCommunicators[id]));
            PlcGuiConfigurations.Add(id, new GuiPlcConfiguration(id, PlcCommunicators[id],  PlcConfigurationFile.Default));
            return id;
        }

        public override uint AddCommunicationInterface()
        {
            var id = (uint)PlcCommunicators.Count + 1;
            CommunicationInterfaceHandlers.Add(id, new CommunicationInterfaceHandler(id, CommunicationInterfacePath.Default));
            GuiComInterfacemunicationConfigurations.Add(id, new GuiComInterfacemunicationConfiguration(id, CommunicationInterfaceHandlers[id], CommunicationInterfacePath.Default));
            return id;
        }

        public override uint AddOutputWriter()
        {
            throw new NotImplementedException();
        }

        public override uint AddVFlashChannel()
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
