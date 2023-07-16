using GrowOps.Enums;
using GrowOps.Models;
using GrowOps.Services;
using System.Globalization;

namespace GrowOps.Views.Owner;

/// <summary>
/// GrowOps
/// Semester 6 April 28th, 2023
/// App Dev III
/// Owner actuators page back end code
/// </summary>
public partial class Actuators : ContentPage
{
    private readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
    private SecuritySubsystem SecuritySubsystem { get; set; }
    private bool hasToggled = false;
    public Actuators()
    {
        InitializeComponent();
        SecuritySubsystem = App.Repo.SecuritySubsystem;
        BindingContext = SecuritySubsystem;
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

    private async void SwitchBuzzer_Toggled(object sender, ToggledEventArgs e)
    {
        var buzzerSwitch = sender as Switch;

        if (!buzzerSwitch.IsEnabled)
        {
            buzzerSwitch.IsEnabled = true;
            return;
        }
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "Internet Access is needed to control actuators", Enums.PopupResponses.Ok);
            buzzerSwitch.IsEnabled = false;
            buzzerSwitch.IsToggled = !buzzerSwitch.IsToggled;
            return;
        }
        try
        {
            CommandPayload commandPayload = new();
            commandPayload.SetPayload($"{textInfo.ToTitleCase(buzzerSwitch.IsToggled.ToString())}");
            await DataService.InvokeMethodAsync(App.DataService.ServiceClient, commandPayload, Enums.Type.BUZZER);
            //Debug.WriteLine($"Buzzer set to: {buzzerSwitch.IsToggled}");
        }
        catch (Exception)
        {
            await DisplayAlert("Error!", "Error while sending command", PopupResponses.Ok);
            buzzerSwitch.IsEnabled = false;
            buzzerSwitch.IsToggled = !buzzerSwitch.IsToggled;
        }
    }

}