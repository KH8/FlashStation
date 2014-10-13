using System;

namespace _PlcAgent.MainRegistry
{
    abstract class RegistryAbstract
    {
        public RegistryComposite PlcCommunicators = new RegistryComposite(0, "Plc Connections");
        public RegistryComposite GuiPlcCommunicatorStatuses = new RegistryComposite(0, "");
        public RegistryComposite GuiPlcCommunicatorStatusBars = new RegistryComposite(0, "");
        public RegistryComposite GuiPlcCommunicatorConfigurations = new RegistryComposite(0, "");

        public RegistryComposite CommunicationInterfaceHandlers = new RegistryComposite(0, "Communication Interfaces");
        public RegistryComposite GuiComInterfacemunicationConfigurations = new RegistryComposite(0, "");
        public RegistryComposite GuiCommunicationInterfaceOnlines = new RegistryComposite(0, "");

        public RegistryComposite OutputDataTemplates = new RegistryComposite(0, "Output Data Templates");
        public RegistryComposite GuiOutputDataTemplates = new RegistryComposite(0, "");

        public RegistryComposite OutputFileCreators = new RegistryComposite(0, "Output File Creators");
        public RegistryComposite GuiOutputFileCreatorComponents = new RegistryComposite(0, "");
        public RegistryComposite GuiOutputFileCreatorInterfaceAssignmentComponents = new RegistryComposite(0, "");

        public RegistryComposite OutputHandlers = new RegistryComposite(0, "Output Handlers");
        public RegistryComposite GuiOutputHandlerComponents = new RegistryComposite(0, "");
        public RegistryComposite GuiOutputHandlerInterfaceAssignmentComponents = new RegistryComposite(0, "");

        public RegistryComposite VFlashTypeBanks = new RegistryComposite(0, "vFlash Type Banks");
        public RegistryComposite GuiVFlashPathBanks = new RegistryComposite(0, "");

        public RegistryComposite VFlashHandlers = new RegistryComposite(0, "vFlash Handlers");
        public RegistryComposite GuiVFlashHandlerComponents = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashStatusBars = new RegistryComposite(0, "");
        public RegistryComposite GuiVFlashHandlerInterfaceAssignmentComponents = new RegistryComposite(0, "");

        public RegistryComposite Analyzers = new RegistryComposite(0, "Analyzers");
        public RegistryComposite GuiAnalyzerConfigurations = new RegistryComposite(0, "");
        public RegistryComposite GuiAnalyzerControls = new RegistryComposite(0, "");
        public RegistryComposite GuiAnalyzerMainFrames = new RegistryComposite(0, "");
        public RegistryComposite GuiAnalyzerInterfaceAssignmentComponents = new RegistryComposite(0, "");
        public RegistryComposite GuiAnalyzerDataCursorTables = new RegistryComposite(0, "");

        public RegistryComposite Modules = new RegistryComposite(0, "Modules");

        public delegate void RegistryUpdated();
        public RegistryUpdated OnRegistryUpdated;

        public abstract uint AddPlcCommunicator();
        public abstract uint AddCommunicationInterface(uint plcConnectionId);
        public abstract uint AddOutputDataTemplate();
        public abstract uint AddOutputFileCreator(uint communicationInterfaceId, uint templateId);
        public abstract uint AddOutputHandler(uint communicationInterfaceId);
        public abstract uint AddVFlashBank();
        public abstract uint AddVFlashChannel(uint communicationInterfaceId, uint vFlashBankId);
        public abstract uint AddAnalyzer(uint communicationInterfaceId);

        public abstract uint AddPlcCommunicator(Boolean save);
        public abstract uint AddCommunicationInterface(Boolean save, uint plcConnectionId);
        public abstract uint AddOutputDataTemplate(Boolean save);
        public abstract uint AddOutputFileCreator(Boolean save, uint communicationInterfaceId, uint templateId);
        public abstract uint AddOutputHandler(Boolean save, uint communicationInterfaceId);
        public abstract uint AddVFlashBank(Boolean save);
        public abstract uint AddVFlashChannel(Boolean save, uint communicationInterfaceId, uint vFlashBankId);
        public abstract uint AddAnalyzer(Boolean save, uint communicationInterfaceId);

        public abstract uint AddPlcCommunicator(uint id);
        public abstract uint AddCommunicationInterface(uint id, uint plcConnectionId);
        public abstract uint AddOutputDataTemplate(uint id);
        public abstract uint AddOutputFileCreator(uint id, uint communicationInterfaceId, uint templateId);
        public abstract uint AddOutputHandler(uint id, uint communicationInterfaceId);
        public abstract uint AddVFlashBank(uint id);
        public abstract uint AddVFlashChannel(uint id, uint communicationInterfaceId, uint vFlashBankId);
        public abstract uint AddAnalyzer(uint id, uint communicationInterfaceId);

        public abstract void RemoveComponent(RegistryComponent component);

        public abstract void RemoveAll();

        protected RegistryAbstract()
        {
            Modules.Add(PlcCommunicators);
            Modules.Add(CommunicationInterfaceHandlers);
            Modules.Add(OutputDataTemplates);
            Modules.Add(OutputFileCreators);
            Modules.Add(OutputHandlers);
            Modules.Add(VFlashTypeBanks);
            Modules.Add(VFlashHandlers);
            Modules.Add(Analyzers);
        }
    }
}