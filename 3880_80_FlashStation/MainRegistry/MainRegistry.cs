using System;
using System.Collections.Generic;
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

        public Dictionary<uint, OutputWriter> OutputWriters = new Dictionary<uint, OutputWriter>();
        public Dictionary<uint, GuiOutputCreator> GuiOutputCreators = new Dictionary<uint, GuiOutputCreator>();

        public Dictionary<uint, VFlashTypeBank> VFlashTypeBanks = new Dictionary<uint, VFlashTypeBank>();
        public Dictionary<uint, GuiVFlashPathBank> GuiVFlashPathBanks = new Dictionary<uint, GuiVFlashPathBank>();

        public Dictionary<uint, VFlashHandler> VFlashHandlers = new Dictionary<uint, VFlashHandler>();
        public Dictionary<uint, GuiVFlash> GuiVFlashes = new Dictionary<uint, GuiVFlash>();
        public Dictionary<uint, GuiVFlashStatusBar> GuiVFlashStatusBars = new Dictionary<uint, GuiVFlashStatusBar>(); 
        
        public abstract uint AddPlcCommunicator();
        public abstract uint AddCommunicationInterface();
        public abstract uint AddOutputWriter();
        public abstract uint AddVFlashBank();
        public abstract uint AddVFlashChannel();

        public abstract void RemovePlcCommunicator(uint id);
        public abstract void RemoveCommunicationInterface(uint id);
        public abstract void RemoveOutputWriter(uint id);
        public abstract void RemoveVFlashBank(uint id);
        public abstract void RemoveVFlashChannel(uint id);
    }

    class Registry : RegistryBase
    {
        public override uint AddPlcCommunicator()
        {
            var id = (uint)PlcCommunicators.Count + 1;
            PlcCommunicators.Add(id, new PlcCommunicator(id, PlcConfigurationFile.Default));
            PlcGuiCommunicationStatuses.Add(id, new GuiCommunicationStatus(id, PlcCommunicators[id], PlcConfigurationFile.Default));
            PlcGuiCommunicationStatusBars.Add(id, new GuiCommunicationStatusBar(id, PlcCommunicators[id]));
            PlcGuiConfigurations.Add(id, new GuiPlcConfiguration(id, PlcCommunicators[id],  PlcConfigurationFile.Default));
            return id;
        }

        public override uint AddCommunicationInterface()
        {
            var id = (uint)CommunicationInterfaceHandlers.Count + 1;
            CommunicationInterfaceHandlers.Add(id, new CommunicationInterfaceHandler(id, CommunicationInterfacePath.Default));
            GuiComInterfacemunicationConfigurations.Add(id, new GuiComInterfacemunicationConfiguration(id, CommunicationInterfaceHandlers[id], CommunicationInterfacePath.Default));
            return id;
        }

        public override uint AddOutputWriter()
        {
            var id = (uint)OutputWriters.Count + 1;
            OutputWriters.Add(id, null);
            GuiOutputCreators.Add(id, new GuiOutputCreator(CommunicationInterfaceHandlers[id], OutputCreatorFile.Default)); //todo connection with interface
            return id;
        }

        public override uint AddVFlashBank()
        {
            var id = (uint)VFlashTypeBanks.Count + 1;
            VFlashTypeBanks.Add(id, new VFlashTypeBank());
            GuiVFlashPathBanks.Add(id, new GuiVFlashPathBank(id, VFlashTypeBankFile.Default, VFlashTypeBanks[id]));
            return id;
        }

        public override uint AddVFlashChannel()
        {
            var id = (uint)VFlashHandlers.Count + 1;
            VFlashHandlers.Add(id, new VFlashHandler(id, CommunicationInterfaceHandlers[id].ReadInterfaceComposite, CommunicationInterfaceHandlers[id].WriteInterfaceComposite)); //todo connection with interface
            GuiVFlashes.Add(id, new GuiVFlash(id, VFlashHandlers[id]));
            GuiVFlashStatusBars.Add(id, new GuiVFlashStatusBar(id, VFlashHandlers[id]));
            return id;
        }

        public override void RemovePlcCommunicator(uint id)
        {
            throw new NotImplementedException();
        }

        public override void RemoveCommunicationInterface(uint id)
        {
            throw new NotImplementedException();
        }

        public override void RemoveOutputWriter(uint id)
        {
            throw new NotImplementedException();
        }

        public override void RemoveVFlashBank(uint id)
        {
            throw new NotImplementedException();
        }

        public override void RemoveVFlashChannel(uint id)
        {
            throw new NotImplementedException();
        }
    }
}
