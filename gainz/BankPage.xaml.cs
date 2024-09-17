//using AndroidX.Lifecycle;

namespace gainz;

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

    private async void OnExerciseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            // Navigate to a detailed view (to be implemented)
            var selectedExercise = (Exercise)e.CurrentSelection.FirstOrDefault();
            // Navigate to the ExerciseDetailsPage and pass the selectedExercise
            await Navigation.PushAsync(new ExerciseDetailsPage(_viewModel.Exercises, selectedExercise));
            // Clear the selection after navigation
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private void OnAddExerciseClicked(object sender, EventArgs e)
    {
        // Logic to open a new page or modal to create a new exercise
    }
}
