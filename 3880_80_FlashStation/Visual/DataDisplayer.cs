using System.Collections.ObjectModel;
using System.Globalization;
using _3880_80_FlashStation.DataAquisition;
using _3880_80_FlashStation.PLC;

namespace _3880_80_FlashStation.Visual
{
    public class DataDisplayer
    {
        public class DisplayData
        {
            public string Address { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public static void Display(ObservableCollection<DisplayData> onlineReadDataCollection, ObservableCollection<DisplayData> onlineWriteDataCollection, PlcCommunicator communication, CommunicationInterfaceHandler communicationHandler)
        {
            onlineReadDataCollection.Clear();
            onlineReadDataCollection.Add(new DisplayData { Address = "DB" + communication.PlcConfiguration.PlcReadDbNumber, Name = "-", Type = "-", Value="-"});
            foreach (CommunicationInterfaceComponent inputComponent in communicationHandler.ReadInterfaceComposite.Children)
            {
                switch (inputComponent.Type)
                {
                    case CommunicationInterfaceComponent.VariableType.Bit: 
                        onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiBit, communication.PlcConfiguration.PlcReadStartAddress));
                        break;
                    case CommunicationInterfaceComponent.VariableType.Byte: 
                        onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiByte, communication.PlcConfiguration.PlcReadStartAddress));
                        break;
                    case CommunicationInterfaceComponent.VariableType.Word:
                        onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiWord, communication.PlcConfiguration.PlcReadStartAddress));
                        break;
                    case CommunicationInterfaceComponent.VariableType.Integer:
                        onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiInteger, communication.PlcConfiguration.PlcReadStartAddress));
                        break;
                    case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                        onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiDoubleInteger, communication.PlcConfiguration.PlcReadStartAddress));
                        break;
                    case CommunicationInterfaceComponent.VariableType.Real:
                        onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiReal, communication.PlcConfiguration.PlcReadStartAddress));
                        break;
                    case CommunicationInterfaceComponent.VariableType.String:
                        onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiString, communication.PlcConfiguration.PlcReadStartAddress));
                        break;
                }
            }
            onlineWriteDataCollection.Clear();
            onlineWriteDataCollection.Add(new DisplayData { Address = "DB" + communication.PlcConfiguration.PlcWriteDbNumber, Name = "-", Type = "-", Value = "-" });
                foreach (CommunicationInterfaceComponent inputComponent in communicationHandler.WriteInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiBit, communication.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Byte:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiByte, communication.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Word:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiWord, communication.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Integer:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiInteger, communication.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiDoubleInteger, communication.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.Real:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiReal, communication.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        case CommunicationInterfaceComponent.VariableType.String:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiString, communication.PlcConfiguration.PlcWriteStartAddress));
                            break;
                    }
                }
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
            return new DisplayData
            {
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.Value.ToString()
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