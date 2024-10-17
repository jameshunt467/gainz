using gainz.JoinTable;
using gainz.Models;
using gainz.Services;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using static gainz.App;

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
            db.DeleteAll<CompletedWorkout>(); // Clear all completed workouts
            db.DeleteAll<CompletedSet>(); // Clear all completed sets

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

        // Retrieve the IDs of the Exercises
        int pushUpId = db.Table<Exercise>().First(e => e.Name == "Push Up").Id;
        int benchPressId = db.Table<Exercise>().First(e => e.Name == "Bench Press").Id;
        int squatId = db.Table<Exercise>().First(e => e.Name == "Squat").Id;
        int legPressId = db.Table<Exercise>().First(e => e.Name == "Leg Press").Id;

        // Add exercises to the workouts
        db.Insert(new ExerciseWorkout { WorkoutId = chestWorkoutId, ExerciseId = pushUpId });
        db.Insert(new ExerciseWorkout { WorkoutId = chestWorkoutId, ExerciseId = benchPressId });
        db.Insert(new ExerciseWorkout { WorkoutId = legsWorkoutId, ExerciseId = squatId });
        db.Insert(new ExerciseWorkout { WorkoutId = legsWorkoutId, ExerciseId = legPressId });

        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId);

        await DisplayAlert("Sample Data Added", "Sample exercises and workouts have been added to the database.", "OK");
    }

    private void AddSampleCompletedWorkout(int pushUpId, int benchPressId, int squatId, int legPressId)
    {
        // Create a sample completed workout
        var completedWorkout = new CompletedWorkout
        {
            WorkoutName = "Sample Workout",
            WorkoutDate = DateTime.Now,
            TotalVolume = 4000,
            TotalSets = 8,
            Sets = new List<CompletedSet>()
        };

        // Save the workout to the database
        DatabaseService.SaveCompletedWorkout(completedWorkout);

        // Add sets for each exercise in the workout
        var pushUpSet = new CompletedSet { Weight = 0, Reps = 20, CompletedWorkoutId = completedWorkout.Id, ExerciseId = pushUpId };
        var benchPressSet = new CompletedSet { Weight = 100, Reps = 10, CompletedWorkoutId = completedWorkout.Id, ExerciseId = benchPressId };
        var squatSet = new CompletedSet { Weight = 150, Reps = 12, CompletedWorkoutId = completedWorkout.Id, ExerciseId = squatId };
        var legPressSet = new CompletedSet { Weight = 200, Reps = 8, CompletedWorkoutId = completedWorkout.Id, ExerciseId = legPressId };

        // Save the sets to the database
        DatabaseService.SaveCompletedSet(pushUpSet);
        DatabaseService.SaveCompletedSet(benchPressSet);
        DatabaseService.SaveCompletedSet(squatSet);
        DatabaseService.SaveCompletedSet(legPressSet);
    }
}