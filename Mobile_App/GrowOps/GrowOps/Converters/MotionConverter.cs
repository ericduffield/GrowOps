using System.Globalization;

namespace GrowOps.Converters
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// Motion Converter
    /// </summary>
    internal class MotionConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if ((double)value > 0)
        {
          return "Motion Detected";
        }
        return "No Motion";
      }
      catch (Exception)
      {
        return "Unknown";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
