using System;
using System.Linq;
using System.Xml;
using _PlcAgent.DataAquisition;
using _PlcAgent.MainRegistry;

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

            OutputDataTemplateComponent actualTemplate = template;
            CommunicationInterfaceHandler actualCommunicationInterfaceHandler = null;
            uint actualId = 0;

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        OutputDataTemplateComponent newTemplate = new OutputDataTemplateComposite(xmlReader.Name, null, new CommunicationInterfaceComposite(xmlReader.Name));
                        
                        if (!xmlReader.IsEmptyElement)
                        {
                            actualTemplate.Add(newTemplate);
                            actualTemplate = newTemplate;
                            break;
                        }
                        
                        var id = Convert.ToUInt32(xmlReader.GetAttribute("InterfaceId"), 16);
                        if (id != actualId)
                        {
                            actualCommunicationInterfaceHandler = (CommunicationInterfaceHandler) RegistryContext.Registry.CommunicationInterfaceHandlers.ReturnComponent(id);
                            actualId = id;
                        }

                        CommunicationInterfaceComponent component = new CiInteger("N/A", -1,
                            CommunicationInterfaceComponent.VariableType.Integer, -1);
                        if (actualCommunicationInterfaceHandler != null) component = actualCommunicationInterfaceHandler.ReadInterfaceComposite.ReturnComponent(xmlReader.GetAttribute("Name"));
                        
                        newTemplate = new OutputDataTemplateLeaf(xmlReader.Name, null, OutputDataTemplateComponent.OutputDataTemplateComponentType.XmlWriterVariable, component);
                        actualTemplate.Add(newTemplate);
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
