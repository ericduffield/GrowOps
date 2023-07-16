using GrowOps.Models;

namespace GrowOps.Views.Owner;

/// <summary>
/// GrowOps
/// Semester 6 April 28th, 2023
/// App Dev III
/// Owner sensors page back end code
/// </summary>
public partial class Sensors : ContentPage
{
    public GeoLocationSubsystem GeoLocationSubsystem { get; set; }
    public SecuritySubsystem SecuritySubsystem { get; set; }
  public Sensors()
  {
    InitializeComponent();
    GeoLocationSubsystem = App.Repo.GeoLocationSubsystem;
    SecuritySubsystem = App.Repo.SecuritySubsystem;
    BindingContext = this;
  }

}