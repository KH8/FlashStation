using System.Windows.Controls;
using _PlcAgent.DataAquisition;
using _PlcAgent.PLC;

namespace _PlcAgent.Visual.TreeListView
{
    /// <summary>
    /// Interaction logic for WindowTest.xaml
    /// </summary>
    public partial class WindowTest
    {
        public CommunicationInterfaceComposite Composite { get; set; } 

        public WindowTest(CommunicationInterfaceComposite composite)
        {
            InitializeComponent();
            Composite = composite;

            CreateTreeStructure();
        }

        private void CreateTreeStructure()
        {
            StepDownComposite(TestTreeListView.Items, Composite);
        }

        private void StepDownComposite(ItemCollection items, CommunicationInterfaceComposite composite)
        {
            foreach (var component in composite)
            {
                var actualItemCollection = items;

                if (component.GetType() == typeof (CommunicationInterfaceComposite))
                {
                    var compositeComponent = (CommunicationInterfaceComposite) component;

                    var header = new TreeListViewItem {Header = compositeComponent};
                    actualItemCollection.Add(header);

                    StepDownComposite(header.Items, compositeComponent);
                }
                else
                {
                    actualItemCollection.Add(component);
                }
            }
        }
    }
}
