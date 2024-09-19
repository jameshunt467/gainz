namespace gainz.Pages;

using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using gainz.Models;
using gainz.Services;
using System.Collections.ObjectModel;

public partial class AddExercisePage : ContentPage
{
    private string _selectedImagePath;
    public ObservableCollection<string> Categories { get; set; }

    public AddExercisePage()
	{
		InitializeComponent();
        LoadCategories(); // Load categories for the Picker
        BindingContext = this; // Set BindingContext for data binding
    }

    private void LoadCategories()
    {
        // Fetch categories from the database
        var categoriesFromDb = DatabaseService.GetAllCategories().Select(c => c.Name).ToList();
        Categories = new ObservableCollection<string>(categoriesFromDb);
        CategoryPicker.ItemsSource = Categories;
    }

    private void OnCategorySelected(object sender, EventArgs e)
    {
        // If a category is selected from the Picker, clear the new category entry
        if (CategoryPicker.SelectedIndex != -1)
        {
            NewCategoryEntry.Text = string.Empty;
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

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        // Show action sheet to choose between taking a photo or selecting from gallery
        string action = await DisplayActionSheet("Select Image Source", "Cancel", null, "Take Photo", "Choose from Gallery");

        if (action == "Take Photo")
        {
            await TakePhotoAsync();
        }
        else if (action == "Choose from Gallery")
        {
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

                // Update the ImageUrl property with the new image path
                _selectedImagePath = fileName;
                SelectedImage.Source = _selectedImagePath;
                SelectedImage.IsVisible = true;
            }
        }


        catch (FeatureNotSupportedException fnsEx)
        {
            await DisplayAlert("Error", "Camera not supported on this device.", "OK");
        }
        catch (PermissionException pEx)
        {
            await DisplayAlert("Error", "Camera permission is required.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
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
                _selectedImagePath = result.FullPath;
                SelectedImage.Source = _selectedImagePath;
                SelectedImage.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnSaveExerciseClicked(object sender, EventArgs e)
    {
        // Get values from input fields
        var name = NameEntry.Text;
        var description = DescriptionEditor.Text;
        //var category = CategoryEntry.Text;
        var selectedCategory = CategoryPicker.SelectedItem as string;
        var newCategory = NewCategoryEntry.Text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) || (string.IsNullOrWhiteSpace(selectedCategory) && string.IsNullOrWhiteSpace(newCategory)))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }

        int categoryIdToSave;

        if (!string.IsNullOrWhiteSpace(newCategory))
        {
            // Add new category to database if it doesn't exist
            if (!Categories.Contains(newCategory))
            {
                var newCategoryObj = new Category { Name = newCategory };
                DatabaseService.Connection.Insert(newCategoryObj);
                Categories.Add(newCategory);
                categoryIdToSave = newCategoryObj.Id; // Get the ID of the newly inserted category
            }
            else
            {
                // Retrieve the ID of the existing category
                categoryIdToSave = DatabaseService.GetCategoryByName(newCategory).Id;
            }
        }
        else
        {
            // Retrieve the ID of the selected category
            categoryIdToSave = DatabaseService.GetCategoryByName(selectedCategory).Id;
        }

        // Create a new Exercise object
        var newExercise = new Exercise
        {
            Name = name,
            Description = description,
            CategoryId = categoryIdToSave,
            ImageUrl = _selectedImagePath // Use the selected image path
        };

        // Save the new exercise to the SQLite database
        DatabaseService.Connection.Insert(newExercise);
        //DatabaseService.Connection.Commit();

        // Go back to the previous page
        await Navigation.PopAsync();
    }
}