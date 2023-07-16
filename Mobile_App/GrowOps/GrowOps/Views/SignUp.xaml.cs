using GrowOps.Models;

namespace GrowOps.Views;

public partial class SignUp : ContentPage
{
    private string checkedRole = "";
    public SignUp()
    {
        InitializeComponent();
        radioOwner.Value = Enums.Role.Owner;
        radioTechnician.Value = Enums.Role.Technician;
    }

    private async void BtnSignup_Clicked(object sender, EventArgs e)
    {
        if (IsFormValid())
        {
            bool isSuccessApp = await App.Repo.UserDB.CreateUser(email.Text, password.Text, checkedRole, rbSaveLoginInfo.IsChecked);
            if (isSuccessApp)
            {
                await Navigation.PopAsync();
                return;
            }

            await DisplayAlert("Error", "Couldn't Create User, Try again or contact support", Enums.PopupResponses.Ok);

        }

        await DisplayAlert("Missing Information", "All fields must be filled in", Enums.PopupResponses.Ok);
    }

    private void Radio_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton checkedBtn = sender as RadioButton;
        if (checkedBtn == null) return;

        checkedRole = checkedBtn.Value.ToString();
    }

    private bool IsFormValid()
    {
        return checkedRole != "" && email.Text.Length != 0 && password.Text.Length != 0;
    }
}