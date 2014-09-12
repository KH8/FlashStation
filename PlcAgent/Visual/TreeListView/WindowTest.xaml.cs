using System.Collections.ObjectModel;
using _PlcAgent.DataAquisition;
using _PlcAgent.Visual.Gui;
using _PlcAgent.Visual.Gui.DataAquisition;

namespace _PlcAgent.Visual.TreeListView
{
    /// <summary>
    /// Interaction logic for WindowTest.xaml
    /// </summary>
    public partial class WindowTest
    {
        public CommunicationInterfaceHandler CommunicationInterfaceHandler { get; set; }

        public WindowTest(CommunicationInterfaceHandler ciHandler)
        {
            InitializeComponent();
            CommunicationInterfaceHandler = ciHandler;

            var component = new GuiComponent(0, "", new GuiCommunicationInterfaceOnlineHierarchical(CommunicationInterfaceHandler));
            component.Initialize(0, 0, GeneralGrid);

            //CreateTreeStructure();
        }

        private void CreateTreeStructure()
        {
            //new DisplayDataHierarchicalBuilder().Build(TestTreeListView.Items, TestTreeListView2.Items, CommunicationInterfaceHandler);
        }
    }
}
