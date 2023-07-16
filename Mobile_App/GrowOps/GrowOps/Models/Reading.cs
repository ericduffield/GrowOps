using GrowOps.Interfaces;
using System.ComponentModel;

namespace GrowOps.Models
{
  /// <summary>
  /// GrowOps
  /// Semester 6 April 28th, 2023
  /// App Dev III
  /// This class holds information about a reading
  /// </summary>
  public class Reading : IHasUKey, INotifyPropertyChanged
  {
    public double Value { get; set; }
    public string Unit { get; set; }
    public string Type { get; set; }
    public string Key { get; set; }

    public Reading(double value, string unit, string type)
    {
      Value = value;
      Unit = unit;
      Type = type;
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
