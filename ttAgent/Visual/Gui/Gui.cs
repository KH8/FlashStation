using System.Windows.Controls;
using _ttAgent.MainRegistry;

namespace _ttAgent.Visual.Gui
{
    #region Abstract Class

    public abstract class Gui : RegistryComponent
    {
        private int _xPosition;
        private int _yPosition;

        protected Gui(uint id, string name) : base(id, name){}

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
