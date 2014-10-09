using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _PlcAgent.DataAquisition;
using _PlcAgent.Log;
using _PlcAgent.Output.OutputFileCreator;
using Module = _PlcAgent.General.Module;

namespace _PlcAgent.Output.Template
{
    public class OutputDataTemplate : Module
    {
        public OutputDataTemplateComposite Composite = new OutputDataTemplateComposite("empty", null, null);
        public OutputDataTemplateFile OutputDataTemplateFile;

        public OutputDataTemplate(uint id, string name, OutputDataTemplateFile outputDataTemplateFile) : base(id, name)
        {
            OutputDataTemplateFile = outputDataTemplateFile;
        }

        public override void Initialize()
        {
            Composite = (OutputDataTemplateComposite)OutputDataTemplateBuilder.XmlFileToTemplate(OutputDataTemplateFile.TemplateFiles[Header.Id]);
            Logger.Log("ID: " + Header.Id + " Output Data Template Initialized");
        }

        public override void Deinitialize()
        {
            Logger.Log("ID: " + Header.Id + " Output Data Template Deinitialized");
        }
    }

    #region Component

    public abstract class OutputDataTemplateComponent
    {
        #region Variables

        private readonly OutputDataTemplateComponentType _type;

        #endregion


        #region Constructors

        protected OutputDataTemplateComponent(string name, object reference, OutputDataTemplateComponentType type, CommunicationInterfaceComponent component)
        {
            Name = name;
            Reference = reference;
            _type = type;
            Component = component;
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

        public OutputDataTemplateComposite Parent { get; set; }

        public OutputDataTemplateComponentType Type
        {
            get { return _type; }
        }

        public CommunicationInterfaceComponent Component { get; set; }

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
            component.Parent = this;
            _children.Add(component);
        }

        public override void Remove(OutputDataTemplateComponent component)
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
            throw new CompositeException("Error: Cannot add to a single leaf");
        }

        public override void Remove(OutputDataTemplateComponent c)
        {
            throw new CompositeException("Error: Cannot remove from a single leaf");
        }

        public override OutputDataTemplateComponent ReturnComponent(string name)
        {
            return Name == name ? this : null;
        }

        #endregion

    }

    #endregion
}
