using System;
using System.Collections.ObjectModel;
using System.Linq;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Visual.TreeListView
{
    /// <summary>
    /// Interaction logic for WindowTest.xaml
    /// </summary>
    public partial class WindowTest
    {
        public ObservableCollection<DisplayDataBuilder.DisplayData> Collection { get; set; } 

        public WindowTest(ObservableCollection<DisplayDataBuilder.DisplayData> collection)
        {
            InitializeComponent();
            Collection = collection;

            CreateTreeStructure();
        }

        private void CreateTreeStructure()
        {
            
            TestTreeListView.Items.Add(Collection.First());

            string[] stringsMemory;

            foreach (var displayData in Collection)
            {
                var strings = displayData.Name.Split('.');
            }
        }
    }
}
