using _PlcAgent.General;

namespace _PlcAgent.Output.OutputFileCreator
{
    class OutputFileCreator : OutputModule
    {
        public OutputFileCreator(uint id, string name) : base(id, name)
        {
        }

        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public override void Deinitialize()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateAssignment()
        {
            throw new System.NotImplementedException();
        }

        protected override bool CheckInterface()
        {
            throw new System.NotImplementedException();
        }

        protected override void CreateInterfaceAssignment(uint id, string[][] assignment)
        {
            throw new System.NotImplementedException();
        }
    }
}
