using System.Linq;
using System.Xml;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Output.Template
{
    static class OutputDataTemplateBuilder
    {
        public static OutputDataTemplateComponent ComunicationInterfaceToTemplate(
            CommunicationInterfaceComponent communicationInterfaceComponent)
        {
            var lastName = communicationInterfaceComponent.LastName.Replace('[', '_');
            lastName = lastName.Replace(']', '_');

            OutputDataTemplateComponent newComposite = new OutputDataTemplateComposite(lastName, null, communicationInterfaceComponent);

            if (communicationInterfaceComponent.GetType() == typeof (CommunicationInterfaceComposite))
            {
                foreach (var component in ((CommunicationInterfaceComposite)communicationInterfaceComponent).Cast<CommunicationInterfaceComponent>())
                {
                    newComposite.Add(ComunicationInterfaceToTemplate(component));
                }
            }
            else
            {
                newComposite = new OutputDataTemplateLeaf(lastName, null, OutputDataTemplateComponent.OutputDataTemplateComponentType.XmlWriterVariable, communicationInterfaceComponent);
            }

            return newComposite;
        }

        public static OutputDataTemplateComponent XmlFileToTemplate(
            string fileName)
        {
            var template = new OutputDataTemplateComposite("Template", null, new CommunicationInterfaceComposite("Template"));

            var xmlReader = XmlReader.Create(fileName);
            xmlReader.MoveToContent();

            var actualTemplate = template;

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        var newTemplate = new OutputDataTemplateComposite(xmlReader.Name, null,
                                new CommunicationInterfaceComposite(xmlReader.Name));
                        actualTemplate.Add(newTemplate);
                        if (!xmlReader.IsEmptyElement) actualTemplate = newTemplate;
                        break;
                    case XmlNodeType.EndElement:
                        actualTemplate = actualTemplate.Parent;
                        break;
                }
            }

            return template;
        }
    }
}
