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
        public RegistryComposite GuiOutputHandlerComponents = new RegistryComposite(0, "");
        public RegistryComposite GuiOutputHandlerInterfaceAssignmentComponents = new RegistryComposite(0, "");

        public RegistryComposite VFlashTypeBanks = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashPathBanks = new RegistryComposite(0, "");

        public RegistryComposite VFlashHandlers = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashHandlerComponents = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashStatusBars = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashHandlerInterfaceAssignmentComponents = new RegistryComposite(0, "");

        public RegistryComposite Analyzers = new RegistryComposite(0, "");
        public RegistryComposite GuiAnalyzers = new RegistryComposite(0, "");
        public RegistryComposite GuiAnalyzerMainFrames = new RegistryComposite(0, "");
        public RegistryComposite GuiAnalyzerInterfaceAssignmentComponents = new RegistryComposite(0, "");

        public abstract uint AddPlcCommunicator();
        public abstract uint AddCommunicationInterface(uint plcConnectionId);
        public abstract uint AddOutputHandler(uint communicationInterfaceId);
        public abstract uint AddVFlashBank();
        public abstract uint AddVFlashChannel(uint communicationInterfaceId, uint vFlashBankId);
        public abstract uint AddAnalyzer(uint communicationInterfaceId);

        public abstract uint AddPlcCommunicator(Boolean save);
        public abstract uint AddCommunicationInterface(Boolean save, uint plcConnectionId);
        public abstract uint AddOutputHandler(Boolean save, uint communicationInterfaceId);
        public abstract uint AddVFlashBank(Boolean save);
        public abstract uint AddVFlashChannel(Boolean save, uint communicationInterfaceId, uint vFlashBankId);
        public abstract uint AddAnalyzer(Boolean save, uint communicationInterfaceId);

        public abstract uint AddPlcCommunicator(uint id);
        public abstract uint AddCommunicationInterface(uint id, uint plcConnectionId);
        public abstract uint AddOutputHandler(uint id, uint communicationInterfaceId);
        public abstract uint AddVFlashBank(uint id);
        public abstract uint AddVFlashChannel(uint id, uint communicationInterfaceId, uint vFlashBankId);
        public abstract uint AddAnalyzer(uint id, uint communicationInterfaceId);

        public abstract void RemoveComponent(RegistryComponent component);

        public abstract void RemoveAll();
    }
}