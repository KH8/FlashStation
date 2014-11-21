using System.Collections.Generic;
using System.Windows.Controls;
using _PlcAgent.Vector;

namespace _PlcAgent.Visual.Gui.Vector
{
    /// <summary>
    /// Interaction logic for GuiVFlashPathBankTransitionConditions.xaml
    /// </summary>
    public partial class GuiVFlashPathBankTransitionConditions
    {
        public delegate void UpdateDelegate();

        private readonly UpdateDelegate _assignmentDelegate;

        public GuiVFlashPathBankTransitionConditions(IEnumerable<VFlashTypeBank.VFlashTypeComponentStepCondition> conditions, UpdateDelegate updateDelegate)
        {
            InitializeComponent();
            ConditionsDataGrid.ItemsSource = conditions;

            _assignmentDelegate += updateDelegate;
        }

        private void Closeing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConditionsDataGrid.ItemsSource = null;
        }

        private void ConditionChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            var condition = (VFlashTypeBank.VFlashTypeComponentStepCondition)checkBox.DataContext;

            condition.Condition = checkBox.IsChecked.Equals(true);
            _assignmentDelegate();
        }
    }
}
