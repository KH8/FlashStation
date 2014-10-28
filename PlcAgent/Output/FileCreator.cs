using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using CsvHelper;
using _PlcAgent.DataAquisition;
using _PlcAgent.DB;
using _PlcAgent.Log;
using _PlcAgent.Output.Template;

namespace _PlcAgent.Output
{
    public abstract class FileCreator
    {
        public enum OutputConfiguration
        {
            Composite,
            Template
        }

        public abstract void CreateOutput(string fileName, DataTemplateComposite outputDataTemplateComposite,
            OutputConfiguration configuration);
    }

    class FileCreatorFactory
    {
        public static FileCreator CreateVariable(string type)
        {
            FileCreator outputWriter = null;
            if (type == null)
            {
                MessageBox.Show("No file type selected!", "Error");
                return null;
            }
            switch (type)
            {
                case "System.Windows.Controls.ComboBoxItem: *.xml":
                    outputWriter = new XmlFileCreator();
                    break;
                case "System.Windows.Controls.ComboBoxItem: *.csv":
                    outputWriter = new CsvFileCreator();
                    break;
            }
            return outputWriter;
        }
    }

    class XmlFileCreator : FileCreator
    {
        public override void CreateOutput(string fileName, DataTemplateComposite outputDataTemplateComposite, OutputConfiguration configuration)
        {
            var settings = new XmlWriterSettings { Indent = true, IndentChars = "\t" };

            fileName += ".xml";
            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(configuration.ToString());

                WriteComponentToTheFile(writer, outputDataTemplateComposite, configuration);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Logger.Log(fileName + " output file created");
        }

        public void CreateOutput(string fileName, DbStoredProcedureList storedProcedures)
        {
            var settings = new XmlWriterSettings { Indent = true, IndentChars = "\t" };

            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("StoredProcedures");

                foreach (var sp in storedProcedures.Items)
                {
                    writer.WriteStartElement("Procedure");

                    writer.WriteAttributeString("Name", sp.SpName);
                    writer.WriteAttributeString("Command", sp.SpCommand.ToString(CultureInfo.InvariantCulture));

                    foreach (var parameter in sp.SpParameters)
                    {
                        writer.WriteStartElement("Parameter");

                        writer.WriteAttributeString("Name", parameter.Name);
                        writer.WriteAttributeString("Component", parameter.Component.Name);

                        var id = 0;
                        var parent = parameter.Component.GetOwner() as CommunicationInterfaceHandler;
                        if (parent != null) id = (int)parent.Header.Id;

                        writer.WriteAttributeString("InterfaceId", id.ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("InterfaceType", parameter.Component.TypeOfInterface.ToString());

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Logger.Log(fileName + " output file created");
        }

        private static void WriteElement(XmlWriter writer, DataTemplateComponent component, OutputConfiguration configuration)
        {
            if (component.Component == null)
            {
                writer.WriteElementString("Position", "n/a");
                writer.WriteElementString("Name", component.Name);
                writer.WriteElementString("Type", "n/a");
                writer.WriteElementString("Value", "n/a");
                return;
            }

            switch (configuration)
            {
                    case OutputConfiguration.Composite:
                        writer.WriteElementString("Position", component.Component.Pos.ToString(CultureInfo.InvariantCulture));
                        writer.WriteElementString("Name", component.Component.Name);
                        writer.WriteElementString("Type", component.Component.TypeOfVariable.ToString());
                        writer.WriteElementString("Value", CleanInvalidXmlChars(component.Component.StringValue()).Trim());
                    break;
                    case OutputConfiguration.Template:
                        writer.WriteAttributeString("Name", component.Component.Name);
                        writer.WriteAttributeString("Reference", "");

                        var id = 0;
                        var parent = component.Component.GetOwner() as CommunicationInterfaceHandler;
                        if (parent != null) id = (int) parent.Header.Id;

                        writer.WriteAttributeString("InterfaceId", id.ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("InterfaceType", component.Component.TypeOfInterface.ToString());
                    break;
            }
        }

        private void WriteComponentToTheFile(XmlWriter writer, DataTemplateComposite outputDataTemplateComposite, OutputConfiguration configuration)
        {
            foreach (var component in outputDataTemplateComposite.Cast<DataTemplateComponent>())
            {
                writer.WriteStartElement(CleanInvalidXmlChars(component.Name));
                if (component.GetType() == typeof(DataTemplateComposite))
                {
                    WriteComponentToTheFile(writer, component as DataTemplateComposite, configuration);
                }
                else
                {
                    WriteElement(writer, component, configuration);
                }
                writer.WriteEndElement();
            }
        }

        private static string CleanInvalidXmlChars(string text)
        {
            const string re = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(text, re, "");
        }
    }

    internal class CsvFileCreator : FileCreator
    {
        public override void CreateOutput(string fileName, DataTemplateComposite outputDataTemplateComposite, OutputConfiguration configuration)
        {
            fileName += ".csv";
            using (var streamWriter = File.AppendText(fileName))
            {
                var writer = new CsvWriter(streamWriter);
                writer.Configuration.Delimiter = ";";

                WriteComponentToTheFile(writer, outputDataTemplateComposite);

                streamWriter.Close();
            }
            Logger.Log(fileName + " output file created");
        }

        private void WriteComponentToTheFile(CsvWriter writer, DataTemplateComposite outputDataTemplateComposite)
        {
            foreach (var component in outputDataTemplateComposite.Cast<DataTemplateComponent>())
            {
                if (component.GetType() == typeof(DataTemplateComposite))
                {
                    writer.WriteField(""); writer.WriteField("");
                    writer.WriteField("Name"); writer.WriteField(component.Name);
                    writer.WriteField(""); writer.WriteField("");
                    writer.WriteField(""); writer.WriteField("");
                    writer.NextRecord();
                    WriteComponentToTheFile(writer, component as DataTemplateComposite);
                }
                else
                {
                    if (component.Component == null)
                    {
                        writer.WriteField("Position"); writer.WriteField("n/a");
                        writer.WriteField("Name"); writer.WriteField(component.Name);
                        writer.WriteField("Type"); writer.WriteField("n/a");
                        writer.WriteField("Value"); writer.WriteField("n/a");
                        writer.NextRecord();
                        break;
                    }

                    writer.WriteField("Position"); writer.WriteField(component.Component.Pos);
                    writer.WriteField("Name"); writer.WriteField(component.Component.Name);
                    writer.WriteField("Type"); writer.WriteField(component.Component.TypeOfVariable.ToString());
                    writer.WriteField("Value"); writer.WriteField(component.Component.StringValue().Trim());
                    writer.NextRecord();
                }
            }
        }
    }
}
