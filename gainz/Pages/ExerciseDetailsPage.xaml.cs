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
using System.Diagnostics;
using Microsoft.VisualBasic;
using Constants = gainz.App.Constants;
using FileSystem = Microsoft.Maui.Storage.FileSystem;
//using AndroidX.Lifecycle;


namespace gainz.Pages;

[QueryProperty(nameof(ExerciseId), "exerciseId")]
public partial class ExerciseDetailsPage : ContentPage
{
    //private ObservableCollection<Exercise> _exercises;  // This is so that we can delete the exercise from the BankPage list
    private Exercise _selectedExercise;
    public  int ExerciseId { get; set; }
    private ExerciseDetailsViewModel viewModel;

    public ExerciseDetailsPage()
    {
        InitializeComponent();
        // This is happening in OnAppearing
        //viewModel = new ExerciseDetailsViewModel(ExerciseId);
        //BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (ExerciseId != 0)
        {
            // Load the exercise details here
            _selectedExercise = DatabaseService.GetExerciseWithId(ExerciseId);
            var viewModel = new ExerciseDetailsViewModel(ExerciseId);
            BindingContext = viewModel;
        }
        else
        {
            Debug.WriteLine("OnAppearing: Invalid ExerciseId received.");
        }
    }

    private void OnIncreaseWidthClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as ExerciseDetailsViewModel;
        viewModel?.IncreaseWidth();
    }

    private void OnDecreaseWidthClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as ExerciseDetailsViewModel;
        viewModel?.DecreaseWidth();
    }

    private void OnVolumeCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value) // If "Total Volume" is selected
        {
            var viewModel = BindingContext as ExerciseDetailsViewModel;
            viewModel?.UpdateChart(isVolume: true); // Update chart to display total volume
        }
    }

    private void OnWeightCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value) // If "Heaviest Weight" is selected
        {
            var viewModel = BindingContext as ExerciseDetailsViewModel;
            viewModel?.UpdateChart(isVolume: false); // Update chart to display highest weight
        }
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