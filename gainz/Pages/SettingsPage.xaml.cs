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

        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 1);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 2);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 3);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 4);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 5);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 6);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 7);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 8);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 9);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 10);

        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 11);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 12);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 13);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 14);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 15);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 16);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 17);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 18);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 19);
        AddSampleCompletedWorkout(pushUpId, benchPressId, squatId, legPressId, 20);

        await DisplayAlert("Sample Data Added", "Sample exercises and workouts have been saved.", "OK");
    }

    private void AddSampleCompletedWorkout(int pushUpId, int benchPressId, int squatId, int legPressId, int workoutCount)
    {
        // Create a sample completed workout
        var completedWorkout = new CompletedWorkout
        {
            WorkoutName = "Sample Workout " + workoutCount,
            WorkoutDate = DateTime.Now,
            TotalVolume = 0, // Initialize as 0
            TotalSets = 0,   // Initialize as 0
            Sets = new List<CompletedSet>() // Initialize empty list
        };

        // Save the workout to the database first
        DatabaseService.SaveCompletedWorkout(completedWorkout);

        Random random = new Random();
        // Add sets for each exercise in the workout
        var pushUpSets = new List<CompletedSet>
        {
            new CompletedSet { Weight = random.Next(20, 101), Reps = 10, CompletedWorkoutId = completedWorkout.Id, ExerciseId = pushUpId },
            new CompletedSet { Weight = random.Next(20, 101), Reps = 10, CompletedWorkoutId = completedWorkout.Id, ExerciseId = pushUpId },
            new CompletedSet { Weight = random.Next(20, 101), Reps = 8, CompletedWorkoutId = completedWorkout.Id, ExerciseId = pushUpId },
            new CompletedSet { Weight = random.Next(20, 101), Reps = 8, CompletedWorkoutId = completedWorkout.Id, ExerciseId = pushUpId }
        };

        var benchPressSets = new List<CompletedSet>
        {
            new CompletedSet { Weight = random.Next(20, 101), Reps = 10, CompletedWorkoutId = completedWorkout.Id, ExerciseId = benchPressId },
            new CompletedSet { Weight = random.Next(20, 101), Reps = 10, CompletedWorkoutId = completedWorkout.Id, ExerciseId = benchPressId },
            new CompletedSet { Weight = random.Next(20, 101), Reps = 10, CompletedWorkoutId = completedWorkout.Id, ExerciseId = benchPressId },
            new CompletedSet { Weight = random.Next(20, 101), Reps = 10, CompletedWorkoutId = completedWorkout.Id, ExerciseId = benchPressId }
        };

        var otherSets = new List<CompletedSet>
        {
            new CompletedSet { Weight = random.Next(20, 81), Reps = 12, CompletedWorkoutId = completedWorkout.Id, ExerciseId = squatId },
            new CompletedSet { Weight = random.Next(20, 81), Reps = 8, CompletedWorkoutId = completedWorkout.Id, ExerciseId = legPressId }
        };

        // Combine all sets
        var allSets = pushUpSets.Concat(benchPressSets).Concat(otherSets).ToList();

        // Save all sets to the database
        foreach (var set in allSets)
        {
            DatabaseService.SaveCompletedSet(set);
        }

        // Re-fetch the sets from the database to ensure they are correctly linked
        completedWorkout.Sets = DatabaseService.GetSetsByWorkoutId(completedWorkout.Id);

        // Calculate Total Volume and Total Sets
        completedWorkout.TotalVolume = completedWorkout.Sets.Sum(set => set.Weight * set.Reps);
        Debug.WriteLine($"[{Constants.LogTag}] Created Sample Workout {workoutCount} with Total Volu {completedWorkout.TotalVolume}");
        completedWorkout.TotalSets = completedWorkout.Sets.Count;

        // Update the workout in the database with the correct totals
        DatabaseService.UpdateCompletedWorkout(completedWorkout);
    }

}