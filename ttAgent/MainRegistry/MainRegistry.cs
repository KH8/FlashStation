using System;
using System.Collections.Generic;
using System.Windows;
using _ttAgent.DataAquisition;
using _ttAgent.Log;
using _ttAgent.Output;
using _ttAgent.PLC;
using _ttAgent.Vector;
using _ttAgent.Visual.Gui;

namespace _ttAgent.MainRegistry
{
    abstract class RegistryBase
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

        public abstract uint AddPlcCommunicator(uint save);
        public abstract uint AddCommunicationInterface(uint save, uint plcConnectionId);
        public abstract uint AddOutputHandler(uint save, uint communicationInterfaceId);
        public abstract uint AddVFlashBank(uint save);
        public abstract uint AddVFlashChannel(uint save, uint communicationInterfaceId, uint vFlashBankId);

        public abstract void RemovePlcCommunicator(uint id);
        public abstract void RemoveCommunicationInterface(uint id);
        public abstract void RemoveOutputHandler(uint id);
        public abstract void RemoveVFlashBank(uint id);
        public abstract void RemoveVFlashChannel(uint id);

        public abstract void RemoveAll();
    }

    class Registry : RegistryBase
    {
        public void Initialize()
        {
            if (MainRegistryFile.Default.PlcCommunicators == null) return;
            if (MainRegistryFile.Default.CommunicationInterfaceHandlers == null) return;
            if (MainRegistryFile.Default.OutputHandlers == null) return;
            if (MainRegistryFile.Default.VFlashTypeBanks == null) return;
            if (MainRegistryFile.Default.VFlashHandlers == null) return;

            foreach (var plcCommunicator in MainRegistryFile.Default.PlcCommunicators)
            { if (plcCommunicator != null) { AddPlcCommunicator(); }}
            foreach (var communicationInterfaceHandler in MainRegistryFile.Default.CommunicationInterfaceHandlers)
            { if (communicationInterfaceHandler != null) { AddCommunicationInterface(communicationInterfaceHandler[1]); } }
            foreach (var outputHandler in MainRegistryFile.Default.OutputHandlers)
            { if (outputHandler != null) { AddOutputHandler(outputHandler[2]); } }
            foreach (var vFlashTypeBank in MainRegistryFile.Default.VFlashTypeBanks)
            { if (vFlashTypeBank != null) { AddVFlashBank(); } }
            foreach (var vFlashHandler in MainRegistryFile.Default.VFlashHandlers)
            { if (vFlashHandler != null) { AddVFlashChannel(vFlashHandler[2], vFlashHandler[3]); } }

            UpdateMainRegistryFile();
            Logger.Log("Registry content initialized");
        }

        public override uint AddPlcCommunicator(uint save)
        {
            var id =  AddPlcCommunicator();
            if (save == 1) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddCommunicationInterface(uint save, uint plcConnectionId)
        {
            var id = AddCommunicationInterface(plcConnectionId);
            if (save == 1) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddOutputHandler(uint save, uint communicationInterfaceId)
        {
            var id = AddOutputHandler(communicationInterfaceId);
            if (save == 1) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddVFlashBank(uint save)
        {
            var id = AddVFlashBank();
            if (save == 1) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddVFlashChannel(uint save, uint communicationInterfaceId, uint vFlashBankId)
        {
            var id = AddVFlashChannel(communicationInterfaceId, vFlashBankId);
            if (save == 1) UpdateMainRegistryFile();
            return id;
        }

        public override uint AddPlcCommunicator()
        {
            var id = (uint)PlcCommunicators.Count + 1;
            if (id > 8) { MessageBox.Show("Maximum number of Plc Communicator \ncomponents exceeded", "Component Creation Failed"); return 0; }

            try
            {
                Logger.Log("ID: " + id + " Creation of the PLC Connection Component");
                PlcCommunicators.Add(id, new PlcCommunicator(id, PlcConfigurationFile.Default));
                Logger.Log("ID: " + id + " Initialization of the PLC Connection");
                PlcCommunicators[id].InitializeConnection();
            }
            catch (Exception)
            {
                if (PlcCommunicators[id] != null) PlcCommunicators.Remove(id);
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new PLC Connection failed");
                return 0;
            }

            PlcGuiCommunicationStatuses.Add(id, new GuiCommunicationStatus(id, PlcCommunicators[id], PlcConfigurationFile.Default));
            PlcGuiCommunicationStatusBars.Add(id, new GuiCommunicationStatusBar(id, PlcCommunicators[id]));
            PlcGuiConfigurations.Add(id, new GuiPlcConfiguration(id, PlcCommunicators[id],  PlcConfigurationFile.Default));
            
            Logger.Log("ID: " + id + " new PLC Connection have been created");
            return id;
        }

        public override uint AddCommunicationInterface(uint plcConnectionId)
        {
            var id = (uint)CommunicationInterfaceHandlers.Count + 1;
            if (id > 8) { MessageBox.Show("Maximum number of Communication Interface \ncomponents exceeded", "Component Creation Failed"); return 0; }

            CommunicationInterfaceHandlersAssignemenTuples[id] = new Tuple<uint, uint>(plcConnectionId, 0);
            try
            {
                Logger.Log("ID: " + id + " Creation of the Communication Interface Component");
                CommunicationInterfaceHandlers.Add(id,
                    new CommunicationInterfaceHandler(id, CommunicationInterfacePath.Default));
                Logger.Log("ID: " + id + " Initialization of the Communication Interface");
                CommunicationInterfaceHandlers[id].InitializeInterface();

                GuiComInterfacemunicationConfigurations.Add(id,
                    new GuiComInterfacemunicationConfiguration(id, 
                        PlcCommunicators[CommunicationInterfaceHandlersAssignemenTuples[id].Item1],
                        CommunicationInterfaceHandlers[id],
                        CommunicationInterfacePath.Default));
                GuiCommunicationInterfaceOnlines.Add(id,
                    new GuiCommunicationInterfaceOnline(id,
                        PlcCommunicators[CommunicationInterfaceHandlersAssignemenTuples[id].Item1],
                        CommunicationInterfaceHandlers[id]));               
            }
            catch (Exception)
            {
                if (CommunicationInterfaceHandlers[id] != null) CommunicationInterfaceHandlers.Remove(id);
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Communication Interface failed");
                return 0;
            }

            Logger.Log("ID: " + id + " new Communication Interface have been created");
            return id;
        }

        public override uint AddOutputHandler(uint communicationInterfaceId)
        {
            var id = (uint)OutputHandlers.Count + 1;
            if (id > 8) { MessageBox.Show("Maximum number of Output Handler \ncomponents exceeded", "Component Creation Failed"); return 0; }

            OutputHandlersAssignemenTuples[id] = new Tuple<uint, uint>(0, communicationInterfaceId);
            try
            {
                Logger.Log("ID: " + id + " Creation of the Output Handler Component");
                OutputHandlers.Add(id, new OutputHandler(id, CommunicationInterfaceHandlers[OutputHandlersAssignemenTuples[id].Item2].ReadInterfaceComposite, CommunicationInterfaceHandlers[OutputHandlersAssignemenTuples[id].Item2].WriteInterfaceComposite));
                GuiOutputCreators.Add(id,
                    new GuiOutputHandler(id, OutputHandlers[id], OutputHandlerFile.Default));
                Logger.Log("ID: " + id + " Initialization of the Output Handler");
                OutputHandlers[id].InitializeOutputHandler();
            }
            catch (Exception)
            {
                if (OutputHandlers[id] != null) OutputHandlers.Remove(id);
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new Output Handler failed");
                return 0;
            }

            Logger.Log("ID: " + id + " new Output Handler have been created");
            return id;
        }

        public override uint AddVFlashBank()
        {
            var id = (uint)VFlashTypeBanks.Count + 1;
            if (id > 8) { MessageBox.Show("Maximum number of vFlash Bank \ncomponents exceeded", "Component Creation Failed"); return 0; }

            Logger.Log("ID: " + id + " Creation of the vFlash Bank Component");
            VFlashTypeBanks.Add(id, new VFlashTypeBank());
            GuiVFlashPathBanks.Add(id, new GuiVFlashPathBank(id, VFlashTypeBankFile.Default, VFlashTypeBanks[id]));

            Logger.Log("ID: " + id + " new vFlash Bank have been created");
            return id;
        }

        public override uint AddVFlashChannel(uint communicationInterfaceId, uint vFlashBankId)
        {
            var id = (uint)VFlashHandlers.Count + 1;
            if (id > 8) { MessageBox.Show("Maximum number of vFlash Channel \ncomponents exceeded", "Component Creation Failed"); return 0; }

            VFlashHandlersAssignemenTuples[id] = new Tuple<uint, uint, uint>(0, communicationInterfaceId, vFlashBankId);
            try
            {
                Logger.Log("ID: " + id + " Creation of the vFlash Channel Component");
                VFlashHandlers.Add(id,
                    new VFlashHandler(id, CommunicationInterfaceHandlers[VFlashHandlersAssignemenTuples[id].Item2].ReadInterfaceComposite,
                        CommunicationInterfaceHandlers[VFlashHandlersAssignemenTuples[id].Item2].WriteInterfaceComposite));
                Logger.Log("ID: " + id + " Initialization of the vFlash Channel");
                VFlashHandlers[id].InitializeVFlash();
                VFlashHandlers[id].VFlashTypeBank = VFlashTypeBanks[vFlashBankId];

                GuiVFlashes.Add(id, new GuiVFlash(id, VFlashHandlers[id]));
                GuiVFlashStatusBars.Add(id, new GuiVFlashStatusBar(id, VFlashHandlers[id]));             
            }
            catch (Exception)
            {
                if (VFlashHandlers[id] != null) VFlashHandlers.Remove(id);
                MessageBox.Show("Component could not be created", "Component Creation Failed");
                Logger.Log("Creation of a new vFlash Channel failed");
                return 0;
            }

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
            foreach (var vFlashHandler in VFlashHandlers) vFlashHandler.Value.Deinitialize();
            foreach (var plcCommunicator in PlcCommunicators) plcCommunicator.Value.CloseConnection();

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

            CommunicationInterfaceHandlersAssignemenTuples.Clear();
            OutputHandlersAssignemenTuples.Clear();
            VFlashHandlersAssignemenTuples.Clear();

            UpdateMainRegistryFile();
            Logger.Log("Registry content removed");
        }

        private void UpdateMainRegistryFile()
        {
            MainRegistryFile.Default.PlcCommunicators = new uint[9][];
            foreach (var plcCommunicator in PlcCommunicators)
            {
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Key] = new uint[4];
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Key][0] = plcCommunicator.Key;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Key][1] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Key][2] = 0;
                MainRegistryFile.Default.PlcCommunicators[plcCommunicator.Key][3] = 0;
            }

            MainRegistryFile.Default.CommunicationInterfaceHandlers = new uint[9][];
            foreach (var communicationInterfaceHandler in CommunicationInterfaceHandlers)
            {
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Key] = new uint[4];
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Key][0] = communicationInterfaceHandler.Key;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Key][1] = CommunicationInterfaceHandlersAssignemenTuples[communicationInterfaceHandler.Key].Item1;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Key][2] = 0;
                MainRegistryFile.Default.CommunicationInterfaceHandlers[communicationInterfaceHandler.Key][3] = 0;
            }

            MainRegistryFile.Default.OutputHandlers = new uint[9][];
            foreach (var outputHandler in OutputHandlers)
            {
                MainRegistryFile.Default.OutputHandlers[outputHandler.Key] = new uint[4];
                MainRegistryFile.Default.OutputHandlers[outputHandler.Key][0] = outputHandler.Key;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Key][1] = 0;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Key][2] = OutputHandlersAssignemenTuples[outputHandler.Key].Item2;
                MainRegistryFile.Default.OutputHandlers[outputHandler.Key][3] = 0;
            }

            MainRegistryFile.Default.VFlashTypeBanks = new uint[9][];
            foreach (var vFlashTypeBank in VFlashTypeBanks)
            {
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Key] = new uint[4];
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Key][0] = vFlashTypeBank.Key;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Key][1] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Key][2] = 0;
                MainRegistryFile.Default.VFlashTypeBanks[vFlashTypeBank.Key][3] = 0;
            }

            MainRegistryFile.Default.VFlashHandlers = new uint[9][];
            foreach (var vFlashHandler in VFlashHandlers)
            {
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Key] = new uint[4];
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Key][0] = vFlashHandler.Key;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Key][1] = 0;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Key][2] = VFlashHandlersAssignemenTuples[vFlashHandler.Key].Item2;
                MainRegistryFile.Default.VFlashHandlers[vFlashHandler.Key][3] = VFlashHandlersAssignemenTuples[vFlashHandler.Key].Item3;
            }
            MainRegistryFile.Default.Save();
        }
    }
}
