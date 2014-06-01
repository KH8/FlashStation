using System.Windows;

namespace _3880_80_FlashStation.Visual
{
    /// <summary>
    /// Interaction logic for FaultReport.xaml
    /// </summary>
    public partial class FaultReportWindow
    {
        public delegate void ClearErrorDelegate();

        private readonly ClearErrorDelegate _clearErrorDelegate;

        public FaultReportWindow(ClearErrorDelegate clearErrorDelegate)
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
