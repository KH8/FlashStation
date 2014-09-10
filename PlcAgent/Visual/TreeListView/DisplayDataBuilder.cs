using System.Collections.ObjectModel;
using System.Windows;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Visual.TreeListView
{
    public class DisplayDataBuilder
    {
        public class DisplayData
        {
            public CommunicationInterfaceComponent Component;
            public string Address { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }

            public void Update()
            {
                if(Component != null) Value = Component.StringValue();
            }
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
                        default:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
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
                        default:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                    }
                }
            });
        }

        public static DisplayData DisplayComponent(CiBit component, int plcStartAddress)
        {
            if (component == null) return null;
            var address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Component = component,
                Address = "DBW " + address + "." + component.BitPosition,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.StringValue()
            };
        }

        public static DisplayData DisplayComponent(CommunicationInterfaceComponent component, int plcStartAddress)
        {
            if (component == null) return null;
            var address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Component = component,
                Address = "DBW " + address,
                Name = component.Name,
                Type = component.Type.ToString(),
                Value = component.StringValue()
            };
        }
    }
}