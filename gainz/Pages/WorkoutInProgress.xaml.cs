using gainz.ViewModels;

namespace gainz.Pages;

[QueryProperty(nameof(WorkoutId), "workoutId")]
public partial class WorkoutInProgress : ContentPage
{
    public int WorkoutId { get; set; }
    public WorkoutInProgress()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Set the BindingContext to WorkoutInProgressViewModel with workoutId
        BindingContext = new WorkoutInProgressViewModel(WorkoutId);
    }
}