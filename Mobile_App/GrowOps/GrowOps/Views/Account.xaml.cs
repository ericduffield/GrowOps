using GrowOps.Enums;
using GrowOps.Models;
using GrowOps.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace GrowOps.Views;

/// <summary>
/// GrowOps
/// Semester 6 April 28th, 2023
/// App Dev III
/// Account page back end code
/// </summary>
public partial class Account : ContentPage, INotifyPropertyChanged
{
    public User CurrentUser { get; set; }
    public IntWrapper TelemetryInterval { get; set; }
    public Account()
    {
        InitializeComponent();
        CurrentUser = App.Repo.UserDB.CurrentUser;
        TelemetryInterval = DataService.TelemetryInterval;
        BindingContext = this;
    }

    private async void Logout(object sender, EventArgs e)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "Internet Access is needed to Logout", PopupResponses.Ok);
            return;
        }
        try
        {
            App.Repo.UserDB.CurrentUser = null;
            await App.DataService.StopProcessing();
            await Shell.Current.GoToAsync($"//MainPage");
            await DisplayAlert("Logout", "Logged out Successfully.", "OK");
        }
        catch (Exception error)
        {
            await DisplayAlert("Alert", error.Message, "OK");
        }

    }

    private void ThemeToggled(object sender, ToggledEventArgs e)
    {
        if (App.Current.UserAppTheme == AppTheme.Dark)
        {
            App.Current.UserAppTheme = AppTheme.Light;
            theme.Text = "Light";
        }
        else
        {
            App.Current.UserAppTheme = AppTheme.Dark;
            theme.Text = "Dark";
        }

    }

    private async void BtnSetTelemetry_Clicked(object sender, EventArgs e)
    {
        try
        {
            string text = btnSetTelemetry.Text;
            btnSetTelemetry.Text = Constants.SendingRequest;
            await App.DataService.SetTelemetryInterval(int.Parse(entryTelemetryInterval.Text));
            entryTelemetryInterval.IsEnabled = false;
            entryTelemetryInterval.Text = string.Empty;
            entryTelemetryInterval.IsEnabled = true;
            btnSetTelemetry.Text = text;
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Couldn't Set the Telemetry Interval", PopupResponses.Ok);
        }
    }

    private async void SwitchChangeLoginPreferences_Toggled(object sender, ToggledEventArgs e)
    {
        bool isToggled = (sender as Switch).IsToggled;
        CurrentUser.Preferences.SaveLogin = isToggled;
        await App.Repo.UserDB.UpdateUserPreferences(CurrentUser);


        if (isToggled)
        {
            saveLogin.Text = "On";
        }
        else
        {
            saveLogin.Text = "Off";
        }
    }
}