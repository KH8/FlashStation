using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using CsvHelper;
using _ttAgent.DataAquisition;
using _ttAgent.Log;

namespace _ttAgent.Output
{
    public abstract class OutputWriter
    {
        public string FilePath { get; set; }

        public abstract void CreateOutput(string fixedName, string directoryName, List<string> elementsList);

        internal static string FileNameCreator(string fixedName, string directoryPath, string extension)
        {
            if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
            return directoryPath + "\\" 
                + DateTime.Now.Year 
                + FillTheStringUp(DateTime.Now.Month.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Day.ToString(CultureInfo.InvariantCulture)) + "_"
                + FillTheStringUp(DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Second.ToString(CultureInfo.InvariantCulture))  
                + "_" + fixedName.Trim() + "." + extension;
        }

        internal static string FillTheStringUp(string dateString)
        {
            if (dateString.Length < 2) return "0" + dateString;
            return dateString;
        }

        public List<string> InterfaceToStrings(CommunicationInterfaceComposite inputComposite, int startPos, int stopPos)
        {
            var list = new List<string>();
            foreach (var variable in inputComposite.Children.Where(variable => variable.Pos >= startPos && variable.Pos < stopPos))
            {
                switch (variable.Type)
                {
                    case CommunicationInterfaceComponent.VariableType.Bit:
                        var variableCastedBit = (CiBit) variable;
                        list.Add(variableCastedBit.Pos + "." + variableCastedBit.BitPosition + "$" + variableCastedBit.Name + "$" + variableCastedBit.Type + "$" +
                                 variableCastedBit.StringValue());
                        break;
                    default:
                        list.Add(variable.Pos + "$" + variable.Name + "$" + variable.Type + "$" +
                                 variable.StringValue());
                        break;
                }
            }
            return list;
        }
    }

    class OutputXmlWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, string directoryName, List<string> elementsList)
        {
            var fileName = FileNameCreator(fixedName, directoryName, "xml");

            var settings = new XmlWriterSettings {Indent = true, IndentChars = "\t"};
            using (var writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("PLCVariables");

                foreach (string line in elementsList)
                {
                    string[] linecomponents = line.Split('$');

                    writer.WriteStartElement("Variable");
                    writer.WriteElementString("Position", linecomponents[0]);
                    writer.WriteElementString("Name", linecomponents[1]);
                    writer.WriteElementString("Type", linecomponents[2]);
                    linecomponents[3] = CleanInvalidXmlChars(linecomponents[3]);
                    writer.WriteElementString("Value", linecomponents[3].Trim());

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Logger.Log(fileName + " output file created");
        }

        internal static string CleanInvalidXmlChars(string text)
        {
            const string re = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(text, re, "");
        }
    }

    class OutputCsvWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, string directoryName, List<string> elementsList)
        {
            var fileName = FileNameCreator(fixedName, directoryName, "csv");
            using (StreamWriter streamWriter = File.AppendText(fileName))
            {
                var writer = new CsvWriter(streamWriter);
                writer.Configuration.Delimiter = ";";
                foreach (string line in elementsList)
                {
                    string[] linecomponents = line.Split('$');

                    writer.WriteField("Position"); writer.WriteField(linecomponents[0]);
                    writer.WriteField("Name"); writer.WriteField(linecomponents[1]);
                    writer.WriteField("Type"); writer.WriteField(linecomponents[2]);
                    writer.WriteField("Value"); writer.WriteField(linecomponents[3].Trim());
                    writer.NextRecord();
                }
                streamWriter.Close();
            }
            Logger.Log(fileName + " output file created");
        }
    }

    class OutputXlsWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, string directoryName, List<string> elementsList)
        {
            Logger.Log("*.xls output file created");
        }
    }
}
