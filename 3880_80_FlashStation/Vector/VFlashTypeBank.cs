using System.Collections.Generic;
using System.Linq;

namespace _3880_80_FlashStation.Vector
{
    public abstract class VFlashType
    {
        private uint _type;
        private string _path;

        public uint Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
    
    class VFlashTypeBank : VFlashType
    {
        private List<VFlashType> _children = new List<VFlashType>();

        public List<VFlashType> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public void Add(VFlashType c)
        {
            var child = _children.FirstOrDefault(typeFound => typeFound.Type == c.Type);
            if (child == null) _children.Add(c);
            else{ child.Path = c.Path;}
        }

        public void Remove(VFlashType c)
        {
            _children.Remove(c);
        }

        public string ReturnPath(uint type)
        {
            var child = _children.FirstOrDefault(typeFound => typeFound.Type == type);
            return child != null ? child.Path : null;
        }
    }

    class VFlashTypeComponent : VFlashType
    {
        public VFlashTypeComponent(uint type, string path)
        {
            Type = type;
            Path = path;
        }
    }
}
