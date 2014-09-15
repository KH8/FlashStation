using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Visual.TreeListView
{
    abstract public class DisplayDataBuilder
    {
        #region SubClasses

        public class DisplayData
        {
            public CommunicationInterfaceComponent Component;
            public string Address { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class DisplayDataContainer : ItemsControl, IObservable<object>
        {
            public void Update()
            {
                StepDownUpdate(Items);
            }

            private static void StepDownUpdate(IEnumerable items)
            {
                foreach (var item in items)
                {
                    if (item.GetType() == typeof(DisplayData))
                    {
                        var displayData = (DisplayData)item;
                        if (displayData.Component != null) displayData.Value = displayData.Component.StringValue();
                    }
                    if (item.GetType() != typeof (TreeListViewItem)) continue;
                    var treeListViewItem = (TreeListViewItem) item;
                    StepDownUpdate(treeListViewItem.Items);
                }
            }

            public IDisposable Subscribe(IObserver<object> observer)
            {
                return Items.DeferRefresh();
            }
        }

        #endregion


        #region Methods

        public abstract void Build(ItemCollection onlineReadDataStructure, ItemCollection onlineWriteDataStructure, CommunicationInterfaceHandler communicationHandler);

        protected static DisplayData DisplayComponent(CiBit component, int plcStartAddress)
        {
            if (component == null) return null;
            var address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Component = component,
                Address = "DBW " + address + "." + component.BitPosition,
                Name = component.Name,
                LastName = component.LastName,
                Type = component.Type.ToString(),
                Value = component.StringValue()
            };
        }

        protected static DisplayData DisplayComponent(CommunicationInterfaceComponent component, int plcStartAddress)
        {
            if (component == null) return null;
            var address = plcStartAddress + component.Pos;
            return new DisplayData
            {
                Component = component,
                Address = "DBW " + address,
                Name = component.Name,
                LastName = component.LastName,
                Type = component.Type.ToString(),
                Value = component.StringValue()
            };
        }

        protected static DisplayData DisplayComposite(CommunicationInterfaceComposite composite, int plcStartAddress)
        {
            if (composite == null) return null;
            var address = plcStartAddress + composite.Pos;
            return new DisplayData
            {
                Component = composite,
                Address = "DBW " + address,
                Name = composite.Name,
                LastName = composite.LastName,
                Type = "Composite",
                Value = "-"
            };
        }

        #endregion
    }

    public class DisplayDataSimpleBuilder : DisplayDataBuilder
    {
        public override void Build(ItemCollection onlineReadDataStructure, ItemCollection onlineWriteDataStructure,
            CommunicationInterfaceHandler communicationHandler)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                onlineReadDataStructure.Clear();
                onlineReadDataStructure.Add(new DisplayData
                {
                    Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadDbNumber,
                    Name = "-",
                    LastName = "-",
                    Type = "-",
                    Value = "-"
                });
                foreach (var inputComponent in communicationHandler.ReadInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            onlineReadDataStructure.Add(DisplayComponent(inputComponent as CiBit,
                                communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                        default:
                            onlineReadDataStructure.Add(DisplayComponent(inputComponent,
                                communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress));
                            break;
                    }
                }
                onlineWriteDataStructure.Clear();
                onlineWriteDataStructure.Add(new DisplayData
                {
                    Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteDbNumber,
                    Name = "-",
                    LastName = "-",
                    Type = "-",
                    Value = "-"
                });
                foreach (var inputComponent in communicationHandler.WriteInterfaceComposite.Children)
                {
                    switch (inputComponent.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            onlineWriteDataStructure.Add(DisplayComponent(inputComponent as CiBit,
                                communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                        default:
                            onlineWriteDataStructure.Add(DisplayComponent(inputComponent,
                                communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress));
                            break;
                    }
                }
            });
        }
    }

    public class DisplayDataHierarchicalBuilder : DisplayDataBuilder
    {
        public override void Build(ItemCollection onlineReadDataStructure, ItemCollection onlineWriteDataStructure, CommunicationInterfaceHandler communicationHandler)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var header = new TreeListViewItem
                {
                    Header = new DisplayData
                    {
                        Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadDbNumber,
                        Name = "Read Interface",
                        LastName = "Read Interface",
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
                        LastName = "Write Interface",
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

                if (component.GetType() == typeof(CommunicationInterfaceComposite))
                {
                    var compositeComponent = (CommunicationInterfaceComposite)component;
                    var displayData = DisplayComposite(compositeComponent, startAddress);

                    var header = new TreeListViewItem { Header = displayData };
                    actualItemCollection.Add(header);

                    StepDownComposite(header.Items, compositeComponent, startAddress);
                }
                else
                {
                    var variable = (CommunicationInterfaceVariable)component;
                    switch (variable.Type)
                    {
                        case CommunicationInterfaceComponent.VariableType.Bit:
                            actualItemCollection.Add(DisplayComponent(variable as CiBit, startAddress));
                            break;
                        default:
                            actualItemCollection.Add(DisplayComponent(variable, startAddress));
                            break;
                    }
                }
            }
        }
    }
}