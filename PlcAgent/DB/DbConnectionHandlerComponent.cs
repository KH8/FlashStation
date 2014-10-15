using System.ComponentModel;
using System.Windows.Controls;

namespace _PlcAgent.DB
{
    public abstract class DbConnectionHandlerComponent : UserControl
    {
        protected DbConnectionHandler DbConnectionHandler;

        protected void OnPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
        }

        protected DbConnectionHandlerComponent(DbConnectionHandler dbConnectionHandler)
        {
            DbConnectionHandler = dbConnectionHandler;
            DbConnectionHandler.PropertyChanged += OnPropertyChangedEventHandler;
        }
    }
}
