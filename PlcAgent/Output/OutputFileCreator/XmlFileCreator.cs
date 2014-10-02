using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using _PlcAgent.Output.Template;

namespace _PlcAgent.Output.OutputFileCreator
{
    class XmlFileCreator
    {
        public enum OutputConfiguration
        {
            Composite,
            Template
        }

        public void CreateOutput(string fileName, OutputDataTemplateComposite outputDataTemplateComposite, OutputConfiguration configuration)
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

        private void WriteElement(XmlWriter writer, OutputDataTemplateComponent component, OutputConfiguration configuration)
        {
            switch (configuration)
            {
                    case OutputConfiguration.Composite:
                        writer.WriteElementString("Position", component.Component.Pos.ToString(CultureInfo.InvariantCulture));
                        writer.WriteElementString("Name", component.Component.Name);
                        writer.WriteElementString("Type", component.Component.Type.ToString());
                        writer.WriteElementString("Value", CleanInvalidXmlChars(component.Component.StringValue()).Trim());
                    break;
                    case OutputConfiguration.Template:
                        writer.WriteElementString("Name", component.Component.Name);
                        writer.WriteElementString("Type", component.Component.Type.ToString());
                        writer.WriteElementString("InterfaceId", "");
                        writer.WriteElementString("InterfaceType", "");
                        writer.WriteElementString("Reference", "");
                    break;
            }
        }

        public void WriteComponentToTheFile(XmlWriter writer, OutputDataTemplateComposite outputDataTemplateComposite, OutputConfiguration configuration)
        {
            foreach (var component in outputDataTemplateComposite.Cast<OutputDataTemplateComponent>())
            {
                writer.WriteStartElement(component.Name);
                if (component.GetType() == typeof(OutputDataTemplateComposite))
                {
                    WriteComponentToTheFile(writer, component as OutputDataTemplateComposite, configuration);
                }
                else
                {
                    WriteElement(writer, component, configuration);
                }
                writer.WriteEndElement();
            }
        }

        internal static string CleanInvalidXmlChars(string text)
        {
            const string re = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(text, re, "");
        }
    }
}
