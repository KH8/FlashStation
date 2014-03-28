using System.Windows;

namespace _3880_80_FlashStation.Visual
{
    /// <summary>
    /// Interaction logic for FaultReport.xaml
    /// </summary>
    public partial class FaultReport
    {
        public delegate void ClearErrorDelegate();

        private ClearErrorDelegate _clearErrorDelegate;

        public FaultReport(ClearErrorDelegate clearErrorDelegate)
        {
            InitializeComponent();
            _clearErrorDelegate = clearErrorDelegate;

        }

        private void ClearFaults(object sender, RoutedEventArgs e)
        {
            _clearErrorDelegate();
        }
    }
}
