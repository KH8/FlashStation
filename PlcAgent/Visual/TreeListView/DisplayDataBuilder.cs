using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Visual.TreeListView
{
    public class DisplayDataBuilder
    {
        #region SubClasses

        public class DisplayData
        {
            public CommunicationInterfaceComponent Component;
            public string Address { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }

            public void Update()
            {
                if (Component != null) Value = Component.StringValue();
            }
        }

        #endregion


        #region Methods

        public static void Build(ObservableCollection<DisplayData> onlineReadDataCollection,
            ObservableCollection<DisplayData> onlineWriteDataCollection,
            CommunicationInterfaceHandler communicationHandler)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                onlineReadDataCollection.Clear();
                onlineReadDataCollection.Add(new DisplayData
                {
                    Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadDbNumber,
                    Name = "-",
                    Type = "-",
                    Value = "-"
                });
                foreach (var inputComponent in communicationHandler.ReadInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent as CiBit,
                                communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        default:
                            onlineReadDataCollection.Add(DisplayComponent(inputComponent,
                                communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                    }
                }
                onlineWriteDataCollection.Clear();
                onlineWriteDataCollection.Add(new DisplayData
                {
                    Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteDbNumber,
                    Name = "-",
                    Type = "-",
                    Value = "-"
                });
                foreach (var inputComponent in communicationHandler.WriteInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent as CiBit,
                                communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        default:
                            onlineWriteDataCollection.Add(DisplayComponent(inputComponent,
                                communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                    }
                }
            });
        }

        public static void Build(ItemCollection onlineReadDataStructure, ItemCollection onlineWriteDataStructure,
            CommunicationInterfaceHandler communicationHandler)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var header = new TreeListViewItem
                {
                    Header = new DisplayData
                    {
                        Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadDbNumber,
                        Name = "Read Interface",
                        Type = "-",
                        Value = "-"
                    },
                    IsExpanded = true
                };
                onlineReadDataStructure.Clear();
                onlineReadDataStructure.Add(header);
                StepDownComposite(header.Items, communicationHandler.ReadInterfaceComposite, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress);

                header = new TreeListViewItem
                {
                    Header = new DisplayData
                    {
                        Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteDbNumber,
                        Name = "Write Interface",
                        Type = "-",
                        Value = "-"
                    },
                    IsExpanded = true
                };
                onlineWriteDataStructure.Clear();
                onlineWriteDataStructure.Add(header);
                StepDownComposite(header.Items, communicationHandler.WriteInterfaceComposite, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress);
            });
        }

        private static void StepDownComposite(ItemCollection items, CommunicationInterfaceComposite composite, int startAddress)
        {
            foreach (var component in composite)
            {
                var actualItemCollection = items;

                if (component.GetType() == typeof (CommunicationInterfaceComposite))
                {
                    var compositeComponent = (CommunicationInterfaceComposite) component;
                    var displayData = DisplayComposite(compositeComponent, startAddress);

                    var header = new TreeListViewItem { Header = displayData };
                    actualItemCollection.Add(header);

                    StepDownComposite(header.Items, compositeComponent, startAddress);
                }
                else
                {
                    var variable = (CommunicationInterfaceVariable) component;
                    DisplayData newComponent;
                    switch (variable.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            actualItemCollection.Add(newComponent = DisplayComponent(variable as CiBit, startAddress));
                            newComponent.Name = variable.LastName;
                            break;
                        default:
                            actualItemCollection.Add(newComponent = DisplayComponent(variable, startAddress));
                            newComponent.Name = variable.LastName;
                            break;
                    }
                }
            }
        }

        private static DisplayData DisplayComponent(CiBit component, int plcStartAddress)
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

        private static DisplayData DisplayComponent(CommunicationInterfaceComponent component, int plcStartAddress)
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

        private static DisplayData DisplayComposite(CommunicationInterfaceComposite composite, int plcStartAddress)
        {
            if (composite == null) return null;
            var address = plcStartAddress + composite.Pos;
            return new DisplayData
            {
                Component = composite,
                Address = "DBW " + address,
                Name = composite.Name,
                Type = "Composite",
                Value = "-"
            };
        }

        #endregion

    }
}