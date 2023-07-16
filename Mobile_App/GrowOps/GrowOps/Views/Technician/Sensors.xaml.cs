using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GrowOps.Views.Technician;

/// <summary>
/// GrowOps
/// Semester 6 April 28th, 2023
/// App Dev III
/// Technician sensors page back end code
/// </summary>
public partial class Sensors : ContentPage
{
    public Sensors()
    {
        InitializeComponent();
        BindingContext = App.Repo.PlantSubsystem;
    }
}