using System;
using System.Linq;
using System.Windows;
using _PlcAgent.Analyzer;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.Output;
using _PlcAgent.PLC;
using _PlcAgent.Vector;
using _PlcAgent.Visual.Gui;

namespace _PlcAgent.MainRegistry
{
    class Registry : RegistryAbstract
    {
        public void Initialize()
        {
            if (MainRegistryFile.Default.PlcCommunicators == null) return;
            if (MainRegistryFile.Default.CommunicationInterfaceHandlers == null) return;
            if (MainRegistryFile.Default.OutputHandlers == null) return;
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
            foreach (var outputHandler in MainRegistryFile.Default.OutputHandlers.Where(outputHandler => outputHandler != null))
            {
                AddOutputHandler(outputHandler[0], outputHandler[2]);
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

            UpdateMainRegistryFile();
            Logger.Log("Registry content initialized");
        }

        public override uint AddPlcCommunicator(Boolean save)
        {
            var id =  AddPlcCommunicator();
            if (save && id != 0) UpdateMainRegistryFile();
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
                component = (PlcCommunicator)PlcCommunicators.ReturnComponent(id);
                component.InitializeConnection();
            }
            catch (Exception)
            {
                PlcCommunicators.Remove(PlcCommunicators.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new PLC Connection failed");
                return 0;
            }

            PlcGuiCommunicationStatuses.Add(new GuiComponent(id, "", new GuiCommunicationStatus(component)));
            PlcGuiCommunicationStatusBars.Add(new GuiComponent(id, "", new GuiCommunicationStatusBar(component)));
            PlcGuiConfigurations.Add(new GuiComponent(id, "", new GuiPlcConfiguration(component)));

            Logger.Log("ID: " + id + " new PLC Connection have been created");
            return id;
        }

        public override uint AddCommunicationInterface(Boolean save, uint plcConnectionId)
        {
            var id = AddCommunicationInterface(plcConnectionId);
            if (save && id != 0) UpdateMainRegistryFile();
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
                MessageBox.Show("Maximum number of Communicator Interface \ncomponents exceeded", "Component Creation Failed");
                return 0;
            }

            CommunicationInterfaceHandler component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the Communication Interface Component");
                CommunicationInterfaceHandlers.Add(
                    new CommunicationInterfaceHandler(id, "INT__" + id, 
                        (PlcCommunicator)PlcCommunicators.ReturnComponent(plcConnectionId), 
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

            GuiComInterfacemunicationConfigurations.Add(new GuiComponent(id, "", new GuiCommunicationInterfaceConfiguration(component)));
            GuiCommunicationInterfaceOnlines.Add(new GuiComponent(id, "", new GuiCommunicationInterfaceOnline(component)));

            Logger.Log("ID: " + id + " new Communication Interface have been created");
            return id;
        }

        public override uint AddOutputHandler(Boolean save, uint communicationInterfaceId)
        {
            var id = AddOutputHandler(communicationInterfaceId);
            if (save && id != 0) UpdateMainRegistryFile();
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
                        (CommunicationInterfaceHandler)CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId), 
                        OutputHandlerFile.Default,
                        OutputHandlerInterfaceAssignmentFile.Default));
                Logger.Log("ID: " + id + " Initialization of the Output Handler");
                component = (OutputHandler)OutputHandlers.ReturnComponent(id);
                component.InitializeOutputHandler();
            }
            catch (Exception)
            {
                OutputHandlers.Remove(OutputHandlers.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Output Handler failed");
                return 0;
            }

            GuiOutputHandlerComponents.Add(new GuiComponent(id, "", new GuiOutputHandler(component)));
            GuiOutputHandlerInterfaceAssignmentComponents.Add(new GuiComponent(id, "", new GuiInterfaceAssignment(component)));

            Logger.Log("ID: " + id + " new Output Handler have been created");
            return id;
        }

