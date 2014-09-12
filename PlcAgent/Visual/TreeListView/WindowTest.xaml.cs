using System.Collections.ObjectModel;
using _PlcAgent.DataAquisition;

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

            CreateTreeStructure();
        }

        private void CreateTreeStructure()
        {
            new DisplayDataHierarchicalBuilder().Build(TestTreeListView.Items, TestTreeListView2.Items, CommunicationInterfaceHandler);
        }
    }
}
