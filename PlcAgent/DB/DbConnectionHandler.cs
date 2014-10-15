using System;
using _PlcAgent.MainRegistry;

namespace _PlcAgent.DB
{
    class DbConnectionHandler : ExecutiveModule
    {
        #region Properties

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        #endregion


        #region Constructors

        public DbConnectionHandler(uint id, string name) : base(id, name)
        {
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Deinitialize()
        {
            throw new NotImplementedException();
        }

        protected override void AssignmentFileUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void CreateInterfaceAssignment()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
