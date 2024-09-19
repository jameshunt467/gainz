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
        await Navigation.PushAsync(new CreateWorkoutPage());
    }

    private bool _isNavigating = false;

    private async void OnWorkoutSelected(object sender, SelectionChangedEventArgs e)
    {
        if (_isNavigating)
            return;
        try
        {
            _isNavigating = true;
            // Get the selected workout
            var selectedWorkout = (Workout)e.CurrentSelection.FirstOrDefault();

            if (selectedWorkout != null)
            {
                Debug.WriteLine($"[{Constants.LogTag}] Selected Workout: {selectedWorkout?.Name}, Description: {selectedWorkout?.Description}");
                // Navigate to the WorkoutDetailsPage and pass the selected workout ID
                await Navigation.PushAsync(new WorkoutDetailsPage(selectedWorkout.Id));

                // Clear the selection
                ((CollectionView)sender).SelectedItem = null;
            } else
            {
                Debug.WriteLine($"[{Constants.LogTag}] Could not find workout");
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.WriteLine($"[{Constants.LogTag}] NullReferenceException: {ex.Message}");
            // Optionally, display an alert to the user
            await Application.Current.MainPage.DisplayAlert("Error", "An error occurred while selecting the workout. Please try again.", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{Constants.LogTag}] Exception: {ex.Message}");
            // Optionally, handle other types of exceptions
            await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred. Please try again.", "OK");
        }
        finally
        {
            _isNavigating = false; // Reset the flag
        }
    }

}