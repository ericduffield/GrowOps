using GrowOps.Enums;
using GrowOps.Models;
using GrowOps.Services;
using System.Globalization;

namespace GrowOps.Views.Technician;

/// <summary>
/// GrowOps
/// Semester 6 April 28th, 2023
/// App Dev III
/// Technician actuators page back end code
/// </summary>
public partial class Actuators : ContentPage
{
    private readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

    public Actuators()
    {
        InitializeComponent();

        BindingContext = App.Repo.PlantSubsystem;

        switchDoorLock.BindingContext = App.Repo.SecuritySubsystem;
    }

    private async void switchFan_Toggled(object sender, ToggledEventArgs e)
    {
        var fanSwitch = sender as Switch;

        if (!fanSwitch.IsEnabled)
        {
            fanSwitch.IsEnabled = true;
            return;
        }
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "Internet Access is needed to control actuators", Enums.PopupResponses.Ok);
            fanSwitch.IsEnabled = false;
            fanSwitch.IsToggled = !fanSwitch.IsToggled;
            return;
        }
        try
        {
            bool changed = false;
            if (App.Repo.PlantSubsystem.TemperatureThreshold < App.Repo.PlantSubsystem.Temperature.Value)
            {
                if (fanSwitch.IsToggled)
                {
                    changed = true;
                }
                App.Repo.PlantSubsystem.FanState = true;
                
            }
            else
            {
                if (!fanSwitch.IsToggled)
                {
                    changed = true;
                }
                App.Repo.PlantSubsystem.FanState = false;
            }

            if(changed)
            {
                CommandPayload commandPayload = new();
                commandPayload.SetPayload($"{textInfo.ToTitleCase(fanSwitch.IsToggled.ToString())}");
                await DataService.InvokeMethodAsync(App.DataService.ServiceClient, commandPayload, Enums.Type.FAN);
            }
            

            return;
        }
        catch (Exception)
        {
            await DisplayAlert("Error!", "Error while sending command", PopupResponses.Ok);
            fanSwitch.IsEnabled = false;
            fanSwitch.IsToggled = !fanSwitch.IsToggled;
        }
    }

    private async void switchLights_Toggled(object sender, ToggledEventArgs e)
    {
        var lightSwitch = sender as Switch;

        if (!lightSwitch.IsEnabled)
        {
            lightSwitch.IsEnabled = true;
            return;
        }
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "Internet Access is needed to control actuators", Enums.PopupResponses.Ok);
            lightSwitch.IsEnabled = false;
            lightSwitch.IsToggled = !lightSwitch.IsToggled;
            return;
        }
        try
        {
            CommandPayload commandPayload = new();
            commandPayload.SetPayload($"{textInfo.ToTitleCase(lightSwitch.IsToggled.ToString())}");
            await DataService.InvokeMethodAsync(App.DataService.ServiceClient, commandPayload, Enums.Type.LIGHT);
            //Debug.WriteLine($"Light set to: {lightSwitch.IsToggled}");
            return;
        }
        catch (Exception)
        {
            await DisplayAlert("Error!", "Error while sending command", PopupResponses.Ok);
            lightSwitch.IsEnabled = false;
            lightSwitch.IsToggled = !lightSwitch.IsToggled;
        }
    }
    private async void SwitchDoorLock_Toggled(object sender, ToggledEventArgs e)
    {
        var lockSwitch = sender as Switch;

        if (!lockSwitch.IsEnabled)
        {
            lockSwitch.IsEnabled = true;
            return;
        }
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "Internet Access is needed to control actuators", Enums.PopupResponses.Ok);
            lockSwitch.IsEnabled = false;
            lockSwitch.IsToggled = !lockSwitch.IsToggled;
            return;
        }
        try
        {
            CommandPayload commandPayload = new();
            commandPayload.SetPayload($"{textInfo.ToTitleCase(lockSwitch.IsToggled.ToString())}");
            await DataService.InvokeMethodAsync(App.DataService.ServiceClient, commandPayload, Enums.Type.DOOR_LOCK);
            //Debug.WriteLine($"Door Lock set to: {lockSwitch.IsToggled}");
            return;
        }
        catch (Exception)
        {
            await DisplayAlert("Error!", "Error while sending command", PopupResponses.Ok);
            lockSwitch.IsEnabled = false;
            lockSwitch.IsToggled = !lockSwitch.IsToggled;
        }
    }

    private async void slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "Internet Access is needed to change settings", Enums.PopupResponses.Ok);
            return;
        }

        if (App.Repo.PlantSubsystem.TemperatureThreshold < App.Repo.PlantSubsystem.Temperature.Value)
        {
            App.Repo.PlantSubsystem.FanState = true;
        }
        else
        {
            App.Repo.PlantSubsystem.FanState = false;
        }
    }
}