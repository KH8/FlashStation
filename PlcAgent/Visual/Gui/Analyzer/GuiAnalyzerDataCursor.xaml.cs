using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using _PlcAgent.Properties;
using _PlcAgent.Visual.Interfaces;

namespace _PlcAgent.Visual.Gui.Analyzer
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerDataCursor.xaml
    /// </summary>
    public partial class GuiAnalyzerDataCursor : IResizableGui, INotifyPropertyChanged
    {
        #region Variables

        private Boolean _isSelected;

        private const double LeftLimitPosition = 206.0;
        private double _rightLimitPosition = 1000.0;

        private double _actualPosition;

        private Grid _parentGrid;

        #endregion


        #region Properties

        public Grid ParentGrid
        {
            set
            {
                _parentGrid = value;
                _parentGrid.MouseMove += ParenGrid_OnMouseMove;
            }
        }

        public Brush Brush
        {
            set
            {
                TopHorizontalGrid.Background = value;
                VerticalGrid.Background = value;
                BottomHorizontalGrid.Background = value;
                PositionLabel.Foreground = value;
            }
        }

        public double ActualPosition
        {
            get { return _actualPosition; }
            set { SetPosition(value); }
        }

        public double PercentageActualPosition { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        #region Constructors

        public GuiAnalyzerDataCursor(_PlcAgent.Analyzer.Analyzer analyzer)
            : base(analyzer)
        {
            InitializeComponent();

            PositionLabel.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Methods

        public void UpdateSizes(double height, double width)
        {
            Height = height - 25;
            SetPosition(_actualPosition);
        }

        private void SetPosition(double newPositionX)
        {
            if (_parentGrid == null) return;

            _rightLimitPosition = _parentGrid.Width - 100;

            if (newPositionX < LeftLimitPosition) newPositionX = LeftLimitPosition;
            if (newPositionX > _rightLimitPosition) newPositionX = _rightLimitPosition;

            Margin = new Thickness(newPositionX, 0, 0, 0);

            _actualPosition = newPositionX;
            PercentageActualPosition = (_actualPosition - LeftLimitPosition) / (_parentGrid.Width - LeftLimitPosition - 26.5);

            OnPropertyChanged();
        }

        #endregion

        #region Event Handlers

        protected override void OnRecordingChanged()
        {}

        protected override void OnRecordingTimeChanged()
        {}

        protected override void OnDataCursorsVisibilityChanged()
        {
            Visibility = Analyzer.AnalyzerSetupFile.ShowDataCursors[Analyzer.Header.Id] ? Visibility.Visible : Visibility.Hidden;
        }

        private void CursorGrid_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isSelected = true;
        }

        private void ParenGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            PositionLabel.Visibility = Visibility.Hidden;

            if (Mouse.RightButton != MouseButtonState.Pressed)
            {
                _isSelected = false;
                return;
            }
            if (!_isSelected) return;
            if (_parentGrid == null) return;

            var offset = CursorGrid.Width/2;
            var newPositionX = e.GetPosition(_parentGrid).X - offset;

            SetPosition(newPositionX);

            PositionLabel.Visibility = Visibility.Visible;
            PositionLabel.Content = TimeSpan.FromMilliseconds(Analyzer.TimeObservableVariable.GetTimePosition(PercentageActualPosition));
        }

        #endregion
    }
}
