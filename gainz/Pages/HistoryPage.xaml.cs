using gainz.ViewModels;

namespace gainz.Pages;

public partial class HistoryPage : ContentPage
{
	public HistoryPage()
	{
		InitializeComponent();
        BindingContext = new HistoryPageViewModel();  // Set the ViewModel as the BindingContext
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Cast the BindingContext to HistoryPageViewModel to access its methods
        if (BindingContext is HistoryPageViewModel viewModel)
        {
            viewModel.LoadCompletedWorkouts();  // Reload the data each time the page appears
        }
    }

    // Navigate to the SettingsPage when the settings icon is clicked
    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"settingspage");
    }
}