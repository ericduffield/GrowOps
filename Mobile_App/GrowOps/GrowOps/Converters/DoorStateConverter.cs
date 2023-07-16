using System.Globalization;

namespace GrowOps.Converters
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// Door Stage Converter
    /// </summary>
    internal class DoorStateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if ((double)value > 0)
        {
          return "Open";
        }
        return "Closed";
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
