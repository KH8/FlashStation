using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
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

        // private Point _storedPosition;

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

            StoredProcedureListBox.Foreground = Brushes.Black;
            ParameterListBox.Foreground = Brushes.Black;

            StoredProcedureListBox.ItemsSource = DbConnectionHandler.StoredProcedures.Items;
            StoredProcedureListBox.SelectedItem = DbConnectionHandler.StoredProcedures.Items.First();
        }

        #endregion


        #region Methods

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            StoredProcedureListBox.Height = height - 27;
            StoredProcedureListBox.Width = 400;
            ParameterListBox.Height = height - 27;
            ParameterListBox.Width = Limiter.DoubleLimit(width - StoredProcedureListBox.Width - 4, 0);

            FooterGrid.Width = Limiter.DoubleLimit(width, 0);
        }

        #endregion


        #region Event Handlers

        public void OnInterfaceUpdatedDelegate()
        {
        }

        private void StoredProcedureSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sp = (DbStoredProcedure)StoredProcedureListBox.SelectedItem;
            ParameterListBox.ItemsSource = sp.SpParameters;
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            DbConnectionHandler.StoredProcedures.Clear();
        }

        private void Import(object sender, RoutedEventArgs e)
        {

        }

        private void Export(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "StoredProcedures",
                DefaultExt = ".xml",
                Filter = "eXtensible Markup Language File (.xml)|*.xml"
            };

            var result = dlg.ShowDialog();

            if (result != true) return;
            DbConnectionHandler.StoredProcedures.Export(dlg.FileName);
        }

        #endregion

    }
}
