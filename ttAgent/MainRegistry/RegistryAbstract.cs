using System;

namespace _ttAgent.MainRegistry
{
    abstract class RegistryAbstract
    {
        public RegistryComposite PlcCommunicators = new RegistryComposite(0, "");
        public RegistryComposite PlcGuiCommunicationStatuses = new RegistryComposite(0, "");
        public RegistryComposite PlcGuiCommunicationStatusBars = new RegistryComposite(0, "");
        public RegistryComposite PlcGuiConfigurations = new RegistryComposite(0, "");

        public RegistryComposite CommunicationInterfaceHandlers = new RegistryComposite(0, "");
        public RegistryComposite GuiComInterfacemunicationConfigurations = new RegistryComposite(0, "");
        public RegistryComposite GuiCommunicationInterfaceOnlines = new RegistryComposite(0, "");

        public RegistryComposite OutputHandlers = new RegistryComposite(0, "");
        public RegistryComposite GuiOutputCreators = new RegistryComposite(0, "");

        public RegistryComposite VFlashTypeBanks = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashPathBanks = new RegistryComposite(0, "");

        public RegistryComposite VFlashHandlers = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashes = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashStatusBars = new RegistryComposite(0, "");

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