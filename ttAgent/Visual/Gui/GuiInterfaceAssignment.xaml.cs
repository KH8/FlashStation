using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.General;
using _ttAgent.MainRegistry;

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiInterfaceAssignment
    {
        private int _xPosition;
        private int _yPosition;
        private Grid _generalGridMemory = new Grid();

        private readonly Module _module;

        public int XPosition
        {
            get { return _xPosition; }
            set { _xPosition = value; }
        }

        public int YPosition
        {
            get { return _yPosition; }
            set { _yPosition = value; }
        }

        public ObservableCollection<InterfaceAssignment> InterfaceAssignmentCollection;
        public RegistryComponent.RegistryComponentHeader Header;

        public GuiInterfaceAssignment(uint id, string name, Module module)
        {
            Header = new RegistryComponent.RegistryComponentHeader
            {
                Id = id,
                Name = name
            };

            _module = module;
            InterfaceAssignmentCollection = module.InterfaceAssignmentCollection.Children; 
        }

        public void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            InitializeComponent();
            GeneralGrid.Margin = new Thickness(xPosition, yPosition, 0, 0);
            try
            {
                if (_generalGridMemory != null) _generalGridMemory.Children.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            generalGrid.Children.Add(this);
            _generalGridMemory = generalGrid;

            AssignmentDataGrid.ItemsSource = InterfaceAssignmentCollection;
        }

        private void AssignmentChanged(object sender, EventArgs e)
        {
            _module.UpdateAssignment();
        }
    }
}
