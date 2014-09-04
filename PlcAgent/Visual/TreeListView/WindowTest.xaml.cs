using System;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Visual.TreeListView
{
    /// <summary>
    /// Interaction logic for WindowTest.xaml
    /// </summary>
    public partial class WindowTest
    {
        public WindowTest()
        {
            InitializeComponent();
            TestTreeListView.Items.Add(new DisplayDataBuilder.DisplayData
            {
                Component = new CiReal("sdas", 0, CommunicationInterfaceComponent.VariableType.Real, 0.9f),
                Address = "0",
                Name = "testVal",
                Type = "asd",
                Value = "000"
            });
            TestTreeListView.Items.Add(new DisplayDataBuilder.DisplayData
            {
                Component = new CiReal("sdas", 0, CommunicationInterfaceComponent.VariableType.Real, 0.9f),
                Address = "0",
                Name = "testVal",
                Type = "asd",
                Value = "000"
            });
            TestTreeListView.Items.Add(new DisplayDataBuilder.DisplayData
            {
                Component = new CiReal("sdas", 0, CommunicationInterfaceComponent.VariableType.Real, 0.9f),
                Address = "0",
                Name = "testVal",
                Type = "asd",
                Value = "000"
            });
            var testHeader = new TreeListViewItem
            {
                Header = new DisplayDataBuilder.DisplayData
                {
                    Component = new CiReal("sdas", 0, CommunicationInterfaceComponent.VariableType.Real, 0.9f),
                    Address = "",
                    Name = "testasdasdasdasdasdVal",
                    Type = "",
                    Value = ""
                } 
            };
            testHeader.Items.Add(new DisplayDataBuilder.DisplayData
            {
                Component = new CiReal("sdas", 0, CommunicationInterfaceComponent.VariableType.Real, 0.9f),
                Address = "0",
                Name = "testVal",
                Type = "asd",
                Value = "000"
            });
            testHeader.Items.Add(new DisplayDataBuilder.DisplayData
            {
                Component = new CiReal("sdas", 0, CommunicationInterfaceComponent.VariableType.Real, 0.9f),
                Address = "0",
                Name = "testVal",
                Type = "asd",
                Value = "000"
            });
            testHeader.Items.Add(new DisplayDataBuilder.DisplayData
            {
                Component = new CiReal("sdas", 0, CommunicationInterfaceComponent.VariableType.Real, 0.9f),
                Address = "0",
                Name = "testVal",
                Type = "asd",
                Value = "000"
            });
            TestTreeListView.Items.Add(testHeader);
            TestTreeListView.Items.Add(new DisplayDataBuilder.DisplayData
            {
                Component = new CiReal("sdas", 0, CommunicationInterfaceComponent.VariableType.Real, 0.9f),
                Address = "0",
                Name = "testVal",
                Type = "asd",
                Value = "000"
            });
        }
    }
}
