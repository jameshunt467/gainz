using gainz.ViewModels;

namespace gainz.Pages;

[QueryProperty(nameof(WorkoutId), "workoutId")]
public partial class ExerciseSelectionPage : ContentPage
{
    public int WorkoutId { get; set; }
    public ExerciseSelectionPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new ExerciseSelectionViewModel(WorkoutId);
    }
}