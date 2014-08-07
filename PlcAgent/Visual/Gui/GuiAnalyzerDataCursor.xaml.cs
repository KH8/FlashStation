using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _PlcAgent.Visual.Gui
{
    /// <summary>
    /// Interaction logic for GuiAnalyzerDataCursor.xaml
    /// </summary>
    public partial class GuiAnalyzerDataCursor
    {
        private readonly Analyzer.Analyzer _analyzer;

        private Boolean _isSelected;

        private readonly double _leftLimitPosition;
        private double _rightLimitPosition;

        private Grid _parentGrid;
        private double _actualPosition;

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

        public GuiAnalyzerDataCursor(Analyzer.Analyzer analyzer)
        {
            _analyzer = analyzer;

            _leftLimitPosition = 206.0;
            _rightLimitPosition = 1000.0;

            _actualPosition = 0.0;

            InitializeComponent();

            PositionLabel.Visibility = Visibility.Hidden;
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height - 25;
            SetPosition(_actualPosition);
        }

        private void SetPosition(double newPositionX)
        {
            if (_parentGrid == null) return;

            _rightLimitPosition = _parentGrid.Width - 100;

            if (newPositionX < _leftLimitPosition) newPositionX = _leftLimitPosition;
            if (newPositionX > _rightLimitPosition) newPositionX = _rightLimitPosition;

            Margin = new Thickness(newPositionX, 0, 0, 0);

            _actualPosition = newPositionX;
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

            var offset = CursorGrid.Width / 2;
            var newPositionX = e.GetPosition(_parentGrid).X - offset;

            SetPosition(newPositionX);

            var positionPercentage = (_actualPosition - _leftLimitPosition) / (_parentGrid.Width - _leftLimitPosition - 26.5);
            PositionLabel.Visibility = Visibility.Visible;
            PositionLabel.Content = TimeSpan.FromMilliseconds(_analyzer.GetTimePosition(positionPercentage));
        }
    }
}
