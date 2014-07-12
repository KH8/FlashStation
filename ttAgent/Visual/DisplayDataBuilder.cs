using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.DataAquisition;
using _ttAgent.PLC;

namespace _ttAgent.Visual
{
    public class DisplayDataBuilder
    {
        public class DisplayData
        {
            public string Address { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public static void Build(ObservableCollection<DisplayData> onlineReadDataCollection, ObservableCollection<DisplayData> onlineWriteDataCollection, CommunicationInterfaceHandler communicationHandler)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                onlineReadDataCollection.Clear();
                onlineReadDataCollection.Add(new DisplayData { Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadDbNumber, Name = "-", Type = "-", Value = "-" });
                foreach (var inputComponent in communicationHandler.ReadInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiBit, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Byte:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiByte, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Char:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiChar, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Word:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiWord, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.DoubleWord:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiDoubleWord, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Integer:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiInteger, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiDoubleInteger, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Real:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiReal, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.String:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiString, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                    }
                }
                onlineWriteDataCollection.Clear();
                onlineWriteDataCollection.Add(new DisplayData { Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteDbNumber, Name = "-", Type = "-", Value = "-" });
                foreach (var inputComponent in communicationHandler.WriteInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiBit, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Byte:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiByte, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Char:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiChar, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Word:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiWord, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.DoubleWord:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiDoubleWord, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Integer:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiInteger, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiDoubleInteger, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Real:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiReal, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.String:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiString, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                    }
                }
            });
        }

        private static DisplayData DisplayComponent(CiBit component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Address = "DBW " + address + "." + component.BitPosition,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.Value.ToString()
            };
        }

        private static DisplayData DisplayComponent(CiByte component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;

            var data = new byte[1];
            data[0] = component.Value;
            string hex = BitConverter.ToString(data);
            string value = hex.Replace("-", "");

            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = value
            };
        }

        private static DisplayData DisplayComponent(CiChar component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.Value.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static DisplayData DisplayComponent(CiWord component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;

            var data = new byte[4];
            component.Value.CopyTo(data, 0);
            var dataShort = new byte[2];

            dataShort[0] = data[0];
            dataShort[1] = data[1];

            string hex = BitConverter.ToString(dataShort);
            string value = hex.Replace("-", "");

            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = value
            };
        }

        private static DisplayData DisplayComponent(CiDoubleWord component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;

            var data = new byte[8];
            component.Value[0].CopyTo(data, 0);
            component.Value[1].CopyTo(data, 2);

            var dataShort = new byte[4];

            dataShort[0] = data[0];
            dataShort[1] = data[1];
            dataShort[2] = data[2];
            dataShort[3] = data[3];

            string hex = BitConverter.ToString(dataShort);
            string value = hex.Replace("-", "");

            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = value
            };
        }

        private static DisplayData DisplayComponent(CiInteger component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.Value.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static DisplayData DisplayComponent(CiDoubleInteger component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.Value.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static DisplayData DisplayComponent(CiReal component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.Value.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static DisplayData DisplayComponent(CiString component, int plcStartAddress)
        {
            if (component == null) return null;
            int address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.Value.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}