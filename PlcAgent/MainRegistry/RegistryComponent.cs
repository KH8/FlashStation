namespace _PlcAgent.MainRegistry
{
    public abstract class RegistryComponent
    {
        public struct RegistryComponentHeader
        {
            public uint Id;
            public string Name;
        }

        public abstract string Description { get; }

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