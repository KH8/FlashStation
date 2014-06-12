using System.Windows.Controls;

namespace _ttAgent.Visual.Gui
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

        public abstract void Initialize(int xPosition, int yPosition, Grid generalGrid);
        public abstract void MakeVisible();
        public abstract void MakeInvisible();
    }

    #endregion
}
