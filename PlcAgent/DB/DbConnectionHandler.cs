using System.Collections.Generic;
using System.Windows.Controls;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.MainRegistry;
using _PlcAgent.Visual.Gui;
using _PlcAgent.Visual.Gui.DB;

namespace _PlcAgent.DB
{
    public class DbConnectionHandler : ExecutiveModule
    {
        #region Properties

        public DbConnectionHandlerFile DbConnectionHandlerFile { get; set; }
        public DbConnectionHandlerInterfaceAssignmentFile DbConnectionHandlerInterfaceAssignmentFile { get; set; }

        public List<DbStoredProcedure> StoredProcedures { get; set; } 

        public override string Description
        {
            get { return Header.Name + " ; assigned components: " + CommunicationInterfaceHandler.Header.Name; }
        }

        #endregion


        #region Constructors

        public DbConnectionHandler(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, DbConnectionHandlerFile dbConnectionHandlerFile, DbConnectionHandlerInterfaceAssignmentFile dbConnectionHandlerInterfaceAssignmentFile)
            : base(id, name, communicationInterfaceHandler)
        {
            DbConnectionHandlerFile = dbConnectionHandlerFile;
            DbConnectionHandlerInterfaceAssignmentFile = dbConnectionHandlerInterfaceAssignmentFile;

            if (dbConnectionHandlerInterfaceAssignmentFile.Assignment == null) dbConnectionHandlerInterfaceAssignmentFile.Assignment = new string[9][];
            Assignment = dbConnectionHandlerInterfaceAssignmentFile.Assignment[Header.Id];
            CreateInterfaceAssignment();
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
        }

        public override void Deinitialize()
        {
        }

        public override void GuiUpdateTemplate(TabControl mainTabControl, TabControl outputTabControl,
            TabControl connectionTabControl, Grid footerGrid)
        {
            var newtabItem = new TabItem { Header = Header.Name };
            outputTabControl.Items.Add(newtabItem);
            outputTabControl.SelectedItem = newtabItem;

            var newScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
            };
            newtabItem.Content = newScrollViewer;

            var newGrid = new Grid();
            newScrollViewer.Content = newGrid;

            var gridDbConnectionHandler = (GuiComponent)RegistryContext.Registry.GuiDbConnectionHandlers.ReturnComponent(Header.Id);
            gridDbConnectionHandler.Initialize(0, 0, newGrid);

            var gridGuiInterfaceAssignment = (GuiComponent)RegistryContext.Registry.GuiDbConnectionHandlerInterfaceAssignmentComponents.ReturnComponent(Header.Id);
            gridGuiInterfaceAssignment.Initialize(402, 0, newGrid);

            newtabItem = new TabItem { Header = Header.Name };
            mainTabControl.Items.Add(newtabItem);
            mainTabControl.SelectedItem = newtabItem;

            newGrid = new Grid();
            newtabItem.Content = newGrid;

            newGrid.Height = Limiter.DoubleLimit(mainTabControl.Height - 32, 0);
            newGrid.Width = Limiter.DoubleLimit(mainTabControl.Width - 10, 0);

            var dbStoredProcedures = (GuiComponent)RegistryContext.Registry.GuiDbStoredProcedures.ReturnComponent(Header.Id);
            dbStoredProcedures.Initialize(0, 0, newGrid);

            var dbStoredProceduresGrid = (GuiDbStoredProcedures)dbStoredProcedures.UserControl;
            dbStoredProceduresGrid.UpdateSizes(newGrid.Height, newGrid.Width);
        }

        protected override void AssignmentFileUpdate()
        {
            DbConnectionHandlerInterfaceAssignmentFile.Assignment[Header.Id] = Assignment;
            DbConnectionHandlerInterfaceAssignmentFile.Save();
        }

        protected override sealed void CreateInterfaceAssignment()
        {
            if (Assignment == null || Assignment.Length == 0) Assignment = new string[4];
            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();

            InterfaceAssignmentCollection.Add(0, "Command", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.In, Assignment);
            InterfaceAssignmentCollection.Add(1, "Life Counter", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(2, "Reply", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(3, "Status", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
        }

        #endregion

    }
}
