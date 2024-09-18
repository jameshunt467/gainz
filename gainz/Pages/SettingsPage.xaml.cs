using gainz.Models;
using gainz.Services;
using Microsoft.Maui.Controls;

namespace gainz.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
    }

    private async void OnManageCategoriesClicked(object sender, EventArgs e)
    {
        // Navigate to a page where users can manage categories 
        await Navigation.PushAsync(new ManageCategoriesPage());
    }

    private async void OnClearDatabaseClicked(object sender, EventArgs e)
    {
        bool confirmClear = await DisplayAlert("Confirm", "Are you sure you want to clear the entire database?", "Yes", "No");

        if (confirmClear)
        {
            var db = DatabaseService.Connection;
            db.DeleteAll<Exercise>(); // Clear all exercises from the database
            db.DeleteAll<Category>(); // Clear all categories from the database

            await DisplayAlert("Database Cleared", "All data has been removed from the database.", "OK");
        }
    }
    private async void OnAddSampleDataClicked(object sender, EventArgs e)
    {
        // Add sample exercises to the database
        var db = DatabaseService.Connection;

        // Add sample categories to the database
        db.Insert(new Category { Name = "Chest" });
        db.Insert(new Category { Name = "Legs" });

        db.Insert(new Exercise { Name = "Push Up", Description = "A basic push-up exercise", Category = "Chest", ImageUrl = "benchpress.png" });
        db.Insert(new Exercise { Name = "Squat", Description = "A basic squat exercise", Category = "Legs", ImageUrl = "squat.png" });

        await DisplayAlert("Sample Data Added", "Sample exercises have been added to the database.", "OK");
    }
}