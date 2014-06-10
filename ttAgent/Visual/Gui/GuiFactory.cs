using System;
using System.Windows;
using System.Windows.Controls;

namespace ttAgent.Visual.Gui
{
    static class GuiFactory
    {
        #region Grid

        public static Grid CreateGrid()
        {
            return new Grid();
        }

        public static Grid CreateGrid(int xPosition, int yPosition, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment)
        {
            return new Grid
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        public static Grid CreateGrid(int xPosition, int yPosition, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, int height)
        {
            return new Grid
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        public static Grid CreateGrid(int xPosition, int yPosition, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, int height, int width)
        {
            return new Grid
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        #endregion

        #region GroupBox

        public static GroupBox CreateGroupBox(string header, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, int height, int width)
        {
            return new GroupBox
            {
                Header = header,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        #endregion

        #region Label

        public static Label CreateLabel(string content, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, int height, int width)
        {
            return new Label
            {
                Content = content,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        public static Label CreateLabel(string name, string content, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            HorizontalAlignment horizontalContentAlignment,
            int height, int width)
        {
            return new Label
            {
                Name = name,
                Content = content,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                HorizontalContentAlignment = horizontalContentAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        public static void UpdateLabel(Label label, string content)
        {
            label.Dispatcher.BeginInvoke((new Action(delegate { label.Content = content; })));
        }

        #endregion

        #region Button

        public static Button CreateButton(string name, string content, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            int height, int width, RoutedEventHandler click)
        {
            var button = new Button
            {
                Name = name,
                Content = content,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0),
                ClickMode = ClickMode.Release
            };

            button.Click += click;
            return button;
        }

        #endregion

        #region CheckBox

        public static CheckBox CreateCheckBox(string name, string content, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            int width, RoutedEventHandler checkedUnchecked)
        {
            var checkBox = new CheckBox
            {
                Name = name,
                Content = content,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0),
                FlowDirection = FlowDirection.RightToLeft
            };

            checkBox.Checked += checkedUnchecked;
            checkBox.Unchecked += checkedUnchecked;

            return checkBox;
        }

        #endregion

        #region TextBox

        public static TextBox CreateTextBox(string name, string text, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            HorizontalAlignment horizontalContentAlignment, int height, int width)
        {
            return new TextBox
            {
                Name = name,
                Text = text,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                HorizontalContentAlignment = horizontalContentAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0),
                TextWrapping = TextWrapping.Wrap,
            };
        }

        public static TextBox CreateTextBox(string name, string text, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            HorizontalAlignment horizontalContentAlignment,
            int height, int width, TextChangedEventHandler textChanged)
        {
            var textBox = new TextBox
            {
                Name = name,
                Text = text,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                HorizontalContentAlignment = horizontalContentAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0),
                TextWrapping = TextWrapping.Wrap,
            };

            textBox.TextChanged += textChanged;

            return textBox;
        }

        #endregion

        #region ProgressBar

        public static ProgressBar CreateProgressBar(string name, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            int height, int width)
        {
            return new ProgressBar
            {
                Name = name,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0),
                UseLayoutRounding = false,
                Foreground = null
            };
        }

        #endregion

        #region ComboBox

        public static ComboBox CreateComboBox(string name, string text, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            int height, int width)
        {
            return new ComboBox
            {
                Name = name,
                Text = text,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        #endregion

        #region ListView

        public static ListView CreateListView(string name, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            double height, double width, SelectionChangedEventHandler selectionChanged)
        {
            var listView = new ListView
            {
                Name = name,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };

            listView.SelectionChanged += selectionChanged;

            return listView;
        }

        public static ListView CreateListView(string name, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            double height, double width)
        {
            return new ListView
            {
                Name = name,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        #endregion
    }
}
