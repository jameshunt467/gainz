using gainz.Models;
using gainz.Services;
using gainz.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static gainz.App;

namespace gainz.Pages;

[QueryProperty(nameof(WorkoutId), "workoutId")]
public partial class WorkoutDetailsPage : ContentPage
{
    public int WorkoutId { get; set; } // Property to hold the workoutId
    public WorkoutDetailsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //// Ensure the ViewModel is loaded with the correct data
        //if (BindingContext is WorkoutDetailsViewModel viewModel)
        //{
        //    viewModel.LoadWorkoutDetails();
        //}

        // Initialize the ViewModel and set the WorkoutId
        var viewModel = new WorkoutDetailsViewModel
        {
            WorkoutId = this.WorkoutId
        };

        // Set the ViewModel as the BindingContext
        BindingContext = viewModel;

        // Load workout details using the WorkoutId
        viewModel.LoadWorkoutDetails();
    }
}