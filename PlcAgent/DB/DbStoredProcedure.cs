using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace _PlcAgent.DB
{
    public class DbConnection
    {
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
    }

    public class DbStoredProcedure
    {
        private readonly SqlConnection _localConn = DbConnection.CreateSqlConnection("","");

        private string _spName = "";
        private List<SqlParameter> _prmList = new List<SqlParameter>();

        //  Public get-set exposing the private stored procedure name object field . . .

        public string SpName
        {
            get { return _spName; }
            set { _spName = value; }
        }

        public List<SqlParameter> SpPrms
        {
            get { return _prmList; }
            set { _prmList = value; }
        }

        public DbStoredProcedure()
        {
            //   Basic constructor . . .
        }

        public virtual DataTable ExecuteSp()
        {
            var cmd = new SqlCommand();
            var dt = new DataTable();
            var da = new SqlDataAdapter();

            //  Make sure that the object has a stored procedure name . . . . . .

            if (String.IsNullOrEmpty(SpName))
            {
                return null;
            }


            //   Set up the command object, and tie the called stored
            //   procedure to the SPName object property . . .

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = SpName;

            //   The SPPrms object property has all the stored procedure
            //   parameters in a list object, so extract them and add
            //   them to the command object parameters property . . .

            foreach (var localPrm in SpPrms)
            {
                cmd.Parameters.Add(localPrm);
            }

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
    }
}
