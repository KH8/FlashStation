using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.Log;
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
        public Dictionary<uint, GuiCommunicationInterfaceOnline> GuiCommunicationInterfaceOnlines = new Dictionary<uint, GuiCommunicationInterfaceOnline>(); 

        public Dictionary<uint, OutputWriter> OutputWriters = new Dictionary<uint, OutputWriter>();
        public Dictionary<uint, GuiOutputCreator> GuiOutputCreators = new Dictionary<uint, GuiOutputCreator>();

        public Dictionary<uint, VFlashTypeBank> VFlashTypeBanks = new Dictionary<uint, VFlashTypeBank>();
        public Dictionary<uint, GuiVFlashPathBank> GuiVFlashPathBanks = new Dictionary<uint, GuiVFlashPathBank>();

        public Dictionary<uint, VFlashHandler> VFlashHandlers = new Dictionary<uint, VFlashHandler>();
        public Dictionary<uint, GuiVFlash> GuiVFlashes = new Dictionary<uint, GuiVFlash>();
        public Dictionary<uint, GuiVFlashStatusBar> GuiVFlashStatusBars = new Dictionary<uint, GuiVFlashStatusBar>(); 

        public Dictionary<uint, Tuple<uint, uint>> CommunicationInterfaceHandlersAssignemenTuples = new Dictionary<uint, Tuple<uint, uint>>();
        public Dictionary<uint, Tuple<uint, uint>> OutputWritersAssignemenTuples = new Dictionary<uint, Tuple<uint, uint>>();
        public Dictionary<uint, Tuple<uint, uint>> VFlashHandlersAssignemenTuples = new Dictionary<uint, Tuple<uint, uint>>();

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
            Logger.Log("ID: " + id + " new PLC Connection have been created");
            return id;
        }

        public override uint AddCommunicationInterface()
        {
            var id = (uint)CommunicationInterfaceHandlers.Count + 1;
            try
            {
                CommunicationInterfaceHandlers.Add(id,
                    new CommunicationInterfaceHandler(id, CommunicationInterfacePath.Default));
                GuiComInterfacemunicationConfigurations.Add(id,
                    new GuiComInterfacemunicationConfiguration(id, CommunicationInterfaceHandlers[id],
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

        public override uint AddOutputWriter()
        {
            var id = (uint)OutputWriters.Count + 1;
            try
            {
                OutputWriters.Add(id, null);
                GuiOutputCreators.Add(id,
                    new GuiOutputCreator(CommunicationInterfaceHandlers[OutputWritersAssignemenTuples[id].Item2], OutputCreatorFile.Default));
            }
            catch (Exception)
            {
                if (OutputWriters[id] != null) OutputWriters.Remove(id);
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
            VFlashTypeBanks.Add(id, new VFlashTypeBank());
            GuiVFlashPathBanks.Add(id, new GuiVFlashPathBank(id, VFlashTypeBankFile.Default, VFlashTypeBanks[id]));
            Logger.Log("ID: " + id + " new vFlash Bank have been created");
            return id;
        }

        public override uint AddVFlashChannel()
        {
            var id = (uint)VFlashHandlers.Count + 1;
            try
            {
                VFlashHandlers.Add(id,
                    new VFlashHandler(id, CommunicationInterfaceHandlers[VFlashHandlersAssignemenTuples[id].Item2].ReadInterfaceComposite,
                        CommunicationInterfaceHandlers[VFlashHandlersAssignemenTuples[id].Item2].WriteInterfaceComposite));
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
