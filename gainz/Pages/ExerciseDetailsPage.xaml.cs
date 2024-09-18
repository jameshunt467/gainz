using System.Collections.ObjectModel;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;

// Saving photo
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using gainz.ViewModels;
using gainz.Services;


namespace gainz.Pages;

public partial class ExerciseDetailsPage : ContentPage
{
    private ObservableCollection<Exercise> _exercises;  // This is so that we can delete the exercise from the BankPage list
    private Exercise _selectedExercise;

    public ExerciseDetailsPage(ObservableCollection<Exercise> exercises, Exercise selectedExercise)
    {
        InitializeComponent();
        _exercises = exercises;
        _selectedExercise = selectedExercise;
        // Create the ViewModel and pass the exercises collection and selected exercise
        var viewModel = new ExerciseDetailsViewModel(exercises, selectedExercise);
        BindingContext = viewModel;
    }
    private void OnNewCategoryTextChanged(object sender, TextChangedEventArgs e)
    {
        // If a new category is being typed, clear the Picker selection
        if (!string.IsNullOrWhiteSpace(NewCategoryEntry.Text))
        {
            CategoryPicker.SelectedIndex = -1;
        }
    }

    private void OnCategorySelected(object sender, EventArgs e)
    {
        // If a category is selected from the Picker, clear the new category entry
        if (CategoryPicker.SelectedIndex != -1)
        {
            NewCategoryEntry.Text = string.Empty;
        }
    }

    private async void OnImageTapped(object sender, EventArgs e)
    {
        // Show action sheet to select an image or take a new photo
        string action = await DisplayActionSheet("Select Image Source", "Cancel", null, "Take Photo", "Choose from Gallery");

        if (action == "Take Photo")
        {
            // Open the camera to take a new photo
            await TakePhotoAsync();
        }
        else if (action == "Choose from Gallery")
        {
            // Open the gallery to select an existing photo
            await PickPhotoAsync();
        }
    }

    private async Task TakePhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // Create a new file path to save the photo
                string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, $"{Path.GetRandomFileName()}.jpg");

                // Save the photo to the file
                using (var stream = await photo.OpenReadAsync())
                using (var newFileStream = File.OpenWrite(fileName))
                {
                    await stream.CopyToAsync(newFileStream);
                }

                // Update the ImageUrl property in the ViewModel with the new image path
                ((ExerciseDetailsViewModel)BindingContext).ImageUrl = fileName;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async Task PickPhotoAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image"
            });

            if (result != null)
            {
                // Update the ImageUrl property in the ViewModel with the selected image path
                ((ExerciseDetailsViewModel)BindingContext).ImageUrl = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}