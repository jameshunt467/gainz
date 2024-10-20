//using AndroidX.Lifecycle;

using gainz.ViewModels;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace gainz.Pages;

public partial class BankPage : ContentPage
{
    private BankViewModel _viewModel;

    public BankPage()
    {
        InitializeComponent();

        _viewModel = new BankViewModel();
        // Set the BindingContext so that the XAML can bind to the ViewModel
        BindingContext = _viewModel;
    }

    // Force refresh the page so images load
    protected override void OnAppearing()
    {
        base.OnAppearing();

        //// Force the CollectionView to refresh
        //ExerciseList.ItemsSource = null;
        //ExerciseList.ItemsSource = _viewModel.Exercises;

        // Refresh exercises list from the database when the page appears
        //_viewModel = new BankViewModel();
        // Refresh exercises list from the database when the page appears
        _viewModel.LoadExercisesFromDatabase();
        BindingContext = _viewModel;
    }

    private async void OnExerciseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            // Navigate to a detailed view
            var selectedExercise = (Exercise)e.CurrentSelection.FirstOrDefault();
            // Navigate to the ExerciseDetailsPage and pass the selectedExercise
            try
            {
                Debug.WriteLine($"[{gainz.App.Constants.LogTag}] Navigating to exercise with Id {selectedExercise.Id}");
                await Shell.Current.GoToAsync("exercisedetails", new Dictionary<string, object>
                {
                    { "exerciseId", selectedExercise.Id }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Cannot navigate. " + ex.InnerException.Message, "OK");
            }
            // Clear the selection after navigation
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnAddExerciseClicked(object sender, EventArgs e)
    {
        // Logic to open a new page or modal to create a new exercise
        //await Navigation.PushAsync(new AddExercisePage());
        await Shell.Current.GoToAsync($"addexercise");
    }
}
