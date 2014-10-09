using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using _PlcAgent.DataAquisition;
using _PlcAgent.Output.Template;

namespace _PlcAgent.Output.OutputFileCreator
{
    internal abstract class FileCreator
    {
        public enum OutputConfiguration
        {
            Composite,
            Template
        }

        public abstract void CreateOutput(string fileName, DataTemplateComposite outputDataTemplateComposite,
            OutputConfiguration configuration);
    }

    class XmlFileCreator : FileCreator
    {
        public override void CreateOutput(string fileName, DataTemplateComposite outputDataTemplateComposite, OutputConfiguration configuration)
        {
            var settings = new XmlWriterSettings { Indent = true, IndentChars = "\t" };
            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(configuration.ToString());

                WriteComponentToTheFile(writer, outputDataTemplateComposite, configuration);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private static void WriteElement(XmlWriter writer, DataTemplateComponent component, OutputConfiguration configuration)
        {
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
                writer.WriteStartElement(component.Name);
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
}
