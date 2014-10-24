using System;
using System.Linq;
using System.Windows;
using _PlcAgent.Analyzer;
using _PlcAgent.DataAquisition;
using _PlcAgent.DB;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.Output;
using _PlcAgent.Output.OutputFileCreator;
using _PlcAgent.Output.OutputHandler;
using _PlcAgent.Output.Template;
using _PlcAgent.PLC;
using _PlcAgent.Vector;
using _PlcAgent.Visual.Gui;
using _PlcAgent.Visual.Gui.Analyzer;
using _PlcAgent.Visual.Gui.DataAquisition;
using _PlcAgent.Visual.Gui.DB;
using _PlcAgent.Visual.Gui.Output;
using _PlcAgent.Visual.Gui.PLC;
using _PlcAgent.Visual.Gui.Vector;

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

        public RegistryComposite DbConnectionHandlers = new RegistryComposite(0, "DB Connection Handlers");
        public RegistryComposite GuiDbConnectionHandlers = new RegistryComposite(0, "");
        public RegistryComposite GuiDbConnectionHandlerInterfaceAssignmentComponents = new RegistryComposite(0, "");
        public RegistryComposite GuiDbStoredProcedures = new RegistryComposite(0, "");

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
        public abstract uint AddDbConnectionHandler(uint communicationInterfaceId);
        public abstract uint AddVFlashBank();
        public abstract uint AddVFlashChannel(uint communicationInterfaceId, uint vFlashBankId);
        public abstract uint AddAnalyzer(uint communicationInterfaceId);

        public abstract uint AddPlcCommunicator(Boolean save);
        public abstract uint AddCommunicationInterface(Boolean save, uint plcConnectionId);
        public abstract uint AddOutputDataTemplate(Boolean save);
        public abstract uint AddOutputFileCreator(Boolean save, uint communicationInterfaceId, uint templateId);
        public abstract uint AddOutputHandler(Boolean save, uint communicationInterfaceId);
        public abstract uint AddDbConnectionHandler(Boolean save, uint communicationInterfaceId);
        public abstract uint AddVFlashBank(Boolean save);
        public abstract uint AddVFlashChannel(Boolean save, uint communicationInterfaceId, uint vFlashBankId);
        public abstract uint AddAnalyzer(Boolean save, uint communicationInterfaceId);

        public abstract uint AddPlcCommunicator(uint id);
        public abstract uint AddCommunicationInterface(uint id, uint plcConnectionId);
        public abstract uint AddOutputDataTemplate(uint id);
        public abstract uint AddOutputFileCreator(uint id, uint communicationInterfaceId, uint templateId);
        public abstract uint AddOutputHandler(uint id, uint communicationInterfaceId);
        public abstract uint AddDbConnectionHandler(uint id, uint communicationInterfaceId);
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
            Modules.Add(DbConnectionHandlers);
            Modules.Add(VFlashTypeBanks);
            Modules.Add(VFlashHandlers);
            Modules.Add(Analyzers);
        }
    }

    class Registry : RegistryAbstract
    {
        public void Initialize()
        {
            if (MainRegistryFile.Default.PlcCommunicators == null) return;
            if (MainRegistryFile.Default.CommunicationInterfaceHandlers == null) return;
            if (MainRegistryFile.Default.OutputDataTemplates == null) return;
            if (MainRegistryFile.Default.OutputFileCreators == null) return;
            if (MainRegistryFile.Default.OutputHandlers == null) return;
            if (MainRegistryFile.Default.DbConnectionHandlers == null) return;
            if (MainRegistryFile.Default.VFlashTypeBanks == null) return;
            if (MainRegistryFile.Default.VFlashHandlers == null) return;
            if (MainRegistryFile.Default.Analyzers == null) return;

            foreach (var plcCommunicator in MainRegistryFile.Default.PlcCommunicators.Where(plcCommunicator => plcCommunicator != null))
            {
                AddPlcCommunicator(plcCommunicator[0]);
            }
            foreach (var communicationInterfaceHandler in MainRegistryFile.Default.CommunicationInterfaceHandlers.Where(communicationInterfaceHandler => communicationInterfaceHandler != null))
            {
                AddCommunicationInterface(communicationInterfaceHandler[0], communicationInterfaceHandler[1]);
            }
            foreach (var outputDataTemplate in MainRegistryFile.Default.OutputDataTemplates.Where(outputDataTemplate => outputDataTemplate != null))
            {
                AddOutputDataTemplate(outputDataTemplate[0]);
            }
            foreach (var outputFileCreator in MainRegistryFile.Default.OutputFileCreators.Where(outputFileCreator => outputFileCreator != null))
            {
                AddOutputFileCreator(outputFileCreator[0], outputFileCreator[2], outputFileCreator[4]);
            }
            foreach (var outputHandler in MainRegistryFile.Default.OutputHandlers.Where(outputHandler => outputHandler != null))
            {
                AddOutputHandler(outputHandler[0], outputHandler[2]);
            }
            foreach (var dbConnectionHandler in MainRegistryFile.Default.DbConnectionHandlers.Where(dbConnectionHandler => dbConnectionHandler != null))
            {
                AddDbConnectionHandler(dbConnectionHandler[0], dbConnectionHandler[2]);
            }
            foreach (var vFlashTypeBank in MainRegistryFile.Default.VFlashTypeBanks.Where(vFlashTypeBank => vFlashTypeBank != null))
            {
                AddVFlashBank(vFlashTypeBank[0]);
            }
            foreach (var vFlashHandler in MainRegistryFile.Default.VFlashHandlers.Where(vFlashHandler => vFlashHandler != null))
            {
                AddVFlashChannel(vFlashHandler[0], vFlashHandler[2], vFlashHandler[3]);
            }
            foreach (var analyzer in MainRegistryFile.Default.Analyzers.Where(analyzer => analyzer != null))
            {
                AddAnalyzer(analyzer[0], analyzer[2]);
            }

            UpdateRegistry();
            Logger.Log("Registry content initialized");
        }

        #region Plc Communicator

        public override uint AddPlcCommunicator(Boolean save)
        {
            var id = AddPlcCommunicator();
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddPlcCommunicator()
        {
            var id = PlcCommunicators.GetFirstNotUsed();
            return AddPlcCommunicator(id);
        }

        public override uint AddPlcCommunicator(uint id)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of Plc Communication \ncomponents exceeded", "Component Creation Failed");
                return 0;
            }

            PlcCommunicator component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the PLC Connection Component");
                PlcCommunicators.Add(
                    new PlcCommunicator(id, "PLC__" + id,
                        PlcConfigurationFile.Default));
                Logger.Log("ID: " + id + " Initialization of the PLC Connection");
                component = (PlcCommunicator) PlcCommunicators.ReturnComponent(id);
                component.InitializeConnection();
            }
            catch (Exception)
            {
                PlcCommunicators.Remove(PlcCommunicators.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new PLC Connection failed");
                return 0;
            }

            GuiPlcCommunicatorStatuses.Add(new GuiComponent(id, "", new GuiPlcCommunicatorStatus(component)));
            GuiPlcCommunicatorStatusBars.Add(new GuiComponent(id, "", new GuiPlcCommunicatorStatusBar(component)));
            GuiPlcCommunicatorConfigurations.Add(new GuiComponent(id, "", new GuiPlcCommunicatorConfiguration(component)));

            Logger.Log("ID: " + id + " new PLC Connection have been created");
            return id;
        }

        #endregion


        #region Communication Interface

        public override uint AddCommunicationInterface(Boolean save, uint plcConnectionId)
        {
            var id = AddCommunicationInterface(plcConnectionId);
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddCommunicationInterface(uint plcConnectionId)
        {
            var id = CommunicationInterfaceHandlers.GetFirstNotUsed();
            return AddCommunicationInterface(id, plcConnectionId);
        }

        public override uint AddCommunicationInterface(uint id, uint plcConnectionId)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of Communicator Interface \ncomponents exceeded",
                    "Component Creation Failed");
                return 0;
            }

            CommunicationInterfaceHandler component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the Communication Interface Component");
                CommunicationInterfaceHandlers.Add(
                    new CommunicationInterfaceHandler(id, "INT__" + id,
                        (PlcCommunicator) PlcCommunicators.ReturnComponent(plcConnectionId),
                        CommunicationInterfacePath.Default));
                Logger.Log("ID: " + id + " Initialization of the Communication Interface");
                component = (CommunicationInterfaceHandler) CommunicationInterfaceHandlers.ReturnComponent(id);
                component.InitializeInterface();
            }
            catch (Exception)
            {
                CommunicationInterfaceHandlers.Remove(CommunicationInterfaceHandlers.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Communication Interface failed");
                return 0;
            }

            GuiComInterfacemunicationConfigurations.Add(new GuiComponent(id, "",
                new GuiCommunicationInterfaceConfiguration(component)));
            GuiCommunicationInterfaceOnlines.Add(new GuiComponent(id, "",
                new GuiCommunicationInterfaceOnlineHierarchical(component)));

            Logger.Log("ID: " + id + " new Communication Interface have been created");
            return id;
        }

        #endregion


        #region Output Data Template

        public override uint AddOutputDataTemplate(Boolean save)
        {
            var id = AddOutputDataTemplate();
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddOutputDataTemplate()
        {
            var id = OutputDataTemplates.GetFirstNotUsed();
            return AddOutputDataTemplate(id);
        }

        public override uint AddOutputDataTemplate(uint id)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of Output Data Template \ncomponents exceeded",
                    "Component Creation Failed");
                return 0;
            }

            OutputDataTemplate component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the Output Data Template Component");
                OutputDataTemplates.Add(new OutputDataTemplate(id, "TEMPLATE__" + id, OutputDataTemplateFile.Default));
                component = (OutputDataTemplate) OutputDataTemplates.ReturnComponent(id);

                Logger.Log("ID: " + id + " Initialization of the Output Data Template");
                component.Initialize();
            }
            catch (Exception)
            {
                OutputDataTemplates.Remove(OutputDataTemplates.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Output Data Template failed");

                return 0;
            }

            GuiOutputDataTemplates.Add(new GuiComponent(id, "", new GuiOutputDataTemplate(component)));

            Logger.Log("ID: " + id + " new Output Data Template have been created");
            return id;
        }

        #endregion


        #region Output Data File Creator

        public override uint AddOutputFileCreator(Boolean save, uint communicationInterfaceId, uint templateId)
        {
            var id = AddOutputFileCreator(communicationInterfaceId, templateId);
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddOutputFileCreator(uint communicationInterfaceId, uint templateId)
        {
            var id = OutputFileCreators.GetFirstNotUsed();
            return AddOutputFileCreator(id, communicationInterfaceId, templateId);
        }

        public override uint AddOutputFileCreator(uint id, uint communicationInterfaceId, uint templateId)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of Output File Creator \ncomponents exceeded",
                    "Component Creation Failed");
                return 0;
            }

            OutputFileCreator component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the Output File Creator Component");
                OutputFileCreators.Add(
                    new OutputFileCreator(id, "OUT__" + id,
                        (CommunicationInterfaceHandler)
                            CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId),
                        (OutputDataTemplate) OutputDataTemplates.ReturnComponent(templateId),
                        OutputFileCreatorFile.Default,
                        OutputFileCreatorInterfaceAssignmentFile.Default));
                Logger.Log("ID: " + id + " Initialization of the Output File Creator");
                component = (OutputFileCreator) OutputFileCreators.ReturnComponent(id);
                component.Initialize();
            }
            catch (Exception)
            {
                OutputFileCreators.Remove(OutputFileCreators.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Output File Creator failed");
                return 0;
            }

            GuiOutputFileCreatorComponents.Add(new GuiComponent(id, "", new GuiOutputFileCreator(component)));
            GuiOutputFileCreatorInterfaceAssignmentComponents.Add(new GuiComponent(id, "",
                new GuiInterfaceAssignment(component)));

            Logger.Log("ID: " + id + " new Output File Creator have been created");
            return id;
        }

        #endregion


        #region Output Handler

        public override uint AddOutputHandler(Boolean save, uint communicationInterfaceId)
        {
            var id = AddOutputHandler(communicationInterfaceId);
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddOutputHandler(uint communicationInterfaceId)
        {
            var id = OutputHandlers.GetFirstNotUsed();
            return AddOutputHandler(id, communicationInterfaceId);
        }

        public override uint AddOutputHandler(uint id, uint communicationInterfaceId)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of Output Handler \ncomponents exceeded", "Component Creation Failed");
                return 0;
            }

            OutputHandler component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the Output Handler Component");
                OutputHandlers.Add(
                    new OutputHandler(id, "OUT__" + id,
                        (CommunicationInterfaceHandler)
                            CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId),
                        OutputHandlerFile.Default,
                        OutputHandlerInterfaceAssignmentFile.Default));
                Logger.Log("ID: " + id + " Initialization of the Output Handler");
                component = (OutputHandler)OutputHandlers.ReturnComponent(id);
                component.Initialize();
            }
            catch (Exception)
            {
                OutputHandlers.Remove(OutputHandlers.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Output Handler failed");
                return 0;
            }

            GuiOutputHandlerComponents.Add(new GuiComponent(id, "", new GuiOutputHandler(component)));
            GuiOutputHandlerInterfaceAssignmentComponents.Add(new GuiComponent(id, "",
                new GuiInterfaceAssignment(component)));

            Logger.Log("ID: " + id + " new Output Handler have been created");
            return id;
        }

        #endregion


        #region DB Connection Handler

        public override uint AddDbConnectionHandler(Boolean save, uint communicationInterfaceId)
        {
            var id = AddDbConnectionHandler(communicationInterfaceId);
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddDbConnectionHandler(uint communicationInterfaceId)
        {
            var id = DbConnectionHandlers.GetFirstNotUsed();
            return AddDbConnectionHandler(id, communicationInterfaceId);
        }

        public override uint AddDbConnectionHandler(uint id, uint communicationInterfaceId)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of DB Connection Handler \ncomponents exceeded", "Component Creation Failed");
                return 0;
            }

            DbConnectionHandler component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the DB Connection Handler Component");
                DbConnectionHandlers.Add(
                    new DbConnectionHandler(id, "DB__" + id,
                        (CommunicationInterfaceHandler)
                            CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId),
                        DbConnectionHandlerFile.Default,
                        DbConnectionHandlerInterfaceAssignmentFile.Default));
                Logger.Log("ID: " + id + " Initialization of the DB Connection Handler");
                component = (DbConnectionHandler) DbConnectionHandlers.ReturnComponent(id);
                component.Initialize();
            }
            catch (Exception)
            {
                DbConnectionHandlers.Remove(DbConnectionHandlers.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new DB Connection Handler failed");
                return 0;
            }

            GuiDbConnectionHandlers.Add(new GuiComponent(id, "", new GuiDbConnectionHandler(component)));
            GuiDbConnectionHandlerInterfaceAssignmentComponents.Add(new GuiComponent(id, "",
                new GuiInterfaceAssignment(component)));
            GuiDbStoredProcedures.Add(new GuiComponent(id, "", new GuiDbStoredProcedures(component)));

            Logger.Log("ID: " + id + " new DB Connection Handler have been created");
            return id;
        }

        #endregion


        #region vFlash Bank

        public override uint AddVFlashBank(Boolean save)
        {
            var id = AddVFlashBank();
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddVFlashBank()
        {
            var id = VFlashTypeBanks.GetFirstNotUsed();
            return AddVFlashBank(id);
        }

        public override uint AddVFlashBank(uint id)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of vFlash Bank \ncomponents exceeded", "Component Creation Failed");
                return 0;
            }

            Logger.Log("ID: " + id + " Creation of the vFlash Bank Component");
            VFlashTypeBanks.Add(new VFlashTypeBank(id, "VFLASH_BANK__" + id, VFlashTypeBankFile.Default));
            var component = (VFlashTypeBank) VFlashTypeBanks.ReturnComponent(id);

            GuiVFlashPathBanks.Add(new GuiComponent(id, "", new GuiVFlashPathBank(component)));

            Logger.Log("ID: " + id + " new vFlash Bank have been created");
            return id;
        }

        #endregion


        #region vFlash Channel

        public override uint AddVFlashChannel(Boolean save, uint communicationInterfaceId, uint vFlashBankId)
        {
            var id = AddVFlashChannel(communicationInterfaceId, vFlashBankId);
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddVFlashChannel(uint communicationInterfaceId, uint vFlashBankId)
        {
            var id = VFlashHandlers.GetFirstNotUsed();
            return AddVFlashChannel(id, communicationInterfaceId, vFlashBankId);
        }

        public override uint AddVFlashChannel(uint id, uint communicationInterfaceId, uint vFlashBankId)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of vFlash Handler \ncomponents exceeded", "Component Creation Failed");
                return 0;
            }

            VFlashHandler component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the vFlash Channel Component");
                VFlashHandlers.Add(
                    new VFlashHandler(id, "VFLASH__" + id,
                        (CommunicationInterfaceHandler)
                            CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId),
                        (VFlashTypeBank) VFlashTypeBanks.ReturnComponent(vFlashBankId),
                        VFlashHandlerInterfaceAssignmentFile.Default));
                Logger.Log("ID: " + id + " Initialization of the vFlash Channel");
                component = (VFlashHandler) VFlashHandlers.ReturnComponent(id);
                component.InitializeVFlash();
            }
            catch (Exception)
            {
                VFlashHandlers.Remove(VFlashHandlers.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new vFlash Channel failed");
                return 0;
            }

            GuiVFlashHandlerComponents.Add(new GuiComponent(id, "", new GuiVFlashHandler(component)));
            GuiVFlashStatusBars.Add(new GuiComponent(id, "", new GuiVFlashStatusBar(component)));
            GuiVFlashHandlerInterfaceAssignmentComponents.Add(new GuiComponent(id, "",
                new GuiInterfaceAssignment(component)));

            Logger.Log("ID: " + id + " new vFlash Channel have been created");
            return id;
        }

        #endregion


        #region Analyzer

        public override uint AddAnalyzer(bool save, uint communicationInterfaceId)
        {
            var id = AddAnalyzer(communicationInterfaceId);
            if (save && id != 0) UpdateRegistry();
            return id;
        }

        public override uint AddAnalyzer(uint communicationInterfaceId)
        {
            var id = Analyzers.GetFirstNotUsed();
            return AddAnalyzer(id, communicationInterfaceId);
        }

        public override uint AddAnalyzer(uint id, uint communicationInterfaceId)
        {
            if (id > 8)
            {
                MessageBox.Show("Maximum number of Analyzer \ncomponents exceeded", "Component Creation Failed");
                return 0;
            }

            Analyzer.Analyzer component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the Analyzer Component");
                Analyzers.Add(
                    new Analyzer.Analyzer(id, "ANALYZER__" + id,
                        (CommunicationInterfaceHandler)
                            CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId),
                        AnalyzerAssignmentFile.Default, AnalyzerSetupFile.Default));
                Logger.Log("ID: " + id + " Initialization of the Analyzer");
                component = (Analyzer.Analyzer) Analyzers.ReturnComponent(id);
                component.Initialize();
            }
            catch (Exception)
            {
                Analyzers.Remove(Analyzers.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Analyzer failed");
                return 0;
            }

            GuiAnalyzerConfigurations.Add(new GuiComponent(id, "", new GuiAnalyzerConfiguration(component)));
            GuiAnalyzerControls.Add(new GuiComponent(id, "", new GuiAnalyzerControl(component)));
            GuiAnalyzerMainFrames.Add(new GuiComponent(id, "", new GuiAnalyzerMainFrame(component)));
            GuiAnalyzerInterfaceAssignmentComponents.Add(new GuiComponent(id, "", new GuiInterfaceAssignment(component)));
            GuiAnalyzerDataCursorTables.Add((new GuiComponent(id, "", new GuiAnalyzerDataCursorTable(component))));

            Logger.Log("ID: " + id + " new Analyzer have been created");
            return id;
        }

        #endregion
        

        public override void RemoveComponent(RegistryComponent component)
        {
            if (PlcCommunicators.Cast<object>().Any(plcCommunicator => component == plcCommunicator))
            {
                CheckAssignment(component, component.ReferencePosition);
                foreach (var plcGuiCommunicationStatusBar in GuiPlcCommunicatorStatusBars.Cast<GuiComponent>().Where(plcGuiCommunicationStatusBar => plcGuiCommunicationStatusBar.Header.Id == component.Header.Id))
                {
                    GuiPlcCommunicatorStatusBars.Children.Remove(plcGuiCommunicationStatusBar);
                    break;
                }
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var plcCommunicator = (PlcCommunicator)component;
                plcCommunicator.Deinitialize();

                PlcCommunicators.Children.Remove(component);
            }
            if (CommunicationInterfaceHandlers.Cast<object>().Any(communicationInterfaceHandler => component == communicationInterfaceHandler))
            {
                CheckAssignment(component, component.ReferencePosition);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var communicationInterfaceHandler = (CommunicationInterfaceHandler)component;
                communicationInterfaceHandler.Deinitialize();

                CommunicationInterfaceHandlers.Children.Remove(component);
            }
            if (OutputDataTemplates.Cast<object>().Any(outputDataTemplate => component == outputDataTemplate))
            {
                CheckAssignment(component, component.ReferencePosition);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var outputDataTemplate = (OutputDataTemplate)component;
                outputDataTemplate.Deinitialize();

                OutputDataTemplates.Children.Remove(component);
            }
            if (OutputFileCreators.Cast<object>().Any(outputFileCreator => component == outputFileCreator))
            {
                CheckAssignment(component, component.ReferencePosition);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var outputFileCreator = (OutputFileCreator)component;
                outputFileCreator.Deinitialize();

                OutputFileCreators.Children.Remove(component);
            }
            if (OutputHandlers.Cast<object>().Any(outputHandler => component == outputHandler))
            {
                CheckAssignment(component, component.ReferencePosition);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var outputHandler = (OutputHandler)component;
                outputHandler.Deinitialize();

                OutputHandlers.Children.Remove(component);
            }
            if (DbConnectionHandlers.Cast<object>().Any(dbCommunicationHandler => component == dbCommunicationHandler))
            {
                CheckAssignment(component, component.ReferencePosition);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var dbCommunicationHandler = (OutputHandler)component;
                dbCommunicationHandler.Deinitialize();

                DbConnectionHandlers.Children.Remove(component);
            }
            if (VFlashTypeBanks.Cast<object>().Any(vFlashTypeBank => component == vFlashTypeBank))
            {
                CheckAssignment(component, component.ReferencePosition);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var vFlashTypeBank = (VFlashTypeBank)component;
                vFlashTypeBank.Deinitialize();

                VFlashTypeBanks.Children.Remove(component);
            }
            if (VFlashHandlers.Cast<object>().Any(vFlashHandler => component == vFlashHandler))
            {
                CheckAssignment(component, component.ReferencePosition);
                foreach (var guiVFlashStatusBar in GuiVFlashStatusBars.Cast<GuiComponent>().Where(guiVFlashStatusBar => guiVFlashStatusBar.Header.Id == component.Header.Id))
                {
                    GuiVFlashStatusBars.Children.Remove(guiVFlashStatusBar);
                    break;
                }
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var vFlashHandler = (VFlashHandler)component;
                vFlashHandler.Deinitialize();

                VFlashHandlers.Children.Remove(component);
            }
            if (Analyzers.Cast<object>().Any(analyzer => component == analyzer))
            {
                CheckAssignment(component, component.ReferencePosition);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var analyzer = (Analyzer.Analyzer)component;
                analyzer.Deinitialize();

                Analyzers.Children.Remove(component);
            }
            UpdateRegistry();
        }

        public void Deinitialize()
        {
            foreach (var composite in Modules.Cast<object>().SelectMany(module => ((RegistryComposite) module).Cast<Module>()))
            {
                composite.Deinitialize();
            }
        }

        public override void RemoveAll()
        {
            Deinitialize();

            PlcCommunicators.Clear();
            GuiPlcCommunicatorStatuses.Clear();
            GuiPlcCommunicatorStatusBars.Clear();
            GuiPlcCommunicatorConfigurations.Clear();

            CommunicationInterfaceHandlers.Clear();
            GuiComInterfacemunicationConfigurations.Clear();
            GuiCommunicationInterfaceOnlines.Clear();

            OutputDataTemplates.Clear();
            GuiOutputDataTemplates.Clear();

            OutputFileCreators.Clear();
            GuiOutputFileCreatorComponents.Clear();
            GuiOutputFileCreatorInterfaceAssignmentComponents.Clear();

            OutputHandlers.Clear();
            GuiOutputHandlerComponents.Clear();
            GuiOutputHandlerInterfaceAssignmentComponents.Clear();

            DbConnectionHandlers.Clear();
            GuiDbConnectionHandlers.Clear();
            GuiDbConnectionHandlerInterfaceAssignmentComponents.Clear();
            GuiDbStoredProcedures.Clear();

            VFlashTypeBanks.Clear();
            GuiVFlashPathBanks.Clear();

            VFlashHandlers.Clear();
            GuiVFlashHandlerComponents.Clear();
            GuiVFlashStatusBars.Clear();
            GuiVFlashHandlerInterfaceAssignmentComponents.Clear();

            Analyzers.Clear();
            GuiAnalyzerConfigurations.Clear();
            GuiAnalyzerControls.Clear();
            GuiAnalyzerMainFrames.Clear();
            GuiAnalyzerInterfaceAssignmentComponents.Clear();
            GuiAnalyzerDataCursorTables.Clear();

            UpdateRegistry();
            Logger.Log("Registry content removed");
        }

        private void UpdateRegistry()
        {
            UpdateMainRegistryFile();
            if (OnRegistryUpdated != null) OnRegistryUpdated();
        }

        private void UpdateMainRegistryFile()
        {
            MainRegistryFile.Default.PlcCommunicators = new uint[9][];
            foreach (PlcCommunicator plcCommunicator in PlcCommunicators)
            {
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id] = new uint[9];
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][0] =
                    plcCommunicator.Header.Id;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][1] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][2] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][3] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][4] = 0;
            }

            MainRegistryFile.Default.CommunicationInterfaceHandlers = new uint[9][];
            foreach (CommunicationInterfaceHandler communicationInterfaceHandler in CommunicationInterfaceHandlers)
            {
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id] = new uint[9];
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][0] =
                    communicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][1] =
                    communicationInterfaceHandler.PlcCommunicator.Header.Id;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][2] = 0;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][3] = 0;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][4] = 0;
            }

            MainRegistryFile.Default.OutputDataTemplates = new uint[9][];
            foreach (OutputDataTemplate outputDataTemplate in OutputDataTemplates)
            {
                MainRegistryFile.Default.OutputDataTemplates[outputDataTemplate.Header.Id] = new uint[9];
                MainRegistryFile.Default.OutputDataTemplates[outputDataTemplate.Header.Id][0] =
                    outputDataTemplate.Header.Id;
                MainRegistryFile.Default.OutputDataTemplates[outputDataTemplate.Header.Id][1] = 0;
                MainRegistryFile.Default.OutputDataTemplates[outputDataTemplate.Header.Id][2] = 0;
                MainRegistryFile.Default.OutputDataTemplates[outputDataTemplate.Header.Id][3] = 0;
                MainRegistryFile.Default.OutputDataTemplates[outputDataTemplate.Header.Id][4] = 0;
            }

            MainRegistryFile.Default.OutputFileCreators = new uint[9][];
            foreach (OutputFileCreator outputFileCreator in OutputFileCreators)
            {
                MainRegistryFile.Default.OutputFileCreators[outputFileCreator.Header.Id] = new uint[9];
                MainRegistryFile.Default.OutputFileCreators[outputFileCreator.Header.Id][0] =
                    outputFileCreator.Header.Id;
                MainRegistryFile.Default.OutputFileCreators[outputFileCreator.Header.Id][1] = 0;
                MainRegistryFile.Default.OutputFileCreators[outputFileCreator.Header.Id][2] =
                    outputFileCreator.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.OutputFileCreators[outputFileCreator.Header.Id][3] = 0;
                MainRegistryFile.Default.OutputFileCreators[outputFileCreator.Header.Id][4] =
                    outputFileCreator.OutputDataTemplate.Header.Id;
            }

            MainRegistryFile.Default.OutputHandlers = new uint[9][];
            foreach (OutputHandler outputHandler in OutputHandlers)
            {
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id] = new uint[9];
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][0] =
                    outputHandler.Header.Id;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][1] = 0;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][2] =
                    outputHandler.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][3] = 0;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][4] = 0;
            }

            MainRegistryFile.Default.DbConnectionHandlers = new uint[9][];
            foreach (DbConnectionHandler dbConnectionHandler in DbConnectionHandlers)
            {
                MainRegistryFile.Default.DbConnectionHandlers[dbConnectionHandler.Header.Id] = new uint[9];
                MainRegistryFile.Default.DbConnectionHandlers[dbConnectionHandler.Header.Id][0] =
                    dbConnectionHandler.Header.Id;
                MainRegistryFile.Default.DbConnectionHandlers[dbConnectionHandler.Header.Id][1] = 0;
                MainRegistryFile.Default.DbConnectionHandlers[dbConnectionHandler.Header.Id][2] =
                    dbConnectionHandler.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.DbConnectionHandlers[dbConnectionHandler.Header.Id][3] = 0;
                MainRegistryFile.Default.DbConnectionHandlers[dbConnectionHandler.Header.Id][4] = 0;
            }

            MainRegistryFile.Default.VFlashTypeBanks = new uint[9][];
            foreach (VFlashTypeBank vFlashTypeBank in VFlashTypeBanks)
            {
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id] = new uint[9];
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][0] =
                    vFlashTypeBank.Header.Id;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][1] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][2] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][3] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][4] = 0;
            }

            MainRegistryFile.Default.VFlashHandlers = new uint[9][];
            foreach (VFlashHandler vFlashHandler in VFlashHandlers)
            {
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id] = new uint[9];
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][0] = vFlashHandler.Header.Id;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][1] = 0;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][2] =
                    vFlashHandler.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][3] =
                    vFlashHandler.VFlashTypeBank.Header.Id;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][4] = 0;
            }

            MainRegistryFile.Default.Analyzers = new uint[9][];
            foreach (Analyzer.Analyzer analyzer in Analyzers)
            {
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id] = new uint[9];
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][0] = analyzer.Header.Id;
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][1] = 0;
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][2] =
                    analyzer.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][3] = 0;
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][4] = 0;
            }
            MainRegistryFile.Default.Save();
        }

        public void MakeNewConfiguration()
        {
            RemoveAll();

            MainRegistryFile.Default.Reset();
            PlcConfigurationFile.Default.Reset();
            CommunicationInterfacePath.Default.Reset();
            OutputDataTemplateFile.Default.Reset();
            OutputFileCreatorFile.Default.Reset();
            OutputFileCreatorInterfaceAssignmentFile.Default.Reset();
            OutputHandlerFile.Default.Reset();
            OutputHandlerInterfaceAssignmentFile.Default.Reset();
            DbConnectionHandlerFile.Default.Reset();
            DbConnectionHandlerInterfaceAssignmentFile.Default.Reset();
            VFlashTypeBankFile.Default.Reset();
            VFlashHandlerInterfaceAssignmentFile.Default.Reset();
            AnalyzerAssignmentFile.Default.Reset();
            AnalyzerSetupFile.Default.Reset();
        }

        public void LoadConfiguration(ProjectFileStruture.ProjectSavedData projectData)
        {
            MakeNewConfiguration();

            MainRegistryFile.Default.PlcCommunicators = projectData.PlcCommunicators;
            MainRegistryFile.Default.CommunicationInterfaceHandlers = projectData.CommunicationInterfaceHandlers;
            MainRegistryFile.Default.OutputDataTemplates = projectData.OutputDataTemplates;
            MainRegistryFile.Default.OutputFileCreators = projectData.OutputFileCreators;
            MainRegistryFile.Default.OutputHandlers = projectData.OutputHandlers;
            MainRegistryFile.Default.VFlashTypeBanks = projectData.VFlashTypeBanks;
            MainRegistryFile.Default.VFlashHandlers = projectData.VFlashHandlers;
            MainRegistryFile.Default.Analyzers = projectData.Analyzers;
            MainRegistryFile.Default.Save();

            PlcConfigurationFile.Default.Configuration = projectData.Configuration;
            PlcConfigurationFile.Default.ConnectAtStartUp = projectData.ConnectAtStartUp;
            PlcConfigurationFile.Default.Save();

            CommunicationInterfacePath.Default.Path = projectData.Path;
            CommunicationInterfacePath.Default.ConfigurationStatus = projectData.ConfigurationStatus;
            CommunicationInterfacePath.Default.Save();

            OutputDataTemplateFile.Default.TemplateFiles = projectData.TemplatePaths;

            OutputFileCreatorFile.Default.FileNameSuffixes = projectData.OutputFileCreatorFileNameSuffixes;
            OutputFileCreatorFile.Default.SelectedIndex = projectData.OutputFileCreatorSelectedIndex;
            OutputFileCreatorFile.Default.DirectoryPaths = projectData.OutputFileCreatorDirectoryPaths;
            OutputFileCreatorFile.Default.Save();

            OutputFileCreatorInterfaceAssignmentFile.Default.Assignment = projectData.OutputFileCreatorAssignment;
            OutputFileCreatorInterfaceAssignmentFile.Default.Save();

            OutputHandlerFile.Default.FileNameSuffixes = projectData.OutputHandlerFileNameSuffixes;
            OutputHandlerFile.Default.StartAddress = projectData.OutputHandlerStartAddress;
            OutputHandlerFile.Default.EndAddress = projectData.OutputHandlerEndAddress;
            OutputHandlerFile.Default.SelectedIndex = projectData.OutputHandlerSelectedIndex;
            OutputHandlerFile.Default.DirectoryPaths = projectData.OutputHandlerDirectoryPaths;
            OutputHandlerFile.Default.Save();

            OutputHandlerInterfaceAssignmentFile.Default.Assignment = projectData.OutputHandlerAssignment;
            OutputHandlerInterfaceAssignmentFile.Default.Save();

            DbConnectionHandlerFile.Default.DbInstances = projectData.DbConnectionHandlerFileDbInstances;
            DbConnectionHandlerFile.Default.InitialCatalogs = projectData.DbConnectionHandlerFileInitialCatalogs;
            DbConnectionHandlerFile.Default.ConfigurationFileNames = projectData.DbConnectionHandlerFileConfigurationFileNames;
            DbConnectionHandlerFile.Default.Save();

            DbConnectionHandlerInterfaceAssignmentFile.Default.Assignment = projectData.DbConnectionHandlerAssignment;
            DbConnectionHandlerInterfaceAssignmentFile.Default.Save();

            VFlashTypeBankFile.Default.TypeBank = projectData.TypeBank;
            VFlashTypeBankFile.Default.Save();

            VFlashHandlerInterfaceAssignmentFile.Default.Assignment = projectData.VFlashHandlerAssignment;
            VFlashHandlerInterfaceAssignmentFile.Default.Save();

            AnalyzerAssignmentFile.Default.Assignment = projectData.AnalyzerAssignment;
            AnalyzerAssignmentFile.Default.Save();

            AnalyzerSetupFile.Default.SampleTime = projectData.SampleTime;
            AnalyzerSetupFile.Default.TimeRange = projectData.TimeRange;
            AnalyzerSetupFile.Default.NumberOfChannels = projectData.NumberOfChannels;
            AnalyzerSetupFile.Default.Channels = projectData.Channels;
            AnalyzerSetupFile.Default.ShowDataCursors = projectData.ShowDataCursors;
            AnalyzerSetupFile.Default.Save();

            Logger.Log("Registry initialization");
            Initialize();
        }

        public ProjectFileStruture.ProjectSavedData SaveConfiguration()
        {
            var projectData = new ProjectFileStruture.ProjectSavedData
            {
                PlcCommunicators = MainRegistryFile.Default.PlcCommunicators,

                CommunicationInterfaceHandlers = MainRegistryFile.Default.CommunicationInterfaceHandlers,
                OutputDataTemplates = MainRegistryFile.Default.OutputDataTemplates,
                OutputFileCreators = MainRegistryFile.Default.OutputFileCreators,
                OutputHandlers = MainRegistryFile.Default.OutputHandlers,
                VFlashTypeBanks = MainRegistryFile.Default.VFlashTypeBanks,
                VFlashHandlers = MainRegistryFile.Default.VFlashHandlers,
                Analyzers = MainRegistryFile.Default.Analyzers,

                Configuration = PlcConfigurationFile.Default.Configuration,
                ConnectAtStartUp = PlcConfigurationFile.Default.ConnectAtStartUp,

                Path = CommunicationInterfacePath.Default.Path,
                ConfigurationStatus = CommunicationInterfacePath.Default.ConfigurationStatus,

                TemplatePaths = OutputDataTemplateFile.Default.TemplateFiles,

                OutputFileCreatorFileNameSuffixes = OutputFileCreatorFile.Default.FileNameSuffixes,
                OutputFileCreatorSelectedIndex = OutputFileCreatorFile.Default.SelectedIndex,
                OutputFileCreatorDirectoryPaths = OutputFileCreatorFile.Default.DirectoryPaths,

                OutputFileCreatorAssignment = OutputFileCreatorInterfaceAssignmentFile.Default.Assignment,

                DbConnectionHandlerFileDbInstances = DbConnectionHandlerFile.Default.DbInstances,
                DbConnectionHandlerFileInitialCatalogs = DbConnectionHandlerFile.Default.InitialCatalogs,
                DbConnectionHandlerFileConfigurationFileNames = DbConnectionHandlerFile.Default.ConfigurationFileNames,

                DbConnectionHandlerAssignment = DbConnectionHandlerInterfaceAssignmentFile.Default.Assignment,

                OutputHandlerFileNameSuffixes = OutputHandlerFile.Default.FileNameSuffixes,
                OutputHandlerStartAddress = OutputHandlerFile.Default.StartAddress,
                OutputHandlerEndAddress = OutputHandlerFile.Default.EndAddress,
                OutputHandlerSelectedIndex = OutputHandlerFile.Default.SelectedIndex,
                OutputHandlerDirectoryPaths = OutputHandlerFile.Default.DirectoryPaths,

                OutputHandlerAssignment = OutputHandlerInterfaceAssignmentFile.Default.Assignment,

                TypeBank = VFlashTypeBankFile.Default.TypeBank,

                VFlashHandlerAssignment = VFlashHandlerInterfaceAssignmentFile.Default.Assignment,

                SampleTime = AnalyzerSetupFile.Default.SampleTime,
                TimeRange = AnalyzerSetupFile.Default.TimeRange,
                NumberOfChannels = AnalyzerSetupFile.Default.NumberOfChannels,
                Channels = AnalyzerSetupFile.Default.Channels,
                ShowDataCursors = AnalyzerSetupFile.Default.ShowDataCursors,

                AnalyzerAssignment = AnalyzerAssignmentFile.Default.Assignment,
            };
            return projectData;
        }

        private static void CheckAssignment(RegistryComponent component, int position)
        {
            if (component == null) return;
            if (position == -1) return;
            var index = position;

            if (MainRegistryFile.Default.PlcCommunicators.Any(plcCommunicator => plcCommunicator != null && plcCommunicator[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.CommunicationInterfaceHandlers.Any(communicationInterfaceHandler => communicationInterfaceHandler != null && communicationInterfaceHandler[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.OutputDataTemplates.Any(outputDataTemplate => outputDataTemplate != null && outputDataTemplate[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.OutputFileCreators.Any(outputFileCreator => outputFileCreator != null && outputFileCreator[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.OutputHandlers.Any(outputHandler => outputHandler != null && outputHandler[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.DbConnectionHandlers.Any(dbConnectionHandler => dbConnectionHandler != null && dbConnectionHandler[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.VFlashTypeBanks.Any(vFlashTypeBank => vFlashTypeBank != null && vFlashTypeBank[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.VFlashHandlers.Any(vFlashHandler => vFlashHandler != null && vFlashHandler[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.Analyzers.Any(analyzer => analyzer != null && analyzer[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
        }
    }
}
