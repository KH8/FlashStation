using System;
using System.Windows.Controls;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Visual
{
    public class DataDisplay
    {
        public static void Display(ListBox onlineReadDataListBox, ListBox onlineWriteDataListBox, PlcCommunicator communication, CommunicationInterfaceHandler communicationHandler)
        {
            onlineReadDataListBox.Dispatcher.BeginInvoke((new Action(delegate
            {
                onlineReadDataListBox.Items.Clear();
                onlineReadDataListBox.Items.Add("Read area: " + "DB" + communication.PlcConfiguration.PlcReadDbNumber);
                foreach (CommunicationInterfaceComponent inputComponent in communicationHandler.ReadInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.SingleBit: DisplayComponent(onlineReadDataListBox, inputComponent as CiSingleBit, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.BitArray: DisplayComponent(onlineReadDataListBox, inputComponent as CiBitArray, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.Integer: DisplayComponent(onlineReadDataListBox, inputComponent as CiInteger, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.DoubleInteger: DisplayComponent(onlineReadDataListBox, inputComponent as CiDoubleInteger, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.Real: DisplayComponent(onlineReadDataListBox, inputComponent as CiReal, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.String: DisplayComponent(onlineReadDataListBox, inputComponent as CiString, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                    }
                }
            })));
            onlineWriteDataListBox.Dispatcher.BeginInvoke((new Action(delegate
            {
                onlineWriteDataListBox.Items.Clear();
                onlineWriteDataListBox.Items.Add("Write area: " + "DB" + communication.PlcConfiguration.PlcWriteDbNumber);
                foreach (CommunicationInterfaceComponent inputComponent in communicationHandler.WriteInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.SingleBit: DisplayComponent(onlineWriteDataListBox, inputComponent as CiSingleBit, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.BitArray: DisplayComponent(onlineWriteDataListBox, inputComponent as CiBitArray, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.Integer: DisplayComponent(onlineWriteDataListBox, inputComponent as CiInteger, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.DoubleInteger: DisplayComponent(onlineWriteDataListBox, inputComponent as CiDoubleInteger, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.Real: DisplayComponent(onlineWriteDataListBox, inputComponent as CiReal, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case CommunicationInterfaceComponent.VariableType.String: DisplayComponent(onlineWriteDataListBox, inputComponent as CiString, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                    }
                }
            })));
        }

        private static void DisplayComponent(ListBox listBox, CiSingleBit component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBW " + address + "." + component.BitPosition + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiBitArray component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBW " + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiInteger component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBW " + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiDoubleInteger component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBW " + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiReal component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBD " + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiString component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DB " + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }
    }
}