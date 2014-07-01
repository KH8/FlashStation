using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.Log;
using _ttAgent.Output;

namespace _ttAgent.Visual.Gui
{
    class GuiOutputHandlerComponent : Gui
    {
        public GuiOutputHandler GuiOutputHandler;

        public GuiOutputHandlerComponent(uint id, string name, OutputHandler outputHandler) : base(id, name)
        {
            GuiOutputHandler = new GuiOutputHandler(id, name, outputHandler);
        }

        public override void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            GuiOutputHandler.Initialize(xPosition, yPosition, generalGrid);
        }

        public override void MakeVisible()
        {
            GuiOutputHandler.Visibility = Visibility.Visible;
        }

        public override void MakeInvisible()
        {
            GuiOutputHandler.Visibility = Visibility.Hidden;
        }
    }
}
