using System;
using System.Linq;
using System.Windows;
using _ttAgent.DataAquisition;
using _ttAgent.Log;
using _ttAgent.Output;
using _ttAgent.PLC;
using _ttAgent.Vector;
using _ttAgent.Visual.Gui;

namespace _ttAgent.MainRegistry
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

            UpdateMainRegistryFile();
            Logger.Log("Registry content initialized");
        }

        public override uint AddPlcCommunicator(Boolean save)
        {
            var id =  AddPlcCommunicator();
            if (save) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddPlcCommunicator()
        {
            var id = PlcCommunicators.GetFirstNotUsed();
            return AddPlcCommunicator(id);
        }

        public override uint AddPlcCommunicator(uint id)
        {
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

            PlcGuiCommunicationStatuses.Add(new GuiCommunicationStatus(id, "", component));
            PlcGuiCommunicationStatusBars.Add(new GuiCommunicationStatusBar(id, "", component));
            PlcGuiConfigurations.Add(new GuiPlcConfiguration(id, "", component));

            Logger.Log("ID: " + id + " new PLC Connection have been created");
            return id;
        }

        public override uint AddCommunicationInterface(Boolean save, uint plcConnectionId)
        {
            var id = AddCommunicationInterface(plcConnectionId);
            if (save) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddCommunicationInterface(uint plcConnectionId)
        {
            var id = CommunicationInterfaceHandlers.GetFirstNotUsed();
            return AddCommunicationInterface(plcConnectionId, id);
        }

        public override uint AddCommunicationInterface(uint id, uint plcConnectionId)
        {
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

            GuiComInterfacemunicationConfigurations.Add(new GuiComInterfacemunicationConfiguration(id, "", component));
            GuiCommunicationInterfaceOnlines.Add(new GuiCommunicationInterfaceOnline(id, "", component));

            Logger.Log("ID: " + id + " new Communication Interface have been created");
            return id;
        }

        public override uint AddOutputHandler(Boolean save, uint communicationInterfaceId)
        {
            var id = AddOutputHandler(communicationInterfaceId);
            if (save) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddOutputHandler(uint communicationInterfaceId)
        {
            var id = OutputHandlers.GetFirstNotUsed();
            return AddOutputHandler(communicationInterfaceId, id);
        }

        public override uint AddOutputHandler(uint id, uint communicationInterfaceId)
        {
            OutputHandler component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the Output Handler Component");
                OutputHandlers.Add(
                    new OutputHandler(id, "OUT__" + id,
                        (CommunicationInterfaceHandler)CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId), 
                        OutputHandlerFile.Default));
                Logger.Log("ID: " + id + " Initialization of the Output Handler");
                component = (OutputHandler)OutputHandlers.ReturnComponent(id);
                component.InitializeOutputHandler();
            }
            catch (Exception)
            {
                OutputHandlers.Remove(CommunicationInterfaceHandlers.ReturnComponent(id));
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Output Handler failed");
                return 0;
            }

            GuiOutputCreators.Add(new GuiOutputHandler(id, "", component));

            Logger.Log("ID: " + id + " new Output Handler have been created");
            return id;
        }

        public override uint AddVFlashBank(Boolean save)
        {
            var id = AddVFlashBank();
            if (save) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddVFlashBank()
        {
            var id = VFlashTypeBanks.GetFirstNotUsed();
            return AddVFlashBank(id);
        }

        public override uint AddVFlashBank(uint id)
        {
            Logger.Log("ID: " + id + " Creation of the vFlash Bank Component");
            VFlashTypeBanks.Add(new VFlashTypeBank(id, "VFLASH_BANK__" + id, VFlashTypeBankFile.Default));
            var component = (VFlashTypeBank)VFlashTypeBanks.ReturnComponent(id);

            GuiVFlashPathBanks.Add(new GuiVFlashPathBank(id, "", component));

            Logger.Log("ID: " + id + " new vFlash Bank have been created");
            return id;
        }

        public override uint AddVFlashChannel(Boolean save, uint communicationInterfaceId, uint vFlashBankId)
        {
            var id = AddVFlashChannel(communicationInterfaceId, vFlashBankId);
            if (save) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddVFlashChannel(uint communicationInterfaceId, uint vFlashBankId)
        {
            var id = VFlashHandlers.GetFirstNotUsed();
            return AddVFlashChannel(communicationInterfaceId, vFlashBankId, id);
        }

        public override uint AddVFlashChannel(uint id, uint communicationInterfaceId, uint vFlashBankId)
        {
            VFlashHandler component;
            try
            {
                Logger.Log("ID: " + id + " Creation of the vFlash Channel Component");
                VFlashHandlers.Add(
                    new VFlashHandler(id, "VFLASH__" + id,
                        (CommunicationInterfaceHandler)CommunicationInterfaceHandlers.ReturnComponent(communicationInterfaceId),
                        (VFlashTypeBank)VFlashTypeBanks.ReturnComponent(vFlashBankId)));
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

            GuiVFlashes.Add(new GuiVFlash(id, "", component));
            GuiVFlashStatusBars.Add(new GuiVFlashStatusBar(id, "", component));

            Logger.Log("ID: " + id + " new vFlash Channel have been created");
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

        public override void RemoveOutputHandler(uint id)
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

        public override void RemoveAll()
        {
            foreach (var vFlashHandler in VFlashHandlers.Cast<VFlashHandler>())
            {
                vFlashHandler.Deinitialize();
            }
            foreach (var plcCommunicator in PlcCommunicators.Cast<PlcCommunicator>())
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
            GuiOutputCreators.Clear();

            VFlashTypeBanks.Clear();
            GuiVFlashPathBanks.Clear();

            VFlashHandlers.Clear();
            GuiVFlashes.Clear();
            GuiVFlashStatusBars.Clear();

            UpdateMainRegistryFile();
            Logger.Log("Registry content removed");
        }

        private void UpdateMainRegistryFile()
        {
            MainRegistryFile.Default.PlcCommunicators = new uint[9][];
            foreach (var plcCommunicator in PlcCommunicators.Cast<PlcCommunicator>())
            {
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id] = new uint[4];
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][0] = 
                    plcCommunicator.Header.Id;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][1] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][2] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Header.Id][3] = 0;
            }

            MainRegistryFile.Default.CommunicationInterfaceHandlers = new uint[9][];
            foreach (var communicationInterfaceHandler in CommunicationInterfaceHandlers.Cast<CommunicationInterfaceHandler>())
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
            foreach (var outputHandler in OutputHandlers.Cast<OutputHandler>())
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
            foreach (var vFlashTypeBank in VFlashTypeBanks.Cast<VFlashTypeBank>())
            {
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id] = new uint[4];
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][0] = 
                    vFlashTypeBank.Header.Id;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][1] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][2] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Header.Id][3] = 0;
            }

            MainRegistryFile.Default.VFlashHandlers = new uint[9][];
            foreach (var vFlashHandler in VFlashHandlers.Cast<VFlashHandler>())
            {
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id] = new uint[4];
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][0] = vFlashHandler.Header.Id;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][1] = 0;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][2] =
                    vFlashHandler.CommunicationInterfaceHandler.Header.Id;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Header.Id][3] = 
                    vFlashHandler.VFlashTypeBank.Header.Id;
            }
            MainRegistryFile.Default.Save();
        }
    }
}
