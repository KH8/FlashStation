using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using _PlcAgent.Log;
using _PlcAgent.Output.Template;

namespace _PlcAgent.Output.OutputFileCreator
{
    class XmlFileCreator
    {
        public void CreateOutput(string fileName, OutputDataTemplateComposite outputDataTemplateComposite)
        {
            var settings = new XmlWriterSettings { Indent = true, IndentChars = "\t" };
            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Composite");

                WriteComponentToTheFile(writer, outputDataTemplateComposite);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Logger.Log(fileName + " XML output file created");
        }

        public void WriteComponentToTheFile(XmlWriter writer, OutputDataTemplateComposite outputDataTemplateComposite)
        {
            foreach (var component in outputDataTemplateComposite.Cast<OutputDataTemplateComponent>())
            {
                writer.WriteStartElement(component.Name);
                if (component.GetType() == typeof(OutputDataTemplateComposite))
                {
                    WriteComponentToTheFile(writer, component as OutputDataTemplateComposite);
                }
                else
                {
                    writer.WriteElementString("Position", component.Component.Pos.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("Name", component.Component.Name);
                    writer.WriteElementString("Type", component.Component.Type.ToString());
                    writer.WriteElementString("Value", CleanInvalidXmlChars(component.Component.StringValue()).Trim());
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