        public override uint AddVFlashBank(Boolean save)
        {
            var id = AddVFlashBank();
            if (save && id != 0) UpdateMainRegistryFile();
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
            var component = (VFlashTypeBank)VFlashTypeBanks.ReturnComponent(id);

            GuiVFlashPathBanks.Add(new GuiComponent(id, "", new GuiVFlashPathBank(component)));

            Logger.Log("ID: " + id + " new vFlash Bank have been created");
            return id;
        }

        public override uint AddVFlashChannel(Boolean save, uint communicationInterfaceId, uint vFlashBankId)
        {
            var id = AddVFlashChannel(communicationInterfaceId, vFlashBankId);
            if (save && id != 0) UpdateMainRegistryFile();
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
                        (CommunicationInterfaceHandler)CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId),
                        (VFlashTypeBank)VFlashTypeBanks.ReturnComponent(vFlashBankId),
                        VFlashHandlerInterfaceAssignmentFile.Default));
                Logger.Log("ID: " + id + " Initialization of the vFlash Channel");
                component = (VFlashHandler)VFlashHandlers.ReturnComponent(id);
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
            GuiVFlashHandlerInterfaceAssignmentComponents.Add(new GuiComponent(id, "", new GuiInterfaceAssignment(component)));

            Logger.Log("ID: " + id + " new vFlash Channel have been created");
            return id;
        }

        public override uint AddAnalyzer(bool save, uint communicationInterfaceId)
        {
            var id = AddAnalyzer(communicationInterfaceId);
            if (save && id != 0) UpdateMainRegistryFile();
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
                        (CommunicationInterfaceHandler)CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId), AnalyzerAssignmentFile.Default, AnalyzerSetupFile.Default));
                Logger.Log("ID: " + id + " Initialization of the Analyzer");
                component = (Analyzer.Analyzer)Analyzers.ReturnComponent(id);
                component.Initialize();
            }
            catch (Exception)
            {
                Analyzers.Remove(Analyzers.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Analyzer failed");
                return 0;
            }

            GuiAnalyzers.Add(new GuiComponent(id, "", new GuiAnalyzer(component)));
            component.GuiAnalyzerMainFrame = new GuiAnalyzerMainFrame(component);
            GuiAnalyzerMainFrames.Add(new GuiComponent(id, "", component.GuiAnalyzerMainFrame));
            GuiAnalyzerInterfaceAssignmentComponents.Add(new GuiComponent(id, "", new GuiInterfaceAssignment(component)));

            Logger.Log("ID: " + id + " new Analyzer have been created");
            return id;
        }

        public override void RemoveComponent(RegistryComponent component)
        {
            if (PlcCommunicators.Cast<object>().Any(plcCommunicator => component == plcCommunicator))
            {
                CheckAssignment(component, 1);
                foreach (var plcGuiCommunicationStatusBar in PlcGuiCommunicationStatusBars.Cast<GuiComponent>().Where(plcGuiCommunicationStatusBar => plcGuiCommunicationStatusBar.Header.Id == component.Header.Id))
                {
                    PlcGuiCommunicationStatusBars.Children.Remove(plcGuiCommunicationStatusBar);
                    break;
                }
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");

                var plcCommunicator = (PlcCommunicator) component;
                plcCommunicator.CloseConnection();

                PlcCommunicators.Children.Remove(component);
            }
            if (CommunicationInterfaceHandlers.Cast<object>().Any(communicationInterfaceHandler => component == communicationInterfaceHandler))
            {
                CheckAssignment(component, 2);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");
                CommunicationInterfaceHandlers.Children.Remove(component);
            }
            if (OutputHandlers.Cast<object>().Any(outputHandler => component == outputHandler))
            {
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");
                OutputHandlers.Children.Remove(component);
            }
            if (VFlashTypeBanks.Cast<object>().Any(vFlashTypeBank => component == vFlashTypeBank))
            {
                CheckAssignment(component, 3);
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");
                VFlashTypeBanks.Children.Remove(component);
            }
            if (VFlashHandlers.Cast<object>().Any(vFlashHandler => component == vFlashHandler))
            {
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
                Logger.Log("ID: " + component.Header.Id + " Component " + component.Header.Name + " has been removed");
                Analyzers.Children.Remove(component);
            }
            UpdateMainRegistryFile();
        }

        public override void RemoveAll()
        {
            foreach (VFlashHandler vFlashHandler in VFlashHandlers)
            {
                vFlashHandler.Deinitialize();
            }
            foreach (PlcCommunicator plcCommunicator in PlcCommunicators)
            {
                plcCommunicator.CloseConnection();
            }

            PlcCommunicators.Clear();
            PlcGuiCommunicationStatuses.Clear();
            PlcGuiCommunicationStatusBars.Clear();
            PlcGuiConfigurations.Clear();

            CommunicationInterfaceHandlers.Clear();
            GuiComInterfacemunicationConfigurations.Clear();
            GuiCommunicationInterfaceOnlines.Clear();

            OutputHandlers.Clear();
            GuiOutputHandlerComponents.Clear();
            GuiOutputHandlerInterfaceAssignmentComponents.Clear();

            VFlashTypeBanks.Clear();
            GuiVFlashPathBanks.Clear();

            VFlashHandlers.Clear();
            GuiVFlashHandlerComponents.Clear();
            GuiVFlashStatusBars.Clear();
            GuiVFlashHandlerInterfaceAssignmentComponents.Clear();

            Analyzers.Clear();
            GuiAnalyzers.Clear();
            GuiAnalyzerMainFrames.Clear();
            GuiAnalyzerInterfaceAssignmentComponents.Clear();

            UpdateMainRegistryFile();
            Logger.Log("Registry content removed");
        }

        private void UpdateMainRegistryFile()
        {
            MainRegistryFile.Default.PlcCommunicators = new uint[9][];
            foreach (PlcCommunicator plcCommunicator in PlcCommunicators)
            {
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id] = new uint[4];
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][0] = 
                    plcCommunicator.Header.Id;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][1] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][2] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][3] = 0;
            }

            MainRegistryFile.Default.CommunicationInterfaceHandlers = new uint[9][];
            foreach (CommunicationInterfaceHandler communicationInterfaceHandler in CommunicationInterfaceHandlers)
            {
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id] = new uint[4];
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][0] = 
                    communicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][1] =
                    communicationInterfaceHandler.PlcCommunicator.Header.Id;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][2] = 0;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Header.Id][3] = 0;
            }

            MainRegistryFile.Default.OutputHandlers = new uint[9][];
            foreach (OutputHandler outputHandler in OutputHandlers)
            {
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id] = new uint[4];
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][0] = 
                    outputHandler.Header.Id;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][1] = 0;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][2] =
                    outputHandler.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Header.Id][3] = 0;
            }

            MainRegistryFile.Default.VFlashTypeBanks = new uint[9][];
            foreach (VFlashTypeBank vFlashTypeBank in VFlashTypeBanks)
            {
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id] = new uint[4];
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][0] = 
                    vFlashTypeBank.Header.Id;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][1] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][2] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][3] = 0;
            }

            MainRegistryFile.Default.VFlashHandlers = new uint[9][];
            foreach (VFlashHandler vFlashHandler in VFlashHandlers)
            {
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id] = new uint[4];
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][0] = vFlashHandler.Header.Id;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][1] = 0;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][2] =
                    vFlashHandler.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][3] = 
                    vFlashHandler.VFlashTypeBank.Header.Id;
            }

            MainRegistryFile.Default.Analyzers = new uint[9][];
            foreach (Analyzer.Analyzer analyzer in Analyzers)
            {
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id] = new uint[4];
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][0] = analyzer.Header.Id;
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][1] = 0;
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][2] =
                    analyzer.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.Analyzers[analyzer.Header.Id][3] = 0;
            }
            MainRegistryFile.Default.Save();
        }

        public void MakeNewConfiguration()
        {
            RemoveAll();

            MainRegistryFile.Default.Reset();
            PlcConfigurationFile.Default.Reset();
            CommunicationInterfacePath.Default.Reset();
            OutputHandlerFile.Default.Reset();
            OutputHandlerInterfaceAssignmentFile.Default.Reset();
            VFlashTypeBankFile.Default.Reset();
            VFlashHandlerInterfaceAssignmentFile.Default.Reset();
            AnalyzerAssignmentFile.Default.Reset();
            AnalyzerSetupFile.Default.Reset();
        }

        public void LoadConfiguration(ProjectFileStruture.ProjectSavedData projectData)
        {
            RemoveAll();

            MainRegistryFile.Default.Reset();
            PlcConfigurationFile.Default.Reset();
            CommunicationInterfacePath.Default.Reset();
            OutputHandlerFile.Default.Reset();
            VFlashTypeBankFile.Default.Reset();

            MainRegistryFile.Default.PlcCommunicators = projectData.PlcCommunicators;
            MainRegistryFile.Default.CommunicationInterfaceHandlers = projectData.CommunicationInterfaceHandlers;
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

            OutputHandlerFile.Default.FileNameSuffixes = projectData.FileNameSuffixes;
            OutputHandlerFile.Default.StartAddress = projectData.StartAddress;
            OutputHandlerFile.Default.EndAddress = projectData.EndAddress;
            OutputHandlerFile.Default.SelectedIndex = projectData.SelectedIndex;
            OutputHandlerFile.Default.DirectoryPaths = projectData.DirectoryPaths;
            OutputHandlerFile.Default.Save();

            OutputHandlerInterfaceAssignmentFile.Default.Assignment = projectData.OutputHandlerAssignment;
            OutputHandlerInterfaceAssignmentFile.Default.Save();

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
                OutputHandlers = MainRegistryFile.Default.OutputHandlers,
                VFlashTypeBanks = MainRegistryFile.Default.VFlashTypeBanks,
                VFlashHandlers = MainRegistryFile.Default.VFlashHandlers,
                Analyzers = MainRegistryFile.Default.Analyzers,
                Configuration = PlcConfigurationFile.Default.Configuration,
                ConnectAtStartUp = PlcConfigurationFile.Default.ConnectAtStartUp,
                Path = CommunicationInterfacePath.Default.Path,
                ConfigurationStatus = CommunicationInterfacePath.Default.ConfigurationStatus,
                FileNameSuffixes = OutputHandlerFile.Default.FileNameSuffixes,
                StartAddress = OutputHandlerFile.Default.StartAddress,
                EndAddress = OutputHandlerFile.Default.EndAddress,
                SelectedIndex = OutputHandlerFile.Default.SelectedIndex,
                DirectoryPaths = OutputHandlerFile.Default.DirectoryPaths,
                OutputHandlerAssignment = OutputHandlerInterfaceAssignmentFile.Default.Assignment,
                TypeBank = VFlashTypeBankFile.Default.TypeBank,
                VFlashHandlerAssignment = VFlashHandlerInterfaceAssignmentFile.Default.Assignment,
                AnalyzerAssignment = AnalyzerAssignmentFile.Default.Assignment,
                SampleTime = AnalyzerSetupFile.Default.SampleTime,
                TimeRange = AnalyzerSetupFile.Default.TimeRange,
                NumberOfChannels = AnalyzerSetupFile.Default.NumberOfChannels,
                Channels = AnalyzerSetupFile.Default.Channels
            };
            return projectData;
        }

        private void CheckAssignment(RegistryComponent component, int position)
        {
            if (component == null) return;
            var index = position;

            if (MainRegistryFile.Default.PlcCommunicators.Any(plcCommunicator => plcCommunicator != null && plcCommunicator[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.CommunicationInterfaceHandlers.Any(communicationInterfaceHandler => communicationInterfaceHandler != null && communicationInterfaceHandler[index] == component.Header.Id))
            {
                throw new Exception("The component is still assigned to another one");
            }
            if (MainRegistryFile.Default.OutputHandlers.Any(outputHandler => outputHandler != null && outputHandler[index] == component.Header.Id))
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
