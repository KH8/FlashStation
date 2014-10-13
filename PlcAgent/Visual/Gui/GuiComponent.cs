using System;
using System.Windows;
using System.Windows.Controls;
using _PlcAgent.General;

namespace _PlcAgent.Visual.Gui
{
    public class GuiComponent : Module
    {
        #region Variables

        private Grid _generalGridMemory = new Grid();

        #endregion


        #region Properties

        public readonly UserControl UserControl;

        public override string Description
        {
            get { return "GuiComponent"; }
        }

        #endregion


        #region Constructors

        public GuiComponent(uint id, string name, UserControl userControl)
            : base(id, name)
        {
            UserControl = userControl;
        }

        #endregion


        #region Methods

        public void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            UserControl.Margin = new Thickness(xPosition, yPosition, 0, 0);
            try
            {
                if (_generalGridMemory != null) _generalGridMemory.Children.Remove(UserControl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            generalGrid.Children.Add(UserControl);
            _generalGridMemory = generalGrid;
        }

        public override void Initialize()
        {}

        public override void Deinitialize()
        {}

        public void MakeVisible()
        {
            UserControl.Visibility = Visibility.Visible;
        }

        public void MakeInvisible()
        {
            UserControl.Visibility = Visibility.Hidden;
        }

        #endregion

    }
}
