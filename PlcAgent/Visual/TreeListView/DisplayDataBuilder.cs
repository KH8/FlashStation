using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using _PlcAgent.DataAquisition;
using _PlcAgent.Properties;

namespace _PlcAgent.Visual.TreeListView
{
    abstract public class DisplayDataBuilder
    {
        #region SubClasses

        public class DisplayData : INotifyPropertyChanged
        {
            private string _value;
            public CommunicationInterfaceComponent Component;
            public string Address { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public string Type { get; set; }

            public string Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class DisplayDataContainer : ObservableCollection<object>
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
                        if (displayData.Component != null && displayData.Value != displayData.Component.StringValue()) displayData.Value = displayData.Component.StringValue();
                        continue;
                    }
                    if (item.GetType() != typeof (TreeListViewItem)) continue;
                    var treeListViewItem = (TreeListViewItem) item;
                    StepDownUpdate(treeListViewItem.Items);
                }
            }
        }

        #endregion


        #region Methods

        public abstract void Build(ObservableCollection<object> onlineReadDataStructure, ObservableCollection<object> onlineWriteDataStructure, CommunicationInterfaceHandler communicationHandler);

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
        public override void Build(ObservableCollection<object> onlineReadDataStructure, ObservableCollection<object> onlineWriteDataStructure,
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
                StepDownComposite(onlineReadDataStructure, communicationHandler.ReadInterfaceComposite, communicationHandler.PlcCommunicator.PlcConfiguration.PlcReadStartAddress);

                onlineWriteDataStructure.Clear();
                onlineWriteDataStructure.Add(new DisplayData
                {
                    Address = "DB" + communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteDbNumber,
                    Name = "-",
                    LastName = "-",
                    Type = "-",
                    Value = "-"
                });
                StepDownComposite(onlineWriteDataStructure, communicationHandler.WriteInterfaceComposite, communicationHandler.PlcCommunicator.PlcConfiguration.PlcWriteStartAddress);
            });
        }

        private static void StepDownComposite(ObservableCollection<object> collection, CommunicationInterfaceComposite composite, int startAddress)
        {
            foreach (var component in composite)
            {
                var actualItemCollection = collection;

                if (component.GetType() == typeof(CommunicationInterfaceComposite))
                {
                    var compositeComponent = (CommunicationInterfaceComposite)component;
                    StepDownComposite(collection, compositeComponent, startAddress);
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

    public class DisplayDataHierarchicalBuilder : DisplayDataBuilder
    {
        public override void Build(ObservableCollection<object> onlineReadDataStructure, ObservableCollection<object> onlineWriteDataStructure, CommunicationInterfaceHandler communicationHandler)
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