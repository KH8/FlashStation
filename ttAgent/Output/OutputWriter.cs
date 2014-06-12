using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using CsvHelper;
using _ttAgent.DataAquisition;
using _ttAgent.Log;

namespace _ttAgent.Output
{
    abstract class OutputWriter
    {
        public string FilePath { get; set; }

        public abstract void CreateOutput(string fixedName, List<string> elementsList);

        internal static string FileNameCreator(string fixedName, string extension)
        {
            const string directoryPath = "Output";

            if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
            return directoryPath + "\\" 
                + DateTime.Now.Year 
                + FillTheStringUp(DateTime.Now.Month.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Day.ToString(CultureInfo.InvariantCulture)) + "_"
                + FillTheStringUp(DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture))
                + FillTheStringUp(DateTime.Now.Second.ToString(CultureInfo.InvariantCulture))  
                + "_" + fixedName + "." + extension;
        }

        internal static string FillTheStringUp(string dateString)
        {
            if (dateString.Length < 2) return "0" + dateString;
            return dateString;
        }

        public List<string> InterfaceToStrings(CommunicationInterfaceComposite inputComposite, int startPos, int stopPos)
        {
            var list = new List<string>();
            foreach (CommunicationInterfaceComponent variable in inputComposite.Children.Where(variable => variable.Pos >= startPos && variable.Pos < stopPos))
            {
                byte[] data;
                string hex;
                string value;
                switch (variable.Type)
                {
                    case CommunicationInterfaceComponent.VariableType.Bit:
                        var variableCastedBit = (CiBit) variable;
                        list.Add(variableCastedBit.Pos + "$" + variableCastedBit.BitPosition + "$" + variableCastedBit.Name + "$" + variableCastedBit.Type + "$" +
                                 variableCastedBit.Value);
                        break;
                    case CommunicationInterfaceComponent.VariableType.Byte:
                        var variableCastedByte = (CiByte) variable;
                        list.Add(variableCastedByte.Pos + "$" + variableCastedByte.Name + "$" + variableCastedByte.Type + "$" +
                                 variableCastedByte.Value);
                        break;
                    case CommunicationInterfaceComponent.VariableType.Char:
                        var variableCastedChar = (CiChar)variable;
                        list.Add(variableCastedChar.Pos + "$" + variableCastedChar.Name + "$" + variableCastedChar.Type + "$" +
                                 variableCastedChar.Value.ToString(CultureInfo.InvariantCulture));
                        break;
                    case CommunicationInterfaceComponent.VariableType.Word:
                        var variableCastedWord = (CiWord) variable;

                        data = new byte[4];
                        variableCastedWord.Value.CopyTo(data, 0);
                        var dataShort = new byte[2];

                        dataShort[0] = data[0];
                        dataShort[1] = data[1];

                        hex = BitConverter.ToString(dataShort);
                        value = hex.Replace("-", "");

                        list.Add(variableCastedWord.Pos + "$" + variableCastedWord.Name + "$" + variableCastedWord.Type + "$" +
                                 value);
                        break;
                    case CommunicationInterfaceComponent.VariableType.DoubleWord:
                        var variableCastedDoubleWord = (CiDoubleWord)variable;

                        data = new byte[4];
                        variableCastedDoubleWord.Value.CopyTo(data, 0);
                        hex = BitConverter.ToString(data);
                        value = hex.Replace("-", "");

                        list.Add(variableCastedDoubleWord.Pos + "$" + variableCastedDoubleWord.Name + "$" + variableCastedDoubleWord.Type + "$" +
                                 value);
                        break;
                    case CommunicationInterfaceComponent.VariableType.Integer:
                        var variableCastedInteger = (CiInteger) variable;
                        list.Add(variableCastedInteger.Pos + "$" + variableCastedInteger.Name + "$" + variableCastedInteger.Type + "$" +
                                 variableCastedInteger.Value);
                        break;
                    case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                        var variableCastedDoubleInteger = (CiDoubleInteger) variable;
                        list.Add(variableCastedDoubleInteger.Pos + "$" + variableCastedDoubleInteger.Name + "$" + variableCastedDoubleInteger.Type + "$" +
                                 variableCastedDoubleInteger.Value);
                        break;
                    case CommunicationInterfaceComponent.VariableType.Real:
                        var variableCastedReal = (CiReal) variable;
                        list.Add(variableCastedReal.Pos + "$" + variableCastedReal.Name + "$" + variableCastedReal.Type + "$" +
                                 variableCastedReal.Value);
                        break;
                    case CommunicationInterfaceComponent.VariableType.String:
                        var variableCastedString = (CiString) variable;
                        list.Add(variableCastedString.Pos + "$" + variableCastedString.Name + "$" + variableCastedString.Type + "$" +
                                 variableCastedString.Value);
                        break;
                }
            }
            return list;
        }
    }

    class OutputXmlWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, List<string> elementsList)
        {
            var fileName = FileNameCreator(fixedName, "xml");

            var settings = new XmlWriterSettings {Indent = true, IndentChars = "\t"};
            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
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
                    writer.WriteElementString("Value", linecomponents[3]);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Logger.Log(fileName + " output file created");
        }
    }

    class OutputCsvWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, List<string> elementsList)
        {
            var fileName = FileNameCreator(fixedName, "csv");
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
                    writer.WriteField("Value"); writer.WriteField(linecomponents[3]);
                    writer.NextRecord();
                }
                streamWriter.Close();
            }
            Logger.Log(fileName + " output file created");
        }
    }

    class OutputXlsWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, List<string> elementsList)
        {
            Logger.Log("*.xls output file created");
        }
    }
}
