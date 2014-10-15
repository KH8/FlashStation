using System;
using _PlcAgent.DB;

namespace _PlcAgent.Visual.Gui.DB
{
    /// <summary>
    /// Interaction logic for GuiOutputFileCreator.xaml
    /// </summary>
    public partial class GuiDbConnectionHandler
    {
        private readonly Boolean _save;

        public GuiDbConnectionHandler(DbConnectionHandler dbConnectionHandler)
            : base(dbConnectionHandler)
        {
            InitializeComponent();

            HeaderGroupBox.Header = "Db Communication " + DbConnectionHandler.Header.Id;
            _save = true;
        }
    }
}
