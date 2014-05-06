namespace _3880_80_FlashStation.Visual.Gui
{
    #region Abstract Class

    internal abstract class Gui
    {
        private uint _id;
        private int _xPosition;
        private int _yPosition;

        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int XPosition
        {
            get { return _xPosition; }
            set { _xPosition = value; }
        }

        public int YPosition
        {
            get { return _yPosition; }
            set { _yPosition = value; }
        }

        public abstract void Initialize(uint id, int xPosition, int yPosition);
        public abstract void MakeVisible(uint id);
        public abstract void MakeInvisible(uint id);
    }

    #endregion
}
