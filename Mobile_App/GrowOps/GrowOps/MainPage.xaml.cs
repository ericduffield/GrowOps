using Firebase.Auth;
using GrowOps.Enums;
using GrowOps.Exceptions;
using GrowOps.Models;
using GrowOps.Services;
using GrowOps.Views;
using Newtonsoft.Json;

namespace GrowOps;
public partial class MainPage : ContentPage
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// Main Page
    /// </summary>
    private string authenticatedUserRole = "";
    private event EventHandler Starting = delegate { };
    private event EventHandler Resuming = delegate { };
    public MainPage()
    {
        InitializeComponent();

        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

    }

    private async void BtnSecurity_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SecurityActuators");
        await Authenticate("duff@growops.ca", "growops");
    }

    private async void BtnGeo_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Sensors");
        await Authenticate("duff@growops.ca", "growops");
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "Internet Access is needed to Login", PopupResponses.Ok);
            return;
        }
        try
        {
            if (await Authenticate(entryEmail.Text, entryPassword.Text))
            {
                if (authenticatedUserRole == Role.Owner)
                {
                    await Shell.Current.GoToAsync("//Sensors");
                }
                else
                {
                    await Shell.Current.GoToAsync("//SecurityActuators");
                }
            }
        }
        catch (GrowOpsAuthenticationException ex)
        {
            await DisplayAlert("Error", ex.Message, PopupResponses.Ok);
        }
        catch (FirebaseAuthException)
        {
            await DisplayAlert("Error", "Authentication Error", PopupResponses.Ok);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error while Authenticating User", PopupResponses.Ok);
        }

    }

    private async Task<bool> Authenticate(string email, string password)
    {
        var user = await App.Repo.UserDB.Authenticate(email, password);
        if (user == null)
        {
            await DisplayAlert("Not Valid", "Email or password invalid", PopupResponses.Ok);
            return false;
        }
        if (!user.Preferences.SaveLogin)
        {
            entryEmail.Text = string.Empty;
            entryPassword.Text = string.Empty;
        }
        if (App.Repo.HasLoggedIn)
        {
            Resuming += OnResuming;
            Resuming(this, EventArgs.Empty);
            authenticatedUserRole = user.Role;
            return true;
        }
        App.Repo.HasLoggedIn = true;
        //subscribe to event
        Starting += OnStarting;
        //raise event
        Starting(this, EventArgs.Empty);
        authenticatedUserRole = user.Role;
        return true;
    }

    private async void OnResuming(object sender, EventArgs e)
    {
        Resuming -= OnResuming;
        await App.DataService.ContinueProcessing();
    }

    private async void OnStarting(object sender, EventArgs e)
    {
        Starting -= OnStarting;
        await App.DataService.StartProcessing();
    }

    private async void BtnSignUp_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUp());
    }
}