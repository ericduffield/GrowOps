using GrowOps.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace GrowOps.Views.Owner;

/// <summary>
/// GrowOps
/// Semester 6 April 28th, 2023
/// App Dev III
/// Map page back end code
/// </summary>
public partial class MapPage : ContentPage
{
    MapViewModel vm;
    public MapPage(MapViewModel vm)
    {
        InitializeComponent();

        this.vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshLocation(); //updates location
    }

    protected override void OnDisappearing()
    {
        vm.DisposeCancellationTokenCommand.Execute(null);

        base.OnDisappearing();
    }

    private void GoToCurrentLocation(object sender, EventArgs e)
    {
        RefreshLocation(); //updates location
    }

    /// <summary>
    /// This function calls GetCurrentLocationCommand which creates a pin and centers map on pin
    /// </summary>
    private void RefreshLocation()
    {
        vm.GetCurrentLocationCommand.Execute(new Button());
    }
}