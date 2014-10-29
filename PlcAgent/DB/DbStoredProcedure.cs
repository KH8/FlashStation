using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using _PlcAgent.DataAquisition;
using _PlcAgent.Log;
using _PlcAgent.Output;

namespace _PlcAgent.DB
{
    public class DbConnection
    {
        #region Methods

        public static SqlConnection CreateSqlConnection(string serverInstance, string initialCatalog)
        {
            var connectionString = "workstation id=" + Environment.MachineName;
            connectionString += ";packet size=4096";
            connectionString += ";integrated security=SSPI";
            connectionString += ";data source=" + serverInstance;
            connectionString += ";persist security info=False";
            connectionString += ";initial catalog=" + initialCatalog;
            connectionString += ";MultipleActiveResultSets=True";

            return new SqlConnection(connectionString);
        }

        #endregion
    }

    public class DbStoredProcedureList : DbConnectionHandlerComponent
    {
        public List<DbStoredProcedure> Items { get; set; } 

        public DbStoredProcedureList(DbConnectionHandler dbConnectionHandler) : base(dbConnectionHandler)
        {
            Items = new List<DbStoredProcedure>();
        }

        public void Clear()
        {
            Items.Clear();
        }

        public void Import(string fileName)
        {
            Items.Clear();
        }

        public void Export(string fileName)
        {
            new XmlFileCreator().CreateOutput(fileName, this);
            Logger.Log("Stored Procedures exported to the file: " + fileName);
        }
    }

    public class DbStoredProcedure : DbConnectionHandlerComponent
    {
        #region Classes

        public class DbSpParameter
        {
            #region Variables

            private string _name;
            private Type _type = Type.In;
            private CommunicationInterfaceComponent _component;

            #endregion


            #region Properties

            public enum Type
            {
                In,
                Out,
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public Type ParameterType
            {
                get { return _type; }
                set { _type = value; }
            }

            public string InterfaceName
            {
                get
                {
                    if (_component == null) return "-";
                    var owner = (CommunicationInterfaceHandler)_component.GetOwner();
                    return owner == null ? "-" : owner.Header.Name;
                }
            }

            public string InterfaceTyp
            {
                get
                {
                    return _component == null ? "-" : _component.TypeOfInterface.ToString();
                }
            }

            public string ComponentName
            {
                get
                {
                    return _component == null ? "-" : _component.Name;
                }
            }

            public CommunicationInterfaceComponent Component
            {
                get { return _component; }
                set { _component = value; }
            }

            #endregion


            #region Methods

            public SqlParameter CreateSqlParameter()
            {
                return new SqlParameter(_name, _component.Value);
            }

            #endregion

        }

        #endregion


        #region Variables

        private readonly SqlConnection _localConn;

        private string _spName;
        private int _spCommand;
        private List<DbSpParameter> _parameterList;

        #endregion


        #region Parameters

        public string SpName
        {
            get { return _spName; }
            set { _spName = value; }
        }

        public int SpCommand
        {
            get { return _spCommand; }
            set { _spCommand = value; }
        }

        public List<DbSpParameter> SpParameters
        {
            get { return _parameterList; }
            set { _parameterList = value; }
        }

        #endregion


        #region Constructors

        public DbStoredProcedure(string name, int command, DbConnectionHandler dbConnectionHandler)
            : base(dbConnectionHandler)
        {
            _spName = name;
            _spCommand = command;
            _parameterList = new List<DbSpParameter>();

            _localConn =
                DbConnection.CreateSqlConnection(
                    DbConnectionHandler.DbConnectionHandlerFile.DbInstances[DbConnectionHandler.Header.Id],
                    DbConnectionHandler.DbConnectionHandlerFile.InitialCatalogs[DbConnectionHandler.Header.Id]);
        }

        #endregion


        #region Methods

        public virtual DataTable ExecuteSp()
        {
            var cmd = new SqlCommand();
            var dt = new DataTable();
            var da = new SqlDataAdapter();

            //  Make sure that the object has a stored procedure name . . . . . .

            if (String.IsNullOrEmpty(SpName)) return null;

            //   Set up the command object, and tie the called stored
            //   procedure to the SPName object property . . .

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SpName;

            //   The SPPrms object property has all the stored procedure
            //   parameters in a list object, so extract them and add
            //   them to the command object parameters property . . .

            foreach (var sqlPrm in SpParameters.Select(localPrm => localPrm.CreateSqlParameter())) cmd.Parameters.Add(sqlPrm);

            //   Set up the command object connection property . . .

            cmd.Connection = _localConn;

            //   Use try-catch to run the command object (AKA the
            //   stored procedure) and fill the data set . . .

            try
            {
                _localConn.Open();
                dt.Clear();
                da.SelectCommand = cmd;

                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //   Clear out the existing parameter collection - each time
            //   a command object runs, it wants a new parameter
            //   collection. Therefore, throw out the existing
            //   parameters collection after command object
            //   execution . . .

            cmd.Parameters.Clear();

            //   Close the connection property ASAP . . .

            _localConn.Close();

            //   Count the rows in the data table; if it has
            //   at least one row, return that row, otherwise
            //   return Nothing . . .

            return dt.Rows.Count > 0 ? dt : null;
        }

        #endregion

    }
}
