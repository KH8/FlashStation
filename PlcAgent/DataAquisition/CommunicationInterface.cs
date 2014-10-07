using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace _PlcAgent.DataAquisition
{
    #region Component

    public abstract class CommunicationInterfaceComponent
    {
        #region Constructors

        protected CommunicationInterfaceComponent(string name, int pos, VariableType type)
        {
            Name = name;
            Pos = pos;
            TypeOfVariable = type;
        }

        #endregion


        #region Properties

        public enum VariableType
        {
            Bit,
            Byte,
            Char,
            Word,
            DoubleWord,
            Integer,
            DoubleInteger,
            Real,
            String,
            Composite
        }

        public enum InterfaceType
        {
            ReadInterface,
            WriteInterface,
        }

        public string Name { get; protected set; }

        public string LastName
        {
            get
            {
                var fullName = Name.Split('.');
                return fullName[fullName.Length - 1];
            }
        }

        public int Pos { get; set; }
        public VariableType TypeOfVariable { get; protected set; }
        public InterfaceType TypeOfInterface { get; set; }
        public CommunicationInterfaceComponent Parent { get; set; }
        public object Owner { get; set; }

        public abstract object Value { get; set; }
        

        #endregion


        #region Methods

        public abstract string StringValue();

        public abstract void Add(CommunicationInterfaceComponent c);
        public abstract void Remove(CommunicationInterfaceComponent c);
        public abstract void ReadValue(byte[] valByte);
        public abstract void WriteValue(byte[] valByte);

        public abstract CommunicationInterfaceComponent ReturnComponent(string name);
        
        public object GetOwner()
        {
            return Parent == null ? Owner : Parent.GetOwner();
        }

        #endregion

    }

    #endregion


    #region Composite

    public class CommunicationInterfaceComposite : CommunicationInterfaceComponent, IEnumerable
    {
        #region Variables

        private List<CommunicationInterfaceComponent> _children = new List<CommunicationInterfaceComponent>();

        #endregion


        #region Properties

        public List<CommunicationInterfaceComponent> Children
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
            : base(name, 0, VariableType.Composite)
        {}

        #endregion


        #region Methods

        public override void Add(CommunicationInterfaceComponent component)
        {
            component.Parent = this;
            component.TypeOfInterface = TypeOfInterface;
            _children.Add(component);
        }

        public override void Remove(CommunicationInterfaceComponent component)
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

        public override CommunicationInterfaceComponent ReturnComponent(string name)
        {
            return _children.Select(communicationInterfaceComponent => communicationInterfaceComponent.ReturnComponent(name)).FirstOrDefault(component => component != null);
        }

        public CommunicationInterfaceComposite ReturnComposite(string name)
        {
            return _children.Where(communicationInterfaceComponent => communicationInterfaceComponent.GetType() == typeof (CommunicationInterfaceComposite)).Cast<CommunicationInterfaceComposite>().FirstOrDefault(composite => composite.Name == name);
        }

        public CommunicationInterfaceVariable ReturnVariable(string name)
        {
            CommunicationInterfaceVariable variable;

            try { variable = (CommunicationInterfaceVariable) ReturnComponent(name);}
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

    public abstract class CommunicationInterfaceVariable : CommunicationInterfaceComponent
    {
        #region Constructors

        protected CommunicationInterfaceVariable(string name, int pos, VariableType type)
            : base(name, pos, type)
        {
        }

        #endregion


        #region Methods

        public override CommunicationInterfaceComponent ReturnComponent(string name)
        {
            return Name == name ? this : null;
        }

        public override void Add(CommunicationInterfaceComponent c)
        {
            throw new CompositeException("Error: Cannot add to a single variable");
        }

        public override void Remove(CommunicationInterfaceComponent c)
        {
            throw new CompositeException("Error: Cannot remove from a single variable");
        }

        #endregion

    }

    public class CiBit : CommunicationInterfaceVariable
    {
        #region Variables

        private Boolean _value;
        private int _bitPosition;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (Boolean) value; }
        }

        public int BitPosition
        {
            get { return _bitPosition; }
            set { _bitPosition = value; }
        }

        #endregion


        #region Constructor

        public CiBit(string name, int pos, int bitPos, VariableType type, Boolean value)
            : base(name, pos, type)
        {
            _value = value;
            _bitPosition = bitPos;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            return _value.ToString();
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadSingleBit(valByte, Pos, _bitPosition);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteSingleBit(valByte, Pos, _bitPosition, _value);
        }

        #endregion

    }

    public class CiByte : CommunicationInterfaceVariable
    {
        #region Variables

        private Byte _value;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (byte) value; }
        }

        #endregion


        #region Constructors

        public CiByte(string name, int pos, VariableType type, Byte value)
            : base(name, pos, type)
        {
            _value = value;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            var data = new byte[1];
            data[0] = _value;
            var hex = BitConverter.ToString(data);
            return hex.Replace("-", "");
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.Read8Bits(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.Write8Bits(valByte, Pos, _value);
        }

        #endregion

    }

    public class CiChar : CommunicationInterfaceVariable
    {
        #region Variables

        private Char _value;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (char) value; }
        }

        #endregion


        #region Constructors

        public CiChar(string name, int pos, VariableType type, Char value)
            : base(name, pos, type)
        {
            _value = value;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public override void ReadValue(byte[] valByte)
        {
            var data = DataMapper.Read8Bits(valByte, Pos);
            _value = Convert.ToChar(data);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.Write8Bits(valByte, Pos, Convert.ToByte(_value));
        }

        #endregion

    }

    public class CiWord : CommunicationInterfaceVariable
    {
        #region Variables

        private BitArray _value;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (BitArray) value; }
        }

        #endregion


        #region Constructors

        public CiWord(string name, int pos, VariableType type, BitArray value)
            : base(name, pos, type)
        {
            _value = value;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            var data = new byte[4];
            _value.CopyTo(data, 0);
            var dataShort = new byte[2];

            dataShort[0] = data[0];
            dataShort[1] = data[1];

            var hex = BitConverter.ToString(dataShort);
            return hex.Replace("-", "");
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.Read16Bits(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.Write16Bits(valByte, Pos, _value);
        }

        #endregion

    }

    public class CiDoubleWord : CommunicationInterfaceVariable
    {
        #region Variables

        private BitArray[] _value;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (BitArray[]) value; }
        }

        #endregion


        #region Contructors

        public CiDoubleWord(string name, int pos, VariableType type, BitArray[] value)
            : base(name, pos, type)
        {
            _value = value;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            var data = new byte[8];
            _value[0].CopyTo(data, 0);
            _value[1].CopyTo(data, 2);

            var dataShort = new byte[4];

            dataShort[0] = data[0];
            dataShort[1] = data[1];
            dataShort[2] = data[2];
            dataShort[3] = data[3];

            var hex = BitConverter.ToString(dataShort);
            return hex.Replace("-", "");
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.Read32Bits(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.Write32Bits(valByte, Pos, _value);
        }

        #endregion

    }

    public class CiInteger : CommunicationInterfaceVariable
    {
        #region Variables

        private Int16 _value;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (Int16) value; }
        }

        #endregion


        #region Constructors

        public CiInteger(string name, int pos, VariableType type, Int16 value)
            : base(name, pos, type)
        {
            _value = value;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadInteger(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteInteger(valByte, Pos, _value);
        }

        #endregion

    }

    public class CiDoubleInteger : CommunicationInterfaceVariable
    {
        #region Variables

        private Int32 _value;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (Int32) value; }
        }

        #endregion


        #region Constructors

        public CiDoubleInteger(string name, int pos, VariableType type, Int32 value)
            : base(name, pos, type)
        {
            _value = value;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadDInteger(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteDInteger(valByte, Pos, _value);
        }

        #endregion

    }

    public class CiReal : CommunicationInterfaceVariable
    {
        #region Variables

        private float _value;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (float) value; }
        }

        #endregion


        #region Constructors

        public CiReal(string name, int pos, VariableType type, float value)
            : base(name, pos, type)
        {
            _value = value;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadReal(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteReal(valByte, Pos, _value);
        }

        #endregion

    }

    public class CiString : CommunicationInterfaceVariable
    {
        #region Variables

        private string _value;
        private int _length;

        #endregion


        #region Properties

        public override object Value
        {
            get { return _value; }
            set { _value = (string) value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        #endregion


        #region Constructors

        public CiString(string name, int pos, VariableType type, string value, int length)
            : base(name, pos, type)
        {
            _value = value;
            _length = length;
        }

        #endregion


        #region Methods

        public override string StringValue()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadString(valByte, Pos, _length);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteString(valByte, Pos, _value);
        }

        #endregion

    }

    #endregion

    #region Auxiliaries

    public class CompositeException : ApplicationException
    {
        public CompositeException(string info) : base(info) { }
    }

    #endregion
}
