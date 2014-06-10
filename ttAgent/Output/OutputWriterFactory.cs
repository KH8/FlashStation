using System.Windows;

namespace ttAgent.Output
{
    class OutputWriterFactory
    {
        public static OutputWriter CreateVariable(string type)
        {
            OutputWriter outputWriter = null;
            if (type == null)
            {
                MessageBox.Show("No file type selected!", "Error");
                return null;
            }
            switch (type)
            {
                case "System.Windows.Controls.ComboBoxItem: *.xml":
                    outputWriter = new OutputXmlWriter();
                    break;
                case "System.Windows.Controls.ComboBoxItem: *.csv":
                    outputWriter = new OutputCsvWriter();
                    break;
                case "System.Windows.Controls.ComboBoxItem: *.xls":
                    outputWriter = new OutputXlsWriter();
                    break;
            }
            return outputWriter;
        }
    }
}
