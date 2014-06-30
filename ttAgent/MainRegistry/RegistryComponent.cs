using _ttAgent.General;

namespace _ttAgent.MainRegistry
{
    public abstract class RegistryComponent
    {
        public struct RegistryComponentHeader
        {
            public uint Id;
            public string Name;
        }

        public RegistryComponentHeader Header;

        protected RegistryComponent(uint id, string name)
        {
            Header = new RegistryComponentHeader
            {
                Id = id,
                Name = name
            };
        }  
    }
}