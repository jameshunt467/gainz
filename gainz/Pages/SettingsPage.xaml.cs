using gainz.JoinTable;
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
        await Shell.Current.GoToAsync($"managecategories");
    }

    private async void OnClearDatabaseClicked(object sender, EventArgs e)
    {
        bool confirmClear = await DisplayAlert("Confirm", "Are you sure you want to clear the entire database?", "Yes", "No");

        if (confirmClear)
        {
            var db = DatabaseService.Connection;
            db.DeleteAll<Exercise>(); // Clear all exercises from the database
            db.DeleteAll<Category>(); // Clear all categories from the database
            db.DeleteAll<Workout>();  // Clear all workouts from the database
            db.DeleteAll<ExerciseWorkout>(); // Clear all joins

            await DisplayAlert("Database Cleared", "All data has been removed from the database.", "OK");
        }
    }
    private async void OnAddSampleDataClicked(object sender, EventArgs e)
    {
        // Add sample exercises to the database
        var db = DatabaseService.Connection;

        // Add sample categories to the database
        var chestCategory = new Category { Name = "Chest" };
        var legsCategory = new Category { Name = "Legs" };

        db.Insert(chestCategory);
        db.Insert(legsCategory);

        // Retrieve the IDs of the inserted categories
        int chestCategoryId = chestCategory.Id;
        int legsCategoryId = legsCategory.Id;

        // Add sample exercises to the database using the retrieved category IDs
        db.Insert(new Exercise { Name = "Push Up", Description = "A basic push-up exercise", CategoryId = chestCategoryId, ImageUrl = "pushup.png" });
        db.Insert(new Exercise { Name = "Bench Press", Description = "A bench press exercise", CategoryId = chestCategoryId, ImageUrl = "benchpress.png" });
        db.Insert(new Exercise { Name = "Squat", Description = "A basic squat exercise", CategoryId = legsCategoryId, ImageUrl = "squat.png" });
        db.Insert(new Exercise { Name = "Leg Press", Description = "A leg press exercise", CategoryId = legsCategoryId, ImageUrl = "legpress.png" });

        // Add sample workouts to the database
        var chestWorkout = new Workout { Name = "Chest Workout", Description = "A workout focusing on chest exercises" };
        var legsWorkout = new Workout { Name = "Legs Workout", Description = "A workout focusing on leg exercises" };

        db.Insert(chestWorkout);
        db.Insert(legsWorkout);

        // Retrieve the IDs of the inserted workouts
        int chestWorkoutId = chestWorkout.Id;
        int legsWorkoutId = legsWorkout.Id;

        // Add exercises to the workouts
        db.Insert(new ExerciseWorkout { WorkoutId = chestWorkoutId, ExerciseId = db.Table<Exercise>().First(e => e.Name == "Push Up").Id });
        db.Insert(new ExerciseWorkout { WorkoutId = chestWorkoutId, ExerciseId = db.Table<Exercise>().First(e => e.Name == "Bench Press").Id });
        db.Insert(new ExerciseWorkout { WorkoutId = legsWorkoutId, ExerciseId = db.Table<Exercise>().First(e => e.Name == "Squat").Id });
        db.Insert(new ExerciseWorkout { WorkoutId = legsWorkoutId, ExerciseId = db.Table<Exercise>().First(e => e.Name == "Leg Press").Id });

        await DisplayAlert("Sample Data Added", "Sample exercises and workouts have been added to the database.", "OK");
    }

}