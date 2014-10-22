using System;
using System.Collections.Generic;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.MainRegistry;

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
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Deinitialize()
        {
            throw new NotImplementedException();
        }

        protected override void AssignmentFileUpdate()
        {
            DbConnectionHandlerInterfaceAssignmentFile.Assignment[Header.Id] = Assignment;
            DbConnectionHandlerInterfaceAssignmentFile.Save();
        }

        protected override void CreateInterfaceAssignment()
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
