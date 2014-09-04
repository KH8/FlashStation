using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace _PlcAgent.Visual.TreeListView
{
    /// <summary>
    /// Convert Level to left margin
    /// Pass a prarameter if you want a unit length other than 19.0.
    /// </summary>
    public class LevelToIndentConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, 
                              CultureInfo culture)
        {
            return new Thickness((int)o * CIndentSize, 0, 0, 0);
        }

        public object ConvertBack(object o, Type type, object parameter, 
                                  CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private const double CIndentSize = 19.0;
    }
}
