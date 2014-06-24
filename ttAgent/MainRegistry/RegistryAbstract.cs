using System;
using System.Collections.Generic;
using _ttAgent.DataAquisition;
using _ttAgent.Output;
using _ttAgent.PLC;
using _ttAgent.Vector;
using _ttAgent.Visual.Gui;

namespace _ttAgent.MainRegistry
{
    abstract class RegistryAbstract
    {
        public Dictionary<uint, PlcCommunicator> PlcCommunicators = new Dictionary<uint, PlcCommunicator>();
        public Dictionary<uint, GuiCommunicationStatus> PlcGuiCommunicationStatuses = new Dictionary<uint, GuiCommunicationStatus>();
        public Dictionary<uint, GuiCommunicationStatusBar> PlcGuiCommunicationStatusBars = new Dictionary<uint, GuiCommunicationStatusBar>();
        public Dictionary<uint, GuiPlcConfiguration> PlcGuiConfigurations = new Dictionary<uint, GuiPlcConfiguration>(); 

        public Dictionary<uint, CommunicationInterfaceHandler> CommunicationInterfaceHandlers = new Dictionary<uint, CommunicationInterfaceHandler>();
        public Dictionary<uint, GuiComInterfacemunicationConfiguration> GuiComInterfacemunicationConfigurations = new Dictionary<uint, GuiComInterfacemunicationConfiguration>();
        public Dictionary<uint, GuiCommunicationInterfaceOnline> GuiCommunicationInterfaceOnlines = new Dictionary<uint, GuiCommunicationInterfaceOnline>();

        public Dictionary<uint, OutputHandler> OutputHandlers = new Dictionary<uint, OutputHandler>();
        public Dictionary<uint, GuiOutputHandler> GuiOutputCreators = new Dictionary<uint, GuiOutputHandler>();

        public Dictionary<uint, VFlashTypeBank> VFlashTypeBanks = new Dictionary<uint, VFlashTypeBank>();
        public Dictionary<uint, GuiVFlashPathBank> GuiVFlashPathBanks = new Dictionary<uint, GuiVFlashPathBank>();

        public Dictionary<uint, VFlashHandler> VFlashHandlers = new Dictionary<uint, VFlashHandler>();
        public Dictionary<uint, GuiVFlash> GuiVFlashes = new Dictionary<uint, GuiVFlash>();
        public Dictionary<uint, GuiVFlashStatusBar> GuiVFlashStatusBars = new Dictionary<uint, GuiVFlashStatusBar>(); 

        public Dictionary<uint, Tuple<uint, uint>> CommunicationInterfaceHandlersAssignemenTuples = new Dictionary<uint, Tuple<uint, uint>>();
        public Dictionary<uint, Tuple<uint, uint>> OutputHandlersAssignemenTuples = new Dictionary<uint, Tuple<uint, uint>>();
        public Dictionary<uint, Tuple<uint, uint, uint>> VFlashHandlersAssignemenTuples = new Dictionary<uint, Tuple<uint, uint, uint>>();

        public abstract uint AddPlcCommunicator();
        public abstract uint AddCommunicationInterface(uint plcConnectionId);
        public abstract uint AddOutputHandler(uint communicationInterfaceId);
        public abstract uint AddVFlashBank();
        public abstract uint AddVFlashChannel(uint communicationInterfaceId, uint vFlashBankId);

        public abstract uint AddPlcCommunicator(Boolean save);
        public abstract uint AddCommunicationInterface(Boolean save, uint plcConnectionId);
        public abstract uint AddOutputHandler(Boolean save, uint communicationInterfaceId);
        public abstract uint AddVFlashBank(Boolean save);
        public abstract uint AddVFlashChannel(Boolean save, uint communicationInterfaceId, uint vFlashBankId);

        public abstract uint AddPlcCommunicator(uint id);
        public abstract uint AddCommunicationInterface(uint id, uint plcConnectionId);
        public abstract uint AddOutputHandler(uint id, uint communicationInterfaceId);
        public abstract uint AddVFlashBank(uint id);
        public abstract uint AddVFlashChannel(uint id, uint communicationInterfaceId, uint vFlashBankId);

        public abstract void RemovePlcCommunicator(uint id);
        public abstract void RemoveCommunicationInterface(uint id);
        public abstract void RemoveOutputHandler(uint id);
        public abstract void RemoveVFlashBank(uint id);
        public abstract void RemoveVFlashChannel(uint id);

        public abstract void RemoveAll();
    }
}