using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using _PlcAgent.DataAquisition;

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

    public class DbStoredProcedure : DbConnectionHandlerComponent
    {
        #region Classes

        public class DbSpParameter
        {
            public CommunicationInterfaceComponent Component;
            public string Name;

            public SqlParameter CreateSqlParameter()
            {
                return new SqlParameter(Name, Component.Value);
            }
        }

        #endregion


        #region Variables

        private readonly SqlConnection _localConn;

        private string _spName = "";
        private List<DbSpParameter> _parameterList = new List<DbSpParameter>();

        #endregion


        #region Parameters

        public string SpName
        {
            get { return _spName; }
            set { _spName = value; }
        }

        public List<DbSpParameter> SpParameters
        {
            get { return _parameterList; }
            set { _parameterList = value; }
        }

        #endregion


        #region Constructors

        public DbStoredProcedure(DbConnectionHandler dbConnectionHandler)
            : base(dbConnectionHandler)
        {
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
