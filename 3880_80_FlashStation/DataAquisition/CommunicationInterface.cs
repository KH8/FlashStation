using System;
using System.Collections;
using System.Collections.Generic;

namespace _3880_80_FlashStation.DataAquisition
{
    #region Component

    public abstract class CommunicationInterfaceComponent
    {
        private int _id;
        private int _pos;
        private string _type;

        public CommunicationInterfaceComponent(int id, int pos, string type)
        {
            _id = id;
            _pos = pos;
            _type = type;
        }

        public int Id
        {
            get { return _id; }
        }

        public int Pos
        {
            get { return _pos; }
        }

        public string Type
        {
            get { return _type; }
        }

        public abstract void Add(CommunicationInterfaceComponent c);
        public abstract void Remove(CommunicationInterfaceComponent c);
    }

    #endregion

    #region Composite

    public class CommunicationInterfaceComposite : CommunicationInterfaceComponent
    {
        private List<CommunicationInterfaceComponent> _children = new List<CommunicationInterfaceComponent>();

        // Constructor
        public CommunicationInterfaceComposite() : base(0, 0, "Composite")
        {
        }

        public override void Add(CommunicationInterfaceComponent component)
        {
            _children.Add(component);
        }

        public override void Remove(CommunicationInterfaceComponent component)
        {
            _children.Remove(component);
        }

        public CommunicationInterfaceVariable ReturnVariable(int id)
        {
            foreach (CommunicationInterfaceComponent component in _children)
            {
                if (component.Id == id)
                {
                    return (CommunicationInterfaceVariable) component;
                }
            }
            throw new CompositeException("Error: Variable not found");
        }
    }

    #endregion

    #region Variables

    public class CommunicationInterfaceVariable : CommunicationInterfaceComponent
    {
        // Constructor
        public CommunicationInterfaceVariable(int id, int pos, string type)
            : base(id, pos, type)
        {
        }

        public override void Add(CommunicationInterfaceComponent c)
        {
            throw new CompositeException("Error: Cannot add to a single variable");
        }

        public override void Remove(CommunicationInterfaceComponent c)
        {
            throw new CompositeException("Error: Cannot remove from a single variable");
        }
    }

    public class CiBitArray : CommunicationInterfaceVariable
    {
        private BitArray _value;

        public BitArray Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiBitArray(int id, int pos, string type, BitArray value)
            : base(id, pos, type)
        {
            _value = value;
        }
    }

    public class CiInteger : CommunicationInterfaceVariable
    {
        private Int16 _value;

        public Int16 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiInteger(int id, int pos, string type, Int16 value)
            : base(id, pos, type)
        {
            _value = value;
        }
    }

    public class CiDoubleInteger : CommunicationInterfaceVariable
    {
        private Int32 _value;

        public Int32 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiDoubleInteger(int id, int pos, string type, Int32 value)
            : base(id, pos, type)
        {
            _value = value;
        }
    }

    public class CiReal : CommunicationInterfaceVariable
    {
        private float _value;

        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiReal(int id, int pos, string type, float value)
            : base(id, pos, type)
        {
            _value = value;
        }
    }

    public class CiString : CommunicationInterfaceVariable
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiString(int id, int pos, string type, string value)
            : base(id, pos, type)
        {
            _value = value;
        }
    }

    #endregion

    #region Auxiliaries

    public class CompositeException : ApplicationException
    {
        public CompositeException(string info) : base(info) { }
    }

    #endregion
}
