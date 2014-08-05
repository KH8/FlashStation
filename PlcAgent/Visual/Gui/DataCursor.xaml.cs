using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for DataCursor.xaml
    /// </summary>
    public partial class DataCursor : UserControl
    {
        public DataCursor()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
            if(Mouse.LeftButton == MouseButtonState.Pressed) grid.Margin = new Thickness(0, e.GetPosition(this).Y - 10, 0, 0);
        }
    }
}
