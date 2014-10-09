using System;
using System.Linq;
using System.Windows;
using System.Xml;
using _PlcAgent.DataAquisition;
using _PlcAgent.MainRegistry;

namespace _PlcAgent.Output.Template
{
    static class OutputDataTemplateBuilder
    {
        public static DataTemplateComponent ComunicationInterfaceToTemplate(
            CommunicationInterfaceComponent communicationInterfaceComponent)
        {
            var lastName = communicationInterfaceComponent.LastName.Replace('[', '_');
            lastName = lastName.Replace(']', '_');

            DataTemplateComponent newComposite = new DataTemplateComposite(lastName, null, null);

            if (communicationInterfaceComponent.GetType() == typeof (CommunicationInterfaceComposite))
            {
                foreach (var component in ((CommunicationInterfaceComposite)communicationInterfaceComponent).Cast<CommunicationInterfaceComponent>())
                {
                    newComposite.Add(ComunicationInterfaceToTemplate(component));
                }
            }
            else
            {
                newComposite = new DataTemplateLeaf(lastName, null, DataTemplateComponent.DataTemplateComponentType.XmlWriterVariable, communicationInterfaceComponent);
            }

            return newComposite;
        }

        public static DataTemplateComponent XmlFileToTemplate(
            string fileName)
        {
            var template = new DataTemplateComposite("Template", null, new CommunicationInterfaceComposite("Template"));

            var xmlReader = XmlReader.Create(fileName);
            xmlReader.MoveToContent();

            DataTemplateComponent actualTemplate = template;
            CommunicationInterfaceHandler actualCommunicationInterfaceHandler = null;
            uint actualId = 0;
            var componentWasNotFound = false;

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        DataTemplateComponent newTemplate = new DataTemplateComposite(xmlReader.Name, null, null);
                        
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

                        if (actualCommunicationInterfaceHandler != null)
                        {
                            var component =
                                actualCommunicationInterfaceHandler.ReadInterfaceComposite.ReturnComponent(
                                    xmlReader.GetAttribute("Name"));
                            newTemplate = new DataTemplateLeaf(xmlReader.Name, null,
                                DataTemplateComponent.DataTemplateComponentType.XmlWriterVariable, component);
                        }
                        else
                        {
                            newTemplate = new DataTemplateLeaf("%component not available", null,
                                DataTemplateComponent.DataTemplateComponentType.XmlWriterVariable, null);
                            componentWasNotFound = true;
                        }

                        actualTemplate.Add(newTemplate);
                        break;
                    case XmlNodeType.EndElement:
                        actualTemplate = actualTemplate.Parent;
                        break;
                }
            }

            if (componentWasNotFound) MessageBox.Show("At leas one of the template components was not found within available interfaces.", "Component was not found");

            return template;
        }
    }
}
