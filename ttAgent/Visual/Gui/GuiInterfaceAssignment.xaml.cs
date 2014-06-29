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

namespace _ttAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiInterfaceAssignment.xaml
    /// </summary>
    public partial class GuiInterfaceAssignment : UserControl
    {
        public GuiInterfaceAssignment(int xPosition, int yPosition)
        {
            InitializeComponent();
            GeneralGrid.Margin = new Thickness(xPosition,yPosition,0,0);
        }
    }
}
