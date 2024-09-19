using gainz.Models;
using gainz.ViewModels;
using System.Diagnostics;

//using HealthKit;
using static gainz.App;

namespace gainz.Pages;

public partial class WorkoutsPage : ContentPage
{
    private WorkoutsViewModel _viewModel;
    public WorkoutsPage()
	{
		InitializeComponent();

        _viewModel = new WorkoutsViewModel();
        BindingContext = _viewModel;
    }

    private async void OnAddWorkoutClicked(object sender, EventArgs e)
    {
        // Navigate to the CreateWorkoutPage to create a new workout
        await Shell.Current.GoToAsync($"createworkout");
    }
}