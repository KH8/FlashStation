using System;
using System.Windows;
using System.Windows.Controls;
using _ttAgent.MainRegistry;

namespace _ttAgent.Visual.Gui
{
    internal class GuiComponent : RegistryComponent
    {
        private Grid _generalGridMemory = new Grid();
        private readonly UserControl _userControl;

        public GuiComponent(uint id, string name, UserControl userControl)
            : base(id, name)
        { _userControl = userControl;}

        public void Initialize(int xPosition, int yPosition, Grid generalGrid)
        {
            _userControl.Margin = new Thickness(xPosition, yPosition, 0, 0);
            try { if (_generalGridMemory != null) _generalGridMemory.Children.Remove(_userControl);}
            catch (Exception e) { Console.WriteLine(e);}
            generalGrid.Children.Add(_userControl);
            _generalGridMemory = generalGrid;
        }

        public void MakeVisible() { _userControl.Visibility = Visibility.Visible; }
        public void MakeInvisible() { _userControl.Visibility = Visibility.Hidden; }
    }
}
