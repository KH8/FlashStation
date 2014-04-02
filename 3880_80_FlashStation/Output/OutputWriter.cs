using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CsvHelper;
using _3880_80_FlashStation.DataAquisition;

namespace _3880_80_FlashStation.Output
{
    abstract class OutputWriter
    {
        public string FilePath { get; set; }

        public abstract void CreateOutput(string fixedName, List<string> elementsList);

        internal static string FileNameCreator(string fixedName, string extension)
        {
            const string directoryPath = "Output";
            if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
            return directoryPath + "\\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" 
                + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second 
                + "_" + fixedName + "." + extension;
        }

        public List<string> InterfaceToStrings(CommunicationInterfaceComposite inputComposite, int startPos, int stopPos)
        {
            var list = new List<string>();
            foreach (CommunicationInterfaceComponent variable in inputComposite.Children.Where(variable => variable.Pos >= startPos && variable.Pos < stopPos))
            {
                switch (variable.Type)
                {
                    case "BitArray":
                        var variableCastedBitArray = (CiBitArray) variable;
                        list.Add(variableCastedBitArray.Pos + ";" + variableCastedBitArray.Name + ";" + variableCastedBitArray.Type + ";" +
                                 variableCastedBitArray.Value);
                        break;
                    case "Integer":
                        var variableCastedInteger = (CiInteger) variable;
                        list.Add(variableCastedInteger.Pos + ";" + variableCastedInteger.Name + ";" + variableCastedInteger.Type + ";" +
                                 variableCastedInteger.Value);
                        break;
                    case "DoubleInteger":
                        var variableCastedDoubleInteger = (CiDoubleInteger) variable;
                        list.Add(variableCastedDoubleInteger.Pos + ";" + variableCastedDoubleInteger.Name + ";" + variableCastedDoubleInteger.Type + ";" +
                                 variableCastedDoubleInteger.Value);
                        break;
                    case "Real":
                        var variableCastedReal = (CiReal) variable;
                        list.Add(variableCastedReal.Pos + ";" + variableCastedReal.Name + ";" + variableCastedReal.Type + ";" +
                                 variableCastedReal.Value);
                        break;
                    case "String":
                        var variableCastedString = (CiString) variable;
                        list.Add(variableCastedString.Pos + ";" + variableCastedString.Name + ";" + variableCastedString.Type + ";" +
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
                    string[] linecomponents = line.Split(';');

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
                    string[] linecomponents = line.Split(';');

                    writer.WriteField("Position");
                    writer.WriteField(linecomponents[0]);
                    writer.WriteField("Name");
                    writer.WriteField(linecomponents[1]);
                    writer.WriteField("Type");
                    writer.WriteField(linecomponents[2]);
                    writer.WriteField("Value");
                    writer.WriteField(linecomponents[3]);
                    writer.NextRecord();
                }
                streamWriter.Close();
            }
        }
    }

    class OutputXlsWriter : OutputWriter
    {
        public override void CreateOutput(string fixedName, List<string> elementsList)
        {
            throw new NotImplementedException();
        }
    }
}
