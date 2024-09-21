using Android.Webkit;
using gainz.Models;
using gainz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gainz.ViewModels
{
    public class BankViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Exercise> Exercises { get; set; }

        public BankViewModel()
        {
            // Load exercises from the database
            LoadExercisesFromDatabase();
        }

        public void LoadExercisesFromDatabase()
        {
            try
            {
                var db = DatabaseService.Connection;

                // Clear the collection first to avoid duplicates
                if (Exercises == null)
                {
                    Exercises = new ObservableCollection<Exercise>();
                }
                Exercises.Clear();

                // Read all exercises from the SQLite database
                var exercisesFromDb = db.Table<Exercise>().ToList();
                // Get all categories
                var categories = db.Table<Category>().ToList();

                if (exercisesFromDb == null)
                {
                    Console.WriteLine("Error: No exercises found in the database.");
                    return; // Exit early if the database query failed
                }

                // Get all categories from the database to check if they still exist
                var existingCategories = DatabaseService.GetAllCategories().Select(c => c.Id).ToHashSet();

                if (existingCategories == null)
                {
                    Console.WriteLine("Error: No categories found in the database.");
                    return; // Exit early if the category query failed
                }

                // Add each exercise to the ObservableCollection
                foreach (var exercise in exercisesFromDb)
                {
                    if (exercise == null)
                    {
                        Console.WriteLine("Error: Encountered a null exercise in the database.");
                        continue; // Skip any null exercises
                    }

                    // If the category does not exist, set it to "Uncategorized"
                    if (exercise.CategoryId != -1 && !existingCategories.Contains(exercise.CategoryId))
                    {
                        exercise.CategoryId = -1; // Assuming -1 represents "Uncategorized" or an invalid category
                        db.Update(exercise); // Update the exercise in the database to save this change
                    }

                    var category = categories.FirstOrDefault(c => c.Id == exercise.CategoryId);
                    exercise.CategoryName = category?.Name ?? "Uncategorized";
                    Exercises.Add(exercise);
                }
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging
                Console.WriteLine($"Error loading exercises: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                // Display error message to the user (DEBUG)
                Application.Current.MainPage.DisplayAlert("Error", "Failed to load exercises.", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
