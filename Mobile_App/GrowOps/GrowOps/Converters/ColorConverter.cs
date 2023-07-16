using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowOps.Converters
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// Thic class is a converter from a double value to color
    /// </summary>
    internal class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if ((double)value < App.Repo.StateThresholds[0])
                    return Colors.Green;
                else if ((double)value < App.Repo.StateThresholds[1])
                    return Colors.Orange;
                else
                    return Colors.Red;
            }
            catch (Exception)
            {
                return Colors.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
