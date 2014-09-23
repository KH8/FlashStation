using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _PlcAgent.DataAquisition;

namespace _PlcAgent.Output.Template
{
    class OutputDataTemplate
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

            public abstract string StringValue();

            public abstract void Add(OutputDataTemplateComponent c);
            public abstract void Remove(OutputDataTemplateComponent c);
            public abstract OutputDataTemplateComponent ReturnComponent(string name);

            #endregion

        }

        #endregion


        #region Composite

        public class CommunicationInterfaceComposite : DataAquisition.CommunicationInterfaceComponent, IEnumerable
        {
            #region Variables

            private List<DataAquisition.CommunicationInterfaceComponent> _children = new List<DataAquisition.CommunicationInterfaceComponent>();

            #endregion


            #region Properties

            public List<DataAquisition.CommunicationInterfaceComponent> Children
            {
                get { return _children; }
                set { _children = value; }
            }

            public override object Value
            {
                get { return null; }
                set { }
            }

            public override string StringValue()
            {
                return "";
            }

            #endregion


            #region Constructors

            public CommunicationInterfaceComposite(string name)
                : base(name, 0, DataAquisition.CommunicationInterfaceComponent.VariableType.Composite)
            { }

            #endregion


            #region Methods

            public override void Add(DataAquisition.CommunicationInterfaceComponent component)
            {
                _children.Add(component);
            }

            public override void Remove(DataAquisition.CommunicationInterfaceComponent component)
            {
                _children.Remove(component);
            }

            public override void ReadValue(byte[] valByte)
            {
                foreach (var component in _children)
                {
                    component.ReadValue(valByte);
                }
            }

            public override void WriteValue(byte[] valByte)
            {
                foreach (var component in _children)
                {
                    component.WriteValue(valByte);
                }
            }

            public override DataAquisition.CommunicationInterfaceComponent ReturnComponent(string name)
            {
                return _children.Select(communicationInterfaceComponent => communicationInterfaceComponent.ReturnComponent(name)).FirstOrDefault(component => component != null);
            }

            public DataAquisition.CommunicationInterfaceComposite ReturnComposite(string name)
            {
                return _children.Where(communicationInterfaceComponent => communicationInterfaceComponent.GetType() == typeof(DataAquisition.CommunicationInterfaceComposite)).Cast<DataAquisition.CommunicationInterfaceComposite>().FirstOrDefault(composite => composite.Name == name);
            }

            public DataAquisition.CommunicationInterfaceVariable ReturnVariable(string name)
            {
                DataAquisition.CommunicationInterfaceVariable variable;

                try { variable = (DataAquisition.CommunicationInterfaceVariable)ReturnComponent(name); }
                catch (Exception) { variable = null; }

                return variable;
            }

            #region ModifyValue Methods

            public void ModifyValue(string name, BitArray bitArrayValue)
            {
                foreach (var componentWord in from communicationInterfaceComponent in _children let component = communicationInterfaceComponent where component.Name == name select (CiWord)communicationInterfaceComponent)
                {
                    componentWord.Value = bitArrayValue;
                    return;
                }
                throw new CompositeException("Error: Variable not found");
            }

            public void ModifyValue(string name, Int16 integerValue)
            {
                foreach (var componentInteger in (from communicationInterfaceComponent in _children let component = communicationInterfaceComponent where component.Name == name select communicationInterfaceComponent).Cast<CiInteger>())
                {
                    componentInteger.Value = integerValue;
                    return;
                }
                throw new CompositeException("Error: Variable not found");
            }

            public void ModifyValue(string name, Int32 doubleIntegerValue)
            {
                foreach (var componentDoubleInteger in (from communicationInterfaceComponent in _children let component = communicationInterfaceComponent where component.Name == name select communicationInterfaceComponent).Cast<CiDoubleInteger>())
                {
                    componentDoubleInteger.Value = doubleIntegerValue;
                    return;
                }
                throw new CompositeException("Error: Variable not found");
            }

            public void ModifyValue(string name, float realValue)
            {
                foreach (var componentReal in from communicationInterfaceComponent in _children let component = communicationInterfaceComponent where component.Name == name select (CiReal)communicationInterfaceComponent)
                {
                    componentReal.Value = realValue;
                    return;
                }
                throw new CompositeException("Error: Variable not found");
            }

            public void ModifyValue(string name, string stringValue)
            {
                foreach (var componentString in (from communicationInterfaceComponent in _children let component = communicationInterfaceComponent where component.Name == name select communicationInterfaceComponent).Cast<CiString>())
                {
                    componentString.Value = stringValue;
                    return;
                }
                throw new CompositeException("Error: Variable not found");
            }

            #endregion

            #endregion

            #region Interface

            public IEnumerator GetEnumerator()
            {
                return Children.GetEnumerator();
            }

            #endregion

        }

        #endregion

        #region Variables

        public abstract class CommunicationInterfaceVariable : DataAquisition.CommunicationInterfaceComponent
        {
            #region Constructors

            protected CommunicationInterfaceVariable(string name, int pos, DataAquisition.CommunicationInterfaceComponent.VariableType type)
                : base(name, pos, type)
            {
            }

            #endregion


            #region Methods

            public override DataAquisition.CommunicationInterfaceComponent ReturnComponent(string name)
            {
                return Name == name ? this : null;
            }

            public override void Add(DataAquisition.CommunicationInterfaceComponent c)
            {
                throw new CompositeException("Error: Cannot add to a single variable");
            }

            public override void Remove(DataAquisition.CommunicationInterfaceComponent c)
            {
                throw new CompositeException("Error: Cannot remove from a single variable");
            }

            #endregion

        }
    }
}
