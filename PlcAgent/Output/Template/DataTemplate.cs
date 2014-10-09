using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _PlcAgent.DataAquisition;
using _PlcAgent.Log;
using _PlcAgent.Output.OutputFileCreator;

namespace _PlcAgent.Output.Template
{
    #region Component

    public abstract class DataTemplateComponent
    {
        #region Variables

        private readonly DataTemplateComponentType _type;

        #endregion


        #region Constructors

        protected DataTemplateComponent(string name, object reference, DataTemplateComponentType type, CommunicationInterfaceComponent component)
        {
            Name = name;
            Reference = reference;
            _type = type;
            Component = component;
        }

        #endregion


        #region Properties

        public enum DataTemplateComponentType
        {
            Assignment,
            Composite
        }

        public string Name { get; set; }

        public object Reference { get; set; }

        public DataTemplateComposite Parent { get; set; }

        public DataTemplateComponentType Type
        {
            get { return _type; }
        }

        public CommunicationInterfaceComponent Component { get; set; }

        #endregion


        #region Methods

        public abstract void Add(DataTemplateComponent c);
        public abstract void Remove(DataTemplateComponent c);
        public abstract DataTemplateComponent ReturnComponent(string name);

        #endregion

    }

    #endregion


    #region Composite

    public class DataTemplateComposite : DataTemplateComponent, IEnumerable
    {
        #region Variables

        private List<DataTemplateComponent> _children = new List<DataTemplateComponent>();

        #endregion


        #region Properties

        public List<DataTemplateComponent> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        #endregion


        #region Constructors

        public DataTemplateComposite(string name, object reference, CommunicationInterfaceComponent component)
            : base(name, reference, DataTemplateComponentType.Composite, component)
        { }

        #endregion


        #region Methods

        public override void Add(DataTemplateComponent component)
        {
            component.Parent = this;
            _children.Add(component);
        }

        public override void Remove(DataTemplateComponent component)
        {
            _children.Remove(component);
        }

        public void Export(string fileName)
        {
            new XmlFileCreator().CreateOutput(fileName, this, FileCreator.OutputConfiguration.Template);
            Logger.Log("Output Data Templated exported to the file: " + fileName);
        }

        public void Clear()
        {
            _children.Clear();
        }

        public override DataTemplateComponent ReturnComponent(string name)
        {
            return _children.Select(outputDataTemplateComponent => outputDataTemplateComponent.ReturnComponent(name)).FirstOrDefault(component => component != null);
        }

        public DataTemplateComposite ReturnComposite(string name)
        {
            return _children.Where(outputDataTemplateComponent => outputDataTemplateComponent.GetType() == typeof(DataTemplateComposite)).Cast<DataTemplateComposite>().FirstOrDefault(composite => composite.Name == name);
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

    public class DataTemplateLeaf : DataTemplateComponent
    {
        #region Constructors

        public DataTemplateLeaf(string name, object reference, DataTemplateComponentType type, CommunicationInterfaceComponent component)
            : base(name, reference, type, component)
        {
        }

        #endregion


        #region Methods

        public override void Add(DataTemplateComponent c)
        {
            throw new CompositeException("Error: Cannot add to a single leaf");
        }

        public override void Remove(DataTemplateComponent c)
        {
            throw new CompositeException("Error: Cannot remove from a single leaf");
        }

        public override DataTemplateComponent ReturnComponent(string name)
        {
            return Name == name ? this : null;
        }

        #endregion

    }

    #endregion
}
