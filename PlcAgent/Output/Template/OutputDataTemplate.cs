using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Output.Template
{
    #region Component

    public abstract class OutputDataTemplateComponent
    {
        #region Variables

        private readonly CommunicationInterfaceComponent _communicationInterfaceComponent;
        private readonly OutputDataTemplateComponentType _type;

        #endregion


        #region Constructors

        protected OutputDataTemplateComponent(string name, object reference, OutputDataTemplateComponentType type, CommunicationInterfaceComponent component)
        {
            Name = name;
            Reference = reference;
            _type = type;
            _communicationInterfaceComponent = component;
        }

        #endregion


        #region Properties

        public enum OutputDataTemplateComponentType
        {
            XmlWriterVariable,
            Composite
        }

        public string Name { get; set; }

        public object Reference { get; set; }

        public OutputDataTemplateComponentType Type
        {
            get { return _type; }
        }

        public CommunicationInterfaceComponent Component
        {
            get { return _communicationInterfaceComponent; }
        }

        #endregion


        #region Methods

        public abstract void Add(OutputDataTemplateComponent c);
        public abstract void Remove(OutputDataTemplateComponent c);
        public abstract OutputDataTemplateComponent ReturnComponent(string name);

        #endregion

    }

    #endregion


    #region Composite

    public class OutputDataTemplateComposite : OutputDataTemplateComponent, IEnumerable
    {
        #region Variables

        private List<OutputDataTemplateComponent> _children = new List<OutputDataTemplateComponent>();

        #endregion


        #region Properties

        public List<OutputDataTemplateComponent> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        #endregion


        #region Constructors

        public OutputDataTemplateComposite(string name, object reference, CommunicationInterfaceComponent component)
            : base(name, reference, OutputDataTemplateComponentType.Composite, component)
        { }

        #endregion


        #region Methods

        public override void Add(OutputDataTemplateComponent component)
        {
            _children.Add(component);
        }

        public override void Remove(OutputDataTemplateComponent component)
        {
            _children.Remove(component);
        }

        public override OutputDataTemplateComponent ReturnComponent(string name)
        {
            return _children.Select(outputDataTemplateComponent => outputDataTemplateComponent.ReturnComponent(name)).FirstOrDefault(component => component != null);
        }

        public OutputDataTemplateComposite ReturnComposite(string name)
        {
            return _children.Where(outputDataTemplateComponent => outputDataTemplateComponent.GetType() == typeof(OutputDataTemplateComposite)).Cast<OutputDataTemplateComposite>().FirstOrDefault(composite => composite.Name == name);
        }

        #endregion

        #region Interface

        public IEnumerator GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        #endregion

    }

    #endregion

    #region Leafs

    public class OutputDataTemplateLeaf : OutputDataTemplateComponent
    {
        #region Constructors

        public OutputDataTemplateLeaf(string name, object reference, OutputDataTemplateComponentType type, CommunicationInterfaceComponent component)
            : base(name, reference, type, component)
        {
        }

        #endregion


        #region Methods

        public override void Add(OutputDataTemplateComponent c)
        {

        }

        public override void Remove(OutputDataTemplateComponent c)
        {

        }

        public override OutputDataTemplateComponent ReturnComponent(string name)
        {
            return Name == name ? this : null;
        }

        #endregion

    }

    #endregion
}
