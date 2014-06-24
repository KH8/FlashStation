using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace _ttAgent.MainRegistry
{
    class RegistryComposite : RegistryComponent, IEnumerable
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

        public void Clear()
        {
            _children.Clear();
        }

        public RegistryComponent ReturnComponent(uint id)
        {
            return Children.FirstOrDefault(component => component.Header.Id == id);
        }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
