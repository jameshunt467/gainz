using gainz.ViewModels;

namespace gainz.Pages;

public partial class CreateWorkoutPage : ContentPage
{
    private CreateWorkoutViewModel _viewModel;
    public CreateWorkoutPage()
	{
		InitializeComponent();
        _viewModel = new CreateWorkoutViewModel();
        BindingContext = _viewModel;
    }

    private async void OnCreateWorkoutClicked(object sender, EventArgs e)
    {
        // Call the CreateWorkout method from the ViewModel
        await _viewModel.CreateWorkout();
    }
}