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
            var dummyTree = new TreeListView();
            DisplayDataBuilder.Build(TestTreeListView.Items, dummyTree.Items, CommunicationInterfaceHandler);

        }
    }
}
