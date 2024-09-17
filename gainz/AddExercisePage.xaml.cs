namespace gainz;

using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
//using gainz.Models;
using gainz.Services;

public partial class AddExercisePage : ContentPage
{
    private string _selectedImagePath;

    public AddExercisePage()
	{
		InitializeComponent();
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select an image"
        });

        if (result != null)
        {
            _selectedImagePath = result.FullPath;
            SelectedImage.Source = _selectedImagePath;
            SelectedImage.IsVisible = true;
        }
    }

    private async void OnSaveExerciseClicked(object sender, EventArgs e)
    {
        // Get values from input fields
        var name = NameEntry.Text;
        var description = DescriptionEditor.Text;
        var category = CategoryEntry.Text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(category))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }

        // Create a new Exercise object
        var newExercise = new Exercise
        {
            Name = name,
            Description = description,
            Category = category,
            ImageUrl = _selectedImagePath // Use the selected image path
        };

        // Save the new exercise to the SQLite database
        DatabaseService.Connection.Insert(newExercise);
        //DatabaseService.Connection.Commit();

        // Go back to the previous page
        await Navigation.PopAsync();

        // Notify the BankPage to refresh (optional, depending on the approach)
        //MessagingCenter.Send(this, "ExerciseAdded", newExercise);
    }
}