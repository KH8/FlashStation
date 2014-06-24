using System.Collections.Generic;

namespace _ttAgent.MainRegistry
{
    class RegistryComposite : RegistryComponent
    {
        private List<RegistryComponent> _children = new List<RegistryComponent>();

        public List<RegistryComponent> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        // Constructor
        public RegistryComposite(uint id, string name)
            : base(id, name)
        {
        }

        public void Add(RegistryComponent component)
        {
            _children.Add(component);
        }

        public void Remove(RegistryComponent component)
        {
            _children.Remove(component);
        }
    }
}
