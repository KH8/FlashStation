using System.Linq;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Output.Template
{
    static class OutputDataTemplateBuilder
    {
        public static OutputDataTemplateComponent ComponentConverter(
            CommunicationInterfaceComponent communicationInterfaceComponent)
        {
            var lastName = communicationInterfaceComponent.LastName.Replace('[', '_');
            lastName = lastName.Replace(']', '_');

            OutputDataTemplateComponent newComposite = new OutputDataTemplateComposite(lastName, null, communicationInterfaceComponent);

            if (communicationInterfaceComponent.GetType() == typeof (CommunicationInterfaceComposite))
            {
                foreach (var component in ((CommunicationInterfaceComposite)communicationInterfaceComponent).Cast<CommunicationInterfaceComponent>())
                {
                    newComposite.Add(ComponentConverter(component));
                }
            }
            else
            {
                newComposite = new OutputDataTemplateLeaf(lastName, null, OutputDataTemplateComponent.OutputDataTemplateComponentType.XmlWriterVariable, communicationInterfaceComponent);
            }

            return newComposite;
        }
    }
}
