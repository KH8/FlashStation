using System.Windows;
using System.Windows.Controls;
using _ttAgent.Vector;

namespace _ttAgent.Visual.Gui
{
    internal class GuiVFlashHandlerComponent : Gui
    {
        public GuiVFlashHandler GuiVFlashHandler;

        public GuiVFlashHandlerComponent(uint id, string name, VFlashHandler vFlashHandler) : base(id, name)
        {
            GuiVFlashHandler = new GuiVFlashHandler(id, name, vFlashHandler);
        }

        public override void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            GuiVFlashHandler.Initialize(xPosition, yPosition, generalGrid);
        }

        public override void MakeVisible()
        {
            GuiVFlashHandler.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible()
        {
            GuiVFlashHandler.Visibility = Visibility.Hidden;
        }
    }
}
