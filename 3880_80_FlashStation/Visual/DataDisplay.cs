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
            int address;
            onlineReadDataListBox.Dispatcher.BeginInvoke((new Action(delegate
            {
                onlineReadDataListBox.Items.Clear();
                onlineReadDataListBox.Items.Add("Read area: " + "DB" + communication.PlcConfiguration.PlcReadDbNumber);
                foreach (CommunicationInterfaceComponent inputComponent in communicationHandler.ReadInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case "BitArray": DisplayComponent(onlineReadDataListBox, inputComponent as CiBitArray, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case "Integer": DisplayComponent(onlineReadDataListBox, inputComponent as CiInteger, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case "DoubleInteger": DisplayComponent(onlineReadDataListBox, inputComponent as CiDoubleInteger, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case "Real": DisplayComponent(onlineReadDataListBox, inputComponent as CiReal, communication.PlcConfiguration.PlcReadStartAddress);
                            break;
                        case "String": DisplayComponent(onlineReadDataListBox, inputComponent as CiString, communication.PlcConfiguration.PlcReadStartAddress);
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
                        case "BitArray": DisplayComponent(onlineWriteDataListBox, inputComponent as CiBitArray, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case "Integer": DisplayComponent(onlineWriteDataListBox, inputComponent as CiInteger, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case "DoubleInteger": DisplayComponent(onlineWriteDataListBox, inputComponent as CiDoubleInteger, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case "Real": DisplayComponent(onlineWriteDataListBox, inputComponent as CiReal, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                        case "String": DisplayComponent(onlineWriteDataListBox, inputComponent as CiString, communication.PlcConfiguration.PlcWriteStartAddress);
                            break;
                    }
                }
            })));
        }

        private static void DisplayComponent(ListBox listBox, CiBitArray component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBB" + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiInteger component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBB" + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiDoubleInteger component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBB" + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiReal component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBB" + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }

        private static void DisplayComponent(ListBox listBox, CiString component, int plcStartAddress)
        {
            int address = plcStartAddress + component.Pos;
            listBox.Items.Add("DBB" + address + " : " + component.Name + " : " + component.Type + " : " + component.Value);
        }
    }
}