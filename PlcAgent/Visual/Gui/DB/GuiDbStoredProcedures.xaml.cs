using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using _PlcAgent.DB;
using _PlcAgent.General;
using _PlcAgent.Visual.Interfaces;

namespace _PlcAgent.Visual.Gui.DB
{
    /// <summary>
    /// Interaction logic for GuiCommunicationInterfaceOnline.xaml
    /// </summary>
    public partial class GuiDbStoredProcedures : IResizableGui
    {
        #region Variables

        private Point _storedPosition;

        #endregion


        #region Properties

        public TabItem TabItem = new TabItem();

        #endregion


        #region Constructors

        public GuiDbStoredProcedures(DbConnectionHandler dbConnectionHandler)
            : base(dbConnectionHandler)
        {
            InitializeComponent();

            DbConnectionHandler.CommunicationInterfaceHandler.OnInterfaceUpdatedDelegate += OnInterfaceUpdatedDelegate;

            StoredProcedureListBox.View = CreateGridView("StoredProcedure");
            StoredProcedureListBox.Foreground = Brushes.Black;

            ParameterListBox.View = CreateGridView("Parameters");
            ParameterListBox.Foreground = Brushes.Black;

            StoredProcedureListBox.ItemsSource = DbConnectionHandler.StoredProcedures;
            StoredProcedureListBox.SelectedItem = DbConnectionHandler.StoredProcedures.First();
        }

        #endregion


        #region Methods

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            StoredProcedureListBox.Height = height;
            StoredProcedureListBox.Width = 400;
            ParameterListBox.Height = height;
            ParameterListBox.Width = Limiter.DoubleLimit(width - StoredProcedureListBox.Width - 4, 0);

            StoredProcedureListBox.View = CreateGridView("StoredProcedure");
            ParameterListBox.View = CreateGridView("Parameters");
        }

        private static GridView CreateGridView(string configuration)
        {
            var gridView = new GridView();

            switch (configuration)
            {
                case "StoredProcedure" :

                    gridView.Columns.Add(new GridViewColumn
                    {
                        Width = 380,
                        Header = "Stored Procedure Name",
                        DisplayMemberBinding = new Binding("SpName")
                    });

                    break;

                case "Parameters" :

                    gridView.Columns.Add(new GridViewColumn
                    {
                        Width = 200,
                        Header = "Parameter Name",
                        DisplayMemberBinding = new Binding("Name")
                    });

                    gridView.Columns.Add(new GridViewColumn
                    {
                        Width = 400,
                        Header = "Component",
                        DisplayMemberBinding = new Binding("Component.Name")
                    });

                    gridView.Columns.Add(new GridViewColumn
                    {
                        Width = 100,
                        Header = "Type",
                        DisplayMemberBinding = new Binding("Component.Value")
                    });

                    break;
            }

            return gridView;
        }

        #endregion


        #region Event Handlers

        public void OnInterfaceUpdatedDelegate()
        {
        }

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            _storedPosition = e.GetPosition(null);
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void StoredProcedureSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sp = (DbStoredProcedure)StoredProcedureListBox.SelectedItem;
            ParameterListBox.ItemsSource = sp.SpParameters;
        }

        #endregion

    }
}
